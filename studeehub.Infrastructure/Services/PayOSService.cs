using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using studeehub.Application.DTOs.Requests.PayOS;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services.ThirdPartyServices;
using studeehub.Domain.Entities;
using studeehub.Domain.Enums;
using studeehub.Domain.Enums.Subscriptions;
using studeehub.Domain.Enums.TransactionStatus;
using System.Security.Cryptography;
using System.Text;

namespace studeehub.Infrastructure.Services
{
	public class PayOSService : IPayOSService
	{
		private readonly PayOS _payOS;
		private readonly IValidator<CreatePaymentLinkRequest> _validator;
		private readonly IConfiguration _configuration;
		private readonly ISubscriptionRepository _subscriptionRepository;
		private readonly ISubPlanRepository _subscriptionPlanRepository;
		private readonly IPayTransactionRepository _paymentTransactionRepository;
		private readonly IUserRepository _userRepository;
		private readonly IEmailService _emailService;
		private readonly IEmailTemplateService _emailTemplateService;
		private readonly IUnitOfWork _unitOfWork;


		public PayOSService(
			PayOS payOS,
			IValidator<CreatePaymentLinkRequest> validator,
			IConfiguration configuration,
			ISubscriptionRepository subscriptionRepository,
			ISubPlanRepository subscriptionPlanRepository,
			IUserRepository userRepository,
			IPayTransactionRepository paymentTransactionRepository,
			IUnitOfWork unitOfWork,
			IEmailService emailService,
			IEmailTemplateService emailTemplateService)
		{
			_payOS = payOS;
			_validator = validator;
			_configuration = configuration;
			_subscriptionRepository = subscriptionRepository;
			_subscriptionPlanRepository = subscriptionPlanRepository;
			_userRepository = userRepository;
			_paymentTransactionRepository = paymentTransactionRepository;
			_unitOfWork = unitOfWork;
			_emailService = emailService;
			_emailTemplateService = emailTemplateService;
		}

		public async Task<BaseResponse<PaymentLinkInformation>> GetPaymentInformationByOrderCode(long orderCode)
		{
			try
			{
				if (orderCode <= 0)
				{
					return BaseResponse<PaymentLinkInformation>.Fail("Payment ID is required and must be positive integer.", ErrorType.Validation);
				}
				var data = await _payOS.getPaymentLinkInformation(orderCode);
				if (data == null)
				{
					return BaseResponse<PaymentLinkInformation>.Fail("Payment information not found.", ErrorType.NotFound);
				}
				return BaseResponse<PaymentLinkInformation>.Ok(data, "Payment information retrieved.");
			}
			catch (Exception ex)
			{
				var errors = new List<string> { ex.Message };
				return BaseResponse<PaymentLinkInformation>.Fail("An error occurred while retrieving payment information.", ErrorType.ServerError, errors);
			}
		}

		public async Task<BaseResponse<CreatePaymentResult>> CreatePaymentLink(CreatePaymentLinkRequest request)
		{
			try
			{
				var resultValidation = await _validator.ValidateAsync(request);
				if (!resultValidation.IsValid)
				{
					var errors = resultValidation.Errors.Select(e => e.ErrorMessage).ToList();
					return BaseResponse<CreatePaymentResult>.Fail("Invalid request data.", ErrorType.Validation, errors);
				}

				// Load target plan early so we can decide whether to allow creating a new payment link
				var plan = await _subscriptionPlanRepository.GetByConditionAsync(sp => sp.Id == request.SubscriptionPlanId);
				if (plan == null)
				{
					return BaseResponse<CreatePaymentResult>.Fail("Subscription plan not found", ErrorType.NotFound);
				}

				// Check if user already has an active (non-upgradeable) subscription.
				var existing = await _subscriptionRepository.GetByConditionAsync(
					s => s.UserId == request.UserId && s.Status == SubscriptionStatus.Active,
					include: q => q.Include(s => s.SubscriptionPlan));

				if (existing != null)
				{
					// If user has an active free subscription (price == 0) and the requested plan is paid, allow creating a payment link (upgrade).
					// Otherwise block as user already has an active subscription.
					var existingPlanPrice = existing.SubscriptionPlan?.Price ?? 0m;
					if (!(existingPlanPrice <= 0m && plan.Price > 0m))
					{
						return BaseResponse<CreatePaymentResult>.Fail("User already has an active subscription", ErrorType.Validation);
					}
				}

				// If plan is free (price == 0) -> no payment link creation. Activate subscription immediately.
				if (plan.Price <= 0m)
				{
					// Ensure we have user info for email and buyer metadata
					var freeUser = await _userRepository.GetByConditionAsync(u => u.Id == request.UserId);
					if (freeUser == null)
					{
						return BaseResponse<CreatePaymentResult>.Fail("User not found", ErrorType.NotFound);
					}

					var tx = await _unitOfWork.BeginTransactionAsync();
					try
					{
						var now = DateTime.UtcNow;
						var subscription = new Subscription
						{
							Id = Guid.NewGuid(),
							UserId = request.UserId!.Value,
							SubscriptionPlanId = request.SubscriptionPlanId!.Value,
							Status = SubscriptionStatus.Active,
							StartDate = now,
							EndDate = plan.DurationInDays > 0 ? now.AddDays(plan.DurationInDays) : now,
							IsPreExpiryNotified = false,
							IsPostExpiryNotified = false,
							CreatedAt = now,
							UpdatedAt = now
						};

						await _subscriptionRepository.AddAsync(subscription);

						var saved = await _unitOfWork.SaveChangesAsync();
						if (!saved)
						{
							await _unitOfWork.RollbackAsync(tx);
							return BaseResponse<CreatePaymentResult>.Fail("Failed to save subscription data", ErrorType.ServerError);
						}

						await _unitOfWork.CommitAsync(tx);

						// No CreatePaymentResult to return for free plans; return success with null data
						return BaseResponse<CreatePaymentResult>.Ok(null!, "Free subscription activated");
					}
					catch (Exception dbEx)
					{
						await _unitOfWork.RollbackAsync(tx);
						return BaseResponse<CreatePaymentResult>.Fail("Failed to create free subscription", ErrorType.ServerError, new List<string> { dbEx.Message });
					}
				}

				var items = new List<ItemData>();
				var item = new ItemData(plan.Name, 1, (int)plan.Price);
				items.Add(item);

				var user = await _userRepository.GetByConditionAsync(u => u.Id == request.UserId);
				if (user == null)
				{
					return BaseResponse<CreatePaymentResult>.Fail("User not found", ErrorType.NotFound);
				}
				var buyerName = user.FullName ?? user.UserName;
				var buyerEmail = user.Email;
				var buyerPhone = user.PhoneNumber;
				var buyerAddress = user.Address;
				long expiredAt = new DateTimeOffset(DateTime.UtcNow.AddHours(1)).ToUnixTimeSeconds();

				long orderCode = DateTimeOffset.Now.ToUnixTimeMilliseconds();
				int amount = items.Sum(i => i.price * i.quantity);
				var desciption = string.IsNullOrWhiteSpace(request.Description)
					? "Thanh toan don hang"
					: request.Description;

				// Build signature payload sorted alphabetically by key:
				// amount=$amount&cancelUrl=$cancelUrl&description=$description&orderCode=$orderCode&returnUrl=$returnUrl
				var payload = $"amount={amount}&cancelUrl={request.CancelUrl}&description={desciption}&orderCode={orderCode}&returnUrl={request.ReturnUrl}";

				// Compute HMAC-SHA256 using checksum key from configuration and produce lowercase hex string
				var checksumKey = _configuration.GetValue<string>("PayOS:ChecksumKey");
				if (string.IsNullOrWhiteSpace(checksumKey))
					return BaseResponse<CreatePaymentResult>.Fail("Payment configuration error", ErrorType.ServerError);

				var keyBytes = Encoding.UTF8.GetBytes(checksumKey);
				var payloadBytes = Encoding.UTF8.GetBytes(payload);
				using var hmac = new HMACSHA256(keyBytes);
				var hash = hmac.ComputeHash(payloadBytes);
				var signature = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

				var paymentData = new PaymentData(orderCode, amount, desciption, items, request.CancelUrl, request.ReturnUrl,
							signature, buyerName, buyerEmail, buyerPhone, buyerAddress, expiredAt);

				var createPayment = await _payOS.createPaymentLink(paymentData);

				if (createPayment == null)
					return BaseResponse<CreatePaymentResult>.Fail("Failed to create payment link", ErrorType.ServerError);

				// Use UnitOfWork for atomic creation of subscription & transaction
				var tx2 = await _unitOfWork.BeginTransactionAsync();
				try
				{
					var subscription = new Subscription
					{
						Id = Guid.NewGuid(),
						UserId = request.UserId!.Value,
						SubscriptionPlanId = request.SubscriptionPlanId!.Value,
						Status = SubscriptionStatus.Pending,
						CreatedAt = DateTime.UtcNow,
						UpdatedAt = DateTime.UtcNow
					};

					await _subscriptionRepository.AddAsync(subscription);

					var paymentTransaction = new PaymentTransaction
					{
						Id = Guid.NewGuid(),
						UserId = request.UserId.Value,
						SubscriptionId = subscription.Id,
						PaymentLinkId = createPayment.paymentLinkId,
						PaymentMethod = "PAYOS",
						TransactionCode = createPayment.orderCode.ToString(),
						Amount = Convert.ToDecimal(createPayment.amount),
						Currency = string.IsNullOrWhiteSpace(createPayment.currency) ? "VND" : createPayment.currency,
						Status = TransactionStatus.Pending,
						CreatedAt = DateTime.UtcNow
					};

					await _paymentTransactionRepository.AddAsync(paymentTransaction);

					// Persist via UnitOfWork (single SaveChanges)
					var saved = await _unitOfWork.SaveChangesAsync();
					if (!saved)
					{
						await _unitOfWork.RollbackAsync(tx2);
						return BaseResponse<CreatePaymentResult>.Fail("Failed to save subscription and transaction data", ErrorType.ServerError);
					}

					await _unitOfWork.CommitAsync(tx2);
				}
				catch (Exception dbEx)
				{
					await _unitOfWork.RollbackAsync(tx2);
					return BaseResponse<CreatePaymentResult>.Fail("Failed to save subscription and transaction data", ErrorType.ServerError, new List<string> { dbEx.Message });
				}

				return BaseResponse<CreatePaymentResult>.Ok(createPayment, "Payment link created");
			}
			catch (Exception ex)
			{
				var errors = new List<string> { ex.Message };
				return BaseResponse<CreatePaymentResult>.Fail("An unexpected error occurred.", ErrorType.ServerError, errors);
			}
		}

		public async Task<BaseResponse<PaymentLinkInformation>> CancelPaymentLink(long orderCode, string? cancellationReason)
		{
			try
			{
				if (orderCode <= 0)
				{
					return BaseResponse<PaymentLinkInformation>.Fail("Payment ID is required and must be positive integer.", ErrorType.Validation);
				}
				var data = await _payOS.cancelPaymentLink(orderCode, cancellationReason);
				if (data == null)
				{
					return BaseResponse<PaymentLinkInformation>.Fail("Payment information not found.", ErrorType.NotFound);
				}

				var transaction = await _paymentTransactionRepository.GetByConditionAsync(pt => pt.TransactionCode == orderCode.ToString() && pt.Status == TransactionStatus.Pending);
				if (transaction == null)
				{
					return BaseResponse<PaymentLinkInformation>.Fail("Cannot cancel a transaction that has already been processed.", ErrorType.Conflict);
				}

				// use UnitOfWork transaction to update entities atomically
				var tx = await _unitOfWork.BeginTransactionAsync();
				try
				{
					transaction.Status = TransactionStatus.Cancelled;
					transaction.CancellationReason = cancellationReason;
					transaction.CompletedAt = data.canceledAt != null ? DateTime.Parse(data.canceledAt) : DateTime.UtcNow;
					_paymentTransactionRepository.Update(transaction);

					// attempt to update subscription linked to this payment (best-effort)
					var subscription = await _subscriptionRepository.GetByConditionAsync(s => s.Id == transaction.SubscriptionId);
					if (subscription != null)
					{
						subscription.Status = SubscriptionStatus.Cancelled;
						subscription.UpdatedAt = DateTime.UtcNow;
						_subscriptionRepository.Update(subscription);
					}

					var saved = await _unitOfWork.SaveChangesAsync();
					if (!saved)
					{
						await _unitOfWork.RollbackAsync(tx);
						return BaseResponse<PaymentLinkInformation>.Fail("Failed to update local records after cancellation", ErrorType.ServerError);
					}

					await _unitOfWork.CommitAsync(tx);
				}
				catch (Exception dbEx)
				{
					await _unitOfWork.RollbackAsync(tx);
					return BaseResponse<PaymentLinkInformation>.Fail("Failed to update local records after cancellation", ErrorType.ServerError, new List<string> { dbEx.Message });
				}

				return BaseResponse<PaymentLinkInformation>.Ok(data, "Payment link cancelled.");
			}
			catch (Exception ex)
			{
				var errors = new List<string> { ex.Message };
				return BaseResponse<PaymentLinkInformation>.Fail("An error occurred while cancelling the payment link.", ErrorType.ServerError, errors);
			}
		}

		public async Task<BaseResponse<string>> ConfirmWebHook(string webhookUrl)
		{
			try
			{
				// confirmWebhook returns a URL or token depending on SDK; treat as string
				var confirmed = await _payOS.confirmWebhook(webhookUrl);
				if (string.IsNullOrWhiteSpace(confirmed))
				{
					return BaseResponse<string>.Fail("Failed to confirm webhook", ErrorType.ServerError);
				}

				return BaseResponse<string>.Ok(confirmed, "Webhook confirmed.");
			}
			catch (Exception ex)
			{
				var errors = new List<string> { ex.Message };
				return BaseResponse<string>.Fail("Failed to confirm webhook", ErrorType.ServerError, errors);
			}
		}

		public async Task<BaseResponse<WebhookData>> TransferHandler(WebhookType body)
		{
			try
			{
				if (body == null)
					return BaseResponse<WebhookData>.Fail("Request body is required", ErrorType.Validation);

				// verifyPaymentWebhookData is provided by the PayOS SDK; returns a WebhookData instance or null
				var data = _payOS.verifyPaymentWebhookData(body);
				if (data == null)
				{
					return BaseResponse<WebhookData>.Fail("Invalid webhook payload", ErrorType.Validation);
				}

				var transaction = await _paymentTransactionRepository.GetByConditionAsync(pt => pt.TransactionCode == data.orderCode.ToString());
				if (transaction == null)
				{
					return BaseResponse<WebhookData>.Fail("Transaction not found for the given order code", ErrorType.NotFound);
				}

				// Use UnitOfWork transaction while updating payment + subscription
				var tx = await _unitOfWork.BeginTransactionAsync();
				try
				{
					// set response code once
					transaction.ResponseCode = data.code;

					var paymentSucceeded = data.code == "00";
					var parsedComplete = DateTime.TryParse(data.transactionDateTime ?? string.Empty, out var parsed) ? parsed : DateTime.UtcNow;
					Subscription? subscription = null;

					// load subscription including navigation properties so we can email
					subscription = await _subscriptionRepository.GetByConditionAsync(
						s => s.Id == transaction.SubscriptionId,
						include: q => q.Include(s => s.User).Include(s => s.SubscriptionPlan),
						asNoTracking: false);

					if (paymentSucceeded)
					{
						transaction.Status = TransactionStatus.Success;
						transaction.CompletedAt = parsedComplete;

						if (subscription != null)
						{
							subscription.Status = SubscriptionStatus.Active;
							subscription.StartDate = parsedComplete;

							var plan = subscription.SubscriptionPlan ?? await _subscriptionPlanRepository.GetByConditionAsync(sp => sp.Id == subscription.SubscriptionPlanId);
							if (plan != null)
								subscription.EndDate = parsedComplete.AddDays(plan.DurationInDays);

							subscription.UpdatedAt = DateTime.UtcNow;
							_subscriptionRepository.Update(subscription);

							// If this subscription is an upgrade from a free plan, find the previous active free subscription and deactivate it
							var previousActive = await _subscriptionRepository.GetByConditionAsync(
								s => s.UserId == subscription.UserId && s.Status == SubscriptionStatus.Active && s.Id != subscription.Id,
								include: q => q.Include(s => s.SubscriptionPlan));

							if (previousActive != null)
							{
								var prevPlan = previousActive.SubscriptionPlan ?? await _subscriptionPlanRepository.GetByConditionAsync(sp => sp.Id == previousActive.SubscriptionPlanId);
								if (prevPlan != null && prevPlan.Price <= 0m)
								{
									previousActive.Status = SubscriptionStatus.Cancelled;
									previousActive.UpdatedAt = DateTime.UtcNow;
									_subscriptionRepository.Update(previousActive);
								}
							}
						}
					}
					else
					{
						transaction.Status = TransactionStatus.Failed;
						transaction.CompletedAt = parsedComplete;

						if (subscription != null)
						{
							subscription.Status = SubscriptionStatus.Failed;
							subscription.UpdatedAt = DateTime.UtcNow;
							_subscriptionRepository.Update(subscription);
						}
					}

					_paymentTransactionRepository.Update(transaction);
					var saved = await _unitOfWork.SaveChangesAsync();
					if (!saved)
					{
						await _unitOfWork.RollbackAsync(tx);
						return BaseResponse<WebhookData>.Fail("Failed to persist webhook updates", ErrorType.ServerError);
					}

					// Send activation email if payment succeeded. If email fails we log it but do not rollback DB changes.
					if (paymentSucceeded && subscription != null)
					{
						var userEmail = subscription.User?.Email;
						if (!string.IsNullOrWhiteSpace(userEmail))
						{
							var fullName = subscription.User?.UserName ?? userEmail;
							var planName = subscription.SubscriptionPlan?.Name ?? "your plan";
							var endDate = subscription.EndDate;

							var subject = $"Your subscription \"{planName}\" is now active";
							var htmlBody = _emailTemplateService.SubscriptionActivatedTemplate(fullName, planName, endDate);

							await _emailService.SendEmailAsync(userEmail, subject, htmlBody);
						}
					}

					await _unitOfWork.CommitAsync(tx);
				}
				catch (Exception dbEx)
				{
					await _unitOfWork.RollbackAsync(tx);
					return BaseResponse<WebhookData>.Fail("Failed to process webhook", ErrorType.ServerError, new List<string> { dbEx.Message });
				}

				// Return verified webhook data to caller for further processing by controller/service caller
				return BaseResponse<WebhookData>.Ok(data, "Webhook data verified");
			}
			catch (Exception ex)
			{
				var errors = new List<string> { ex.Message };
				return BaseResponse<WebhookData>.Fail("Failed to process webhook", ErrorType.ServerError, errors);
			}
		}
	}
}
