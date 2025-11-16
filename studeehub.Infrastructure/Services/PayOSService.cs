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

				// Validate and load required entities
				var (existingSubscription, plan, user, validationError) = await ValidateAndLoadEntitiesAsync(request);
				if (validationError != null)
					return validationError;

				// Prevent creating a payment/link or activating subscription when the user already has an active subscription
				// Allow upgrade when existing active plan is free (price <= 0) and requested plan is paid (price > 0)
				if (existingSubscription == null)
				{
					var existingActive = await _subscriptionRepository.GetByConditionAsync(
						 s => s.UserId == user.Id && s.Status == SubscriptionStatus.Active,
						 include: q => q.Include(s => s.SubscriptionPlan));

					if (existingActive != null)
					{
						var existingPlanPrice = existingActive.SubscriptionPlan?.Price ?? 0m;
						if (!(existingPlanPrice <= 0m && plan.Price > 0m))
						{
							return BaseResponse<CreatePaymentResult>.Fail("User already has an active subscription", ErrorType.Validation);
						}
					}
				}

				// If free plan, activate (either existing pending or create active subscription)
				var freePlanResult = await HandleFreePlanActivationAsync(existingSubscription, plan, user);
				if (freePlanResult != null)
					return freePlanResult;

				// Prepare payment details
				long orderCode = DateTimeOffset.Now.ToUnixTimeMilliseconds();
				var items = new List<ItemData> { new ItemData(plan.Name, 1, (int)plan.Price) };
				int amount = items.Sum(i => i.price * i.quantity);

				var paymentData = BuildPaymentData(request, plan, user, orderCode, amount, items);

				// Create payment link on provider
				var createPayment = await _payOS.createPaymentLink(paymentData);
				if (createPayment == null)
					return BaseResponse<CreatePaymentResult>.Fail("Failed to create payment link", ErrorType.ServerError);

				// Persist subscription (if needed) and payment transaction atomically
				var persistResult = await PersistSubscriptionAndTransactionAsync(createPayment, existingSubscription, plan, user);
				return persistResult;
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

		// --- Helper methods ---

		private async Task<(Subscription? existingSubscription, SubscriptionPlan plan, User user, BaseResponse<CreatePaymentResult>? error)> ValidateAndLoadEntitiesAsync(CreatePaymentLinkRequest request)
		{
			// If SubscriptionId provided, load and ensure it's pending
			if (request.SubscriptionId is Guid subscriptionId)
			{
				var existingSubscription = await _subscriptionRepository.GetByConditionAsync(
					s => s.Id == subscriptionId,
					include: q => q.Include(s => s.SubscriptionPlan).Include(s => s.User));

				if (existingSubscription == null)
					return (null!, null!, null!, BaseResponse<CreatePaymentResult>.Fail("Subscription not found", ErrorType.NotFound));

				if (existingSubscription.Status != SubscriptionStatus.Pending)
					return (null!, null!, null!, BaseResponse<CreatePaymentResult>.Fail("Only pending subscriptions can be used to create a payment link.", ErrorType.Conflict));

				var plan = existingSubscription.SubscriptionPlan ?? await _subscriptionPlanRepository.GetByConditionAsync(sp => sp.Id == existingSubscription.SubscriptionPlanId);
				if (plan == null)
					return (null!, null!, null!, BaseResponse<CreatePaymentResult>.Fail("Subscription plan not found", ErrorType.NotFound));

				var user = existingSubscription.User ?? await _userRepository.GetByConditionAsync(u => u.Id == existingSubscription.UserId);
				if (user == null)
					return (null!, null!, null!, BaseResponse<CreatePaymentResult>.Fail("User not found", ErrorType.NotFound));

				return (existingSubscription, plan, user, null);
			}

			// No subscriptionId: require both SubscriptionPlanId and UserId
			var subscriptionPlanId = request.SubscriptionPlanId;
			var userId = request.UserId;

			if (subscriptionPlanId == Guid.Empty || userId == Guid.Empty)
				return (null!, null!, null!, BaseResponse<CreatePaymentResult>.Fail("SubscriptionPlanId and UserId are required when SubscriptionId is not provided.", ErrorType.Validation));

			var planResult = await _subscriptionPlanRepository.GetByConditionAsync(sp => sp.Id == subscriptionPlanId);
			if (planResult == null)
				return (null!, null!, null!, BaseResponse<CreatePaymentResult>.Fail("Subscription plan not found", ErrorType.NotFound));

			var userResult = await _userRepository.GetByConditionAsync(u => u.Id == userId);
			if (userResult == null)
				return (null!, null!, null!, BaseResponse<CreatePaymentResult>.Fail("User not found", ErrorType.NotFound));

			return (null, planResult, userResult, null);
		}

		private async Task<BaseResponse<CreatePaymentResult>?> HandleFreePlanActivationAsync(Subscription? existingSubscription, SubscriptionPlan plan, User user)
		{
			if (plan.Price > 0m) return null; // not a free plan

			var tx = await _unitOfWork.BeginTransactionAsync();
			try
			{
				var now = DateTime.UtcNow;
				if (existingSubscription != null)
				{
					existingSubscription.Status = SubscriptionStatus.Active;
					existingSubscription.StartDate = now;
					existingSubscription.EndDate = plan.DurationInDays > 0 ? now.AddDays(plan.DurationInDays) : now;
					existingSubscription.IsPreExpiryNotified = false;
					existingSubscription.IsPostExpiryNotified = false;
					existingSubscription.UpdatedAt = now;

					_subscriptionRepository.Update(existingSubscription);
				}
				else
				{
					var subscription = new Subscription
					{
						Id = Guid.NewGuid(),
						UserId = user.Id,
						SubscriptionPlanId = plan.Id,
						Status = SubscriptionStatus.Active,
						StartDate = now,
						EndDate = plan.DurationInDays > 0 ? now.AddDays(plan.DurationInDays) : now,
						IsPreExpiryNotified = false,
						IsPostExpiryNotified = false,
						CreatedAt = now,
						UpdatedAt = now
					};

					await _subscriptionRepository.AddAsync(subscription);
				}

				var saved = await _unitOfWork.SaveChangesAsync();
				if (!saved)
				{
					await _unitOfWork.RollbackAsync(tx);
					return BaseResponse<CreatePaymentResult>.Fail("Failed to process free subscription", ErrorType.ServerError);
				}

				await _unitOfWork.CommitAsync(tx);
				return BaseResponse<CreatePaymentResult>.Ok(null!, "Free subscription activated");
			}
			catch (Exception dbEx)
			{
				await _unitOfWork.RollbackAsync(tx);
				return BaseResponse<CreatePaymentResult>.Fail("Failed to process free subscription", ErrorType.ServerError, new List<string> { dbEx.Message });
			}
		}

		private PaymentData BuildPaymentData(CreatePaymentLinkRequest request, SubscriptionPlan plan, User user, long orderCode, int amount, List<ItemData> items)
		{
			var desciption = string.IsNullOrWhiteSpace(request.Description) ? "Thanh toan don hang" : request.Description;
			long expiredAt = new DateTimeOffset(DateTime.UtcNow.AddHours(1)).ToUnixTimeSeconds();

			// Build signature payload
			var payload = $"amount={amount}&cancelUrl={request.CancelUrl}&description={desciption}&orderCode={orderCode}&returnUrl={request.ReturnUrl}";

			var checksumKey = _configuration.GetValue<string>("PayOS:ChecksumKey");
			if (string.IsNullOrWhiteSpace(checksumKey))
				throw new InvalidOperationException("Payment configuration error");

			var keyBytes = Encoding.UTF8.GetBytes(checksumKey);
			var payloadBytes = Encoding.UTF8.GetBytes(payload);
			using var hmac = new HMACSHA256(keyBytes);
			var hash = hmac.ComputeHash(payloadBytes);
			var signature = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

			return new PaymentData(orderCode, amount, desciption, items, request.CancelUrl, request.ReturnUrl,
				signature, user.FullName ?? user.UserName, user.Email, user.PhoneNumber, user.Address, expiredAt);
		}

		private async Task<BaseResponse<CreatePaymentResult>> PersistSubscriptionAndTransactionAsync(CreatePaymentResult createPayment, Subscription? existingSubscription, SubscriptionPlan plan, User user)
		{
			var tx = await _unitOfWork.BeginTransactionAsync();
			try
			{
				if (existingSubscription == null)
				{
					var subscription = new Subscription
					{
						Id = Guid.NewGuid(),
						UserId = user.Id,
						SubscriptionPlanId = plan.Id,
						Status = SubscriptionStatus.Pending,
						CreatedAt = DateTime.UtcNow,
						UpdatedAt = DateTime.UtcNow
					};

					await _subscriptionRepository.AddAsync(subscription);

					var paymentTransaction = new PaymentTransaction
					{
						Id = Guid.NewGuid(),
						UserId = user.Id,
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
				}
				else
				{
					var paymentTransaction = new PaymentTransaction
					{
						Id = Guid.NewGuid(),
						UserId = existingSubscription.UserId,
						SubscriptionId = existingSubscription.Id,
						PaymentLinkId = createPayment.paymentLinkId,
						PaymentMethod = "PAYOS",
						TransactionCode = createPayment.orderCode.ToString(),
						Amount = Convert.ToDecimal(createPayment.amount),
						Currency = string.IsNullOrWhiteSpace(createPayment.currency) ? "VND" : createPayment.currency,
						Status = TransactionStatus.Pending,
						CreatedAt = DateTime.UtcNow
					};

					await _paymentTransactionRepository.AddAsync(paymentTransaction);
				}

				var saved = await _unitOfWork.SaveChangesAsync();
				if (!saved)
				{
					await _unitOfWork.RollbackAsync(tx);
					return BaseResponse<CreatePaymentResult>.Fail("Failed to save subscription and transaction data", ErrorType.ServerError);
				}

				await _unitOfWork.CommitAsync(tx);
				return BaseResponse<CreatePaymentResult>.Ok(createPayment, "Payment link created");
			}
			catch (Exception dbEx)
			{
				await _unitOfWork.RollbackAsync(tx);
				return BaseResponse<CreatePaymentResult>.Fail("Failed to save subscription and transaction data", ErrorType.ServerError, new List<string> { dbEx.Message });
			}
		}
	}
}
