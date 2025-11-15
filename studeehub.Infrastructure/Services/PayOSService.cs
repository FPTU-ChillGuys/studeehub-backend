using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PayOS;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;
using studeehub.Application.DTOs.Requests.PayOS;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services.ThirdPartyServices;
using studeehub.Domain.Entities;
using studeehub.Domain.Enums;
using studeehub.Domain.Enums.Subscriptions;
using studeehub.Domain.Enums.TransactionStatus;

namespace studeehub.Infrastructure.Services
{
	public class PayOSService : IPayOSService
	{
		private readonly PayOSClient _payOS;
		private readonly IValidator<CreateLinkRequest> _validator;
		private readonly IConfiguration _configuration;
		private readonly ISubscriptionRepository _subscriptionRepository;
		private readonly ISubPlanRepository _subscriptionPlanRepository;
		private readonly IPayTransactionRepository _paymentTransactionRepository;
		private readonly IUserRepository _userRepository;
		private readonly IEmailService _emailService;
		private readonly IEmailTemplateService _emailTemplateService;
		private readonly IUnitOfWork _unitOfWork;


		public PayOSService(
			PayOSClient payOS,
			IValidator<CreateLinkRequest> validator,
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

		public async Task<BaseResponse<CreatePaymentLinkResponse>> CreatePaymentLink(CreateLinkRequest request)
		{
			try
			{
				var resultValidation = await _validator.ValidateAsync(request);
				if (!resultValidation.IsValid)
				{
					var errors = resultValidation.Errors.Select(e => e.ErrorMessage).ToList();
					return BaseResponse<CreatePaymentLinkResponse>.Fail("Invalid request data.", ErrorType.Validation, errors);
				}

				var existing = await _subscriptionRepository.GetByConditionAsync(
				s => s.UserId == request.UserId && s.Status == SubscriptionStatus.Active);
				if (existing != null)
					return BaseResponse<CreatePaymentLinkResponse>.Fail("User already has an active subscription", ErrorType.Validation);

				var items = new List<PaymentLinkItem>();
				var plan = await _subscriptionPlanRepository.GetByConditionAsync(sp => sp.Id == request.SubscriptionPlanId);
				if (plan == null)
				{
					return BaseResponse<CreatePaymentLinkResponse>.Fail("Subscription plan not found", ErrorType.NotFound);
				}

				// If plan is free (price == 0) -> no payment link creation. Activate subscription immediately.
				if (plan.Price <= 0m)
				{
					// Ensure we have user info for email and buyer metadata
					var freeUser = await _userRepository.GetByConditionAsync(u => u.Id == request.UserId);
					if (freeUser == null)
					{
						return BaseResponse<CreatePaymentLinkResponse>.Fail("User not found", ErrorType.NotFound);
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
							return BaseResponse<CreatePaymentLinkResponse>.Fail("Failed to save subscription data", ErrorType.ServerError);
						}

						await _unitOfWork.CommitAsync(tx);

						// No CreatePaymentResult to return for free plans; return success with null data
						return BaseResponse<CreatePaymentLinkResponse>.Ok(null!, "Free subscription activated");
					}
					catch (Exception dbEx)
					{
						await _unitOfWork.RollbackAsync(tx);
						return BaseResponse<CreatePaymentLinkResponse>.Fail("Failed to create free subscription", ErrorType.ServerError, new List<string> { dbEx.Message });
					}
				}

				var item = new PaymentLinkItem
				{
					Name = plan.Name,
					Quantity = 1,
					Price = (long)plan.Price,
					Unit = null,
					TaxPercentage = null
				};
				items.Add(item);

				var user = await _userRepository.GetByConditionAsync(u => u.Id == request.UserId);
				if (user == null)
				{
					return BaseResponse<CreatePaymentLinkResponse>.Fail("User not found", ErrorType.NotFound);
				}
				var buyerName = user.FullName ?? user.UserName;
				var buyerEmail = user.Email;
				var buyerPhone = user.PhoneNumber;
				var buyerAddress = user.Address;
				long expiredAt = new DateTimeOffset(DateTime.UtcNow.AddHours(1)).ToUnixTimeSeconds();

				long orderCode = DateTimeOffset.Now.ToUnixTimeMilliseconds();
				long amount = items.Sum(i => i.Price * i.Quantity);
				var desciption = string.IsNullOrWhiteSpace(request.Description)
					? "Thanh toan don hang"
					: request.Description;

				// Build SDK payment request
				var paymentData = new CreatePaymentLinkRequest
				{
					OrderCode = orderCode,
					Amount = amount,
					Description = desciption,
					ReturnUrl = request.ReturnUrl,
					CancelUrl = request.CancelUrl,
					Items = items,
					BuyerName = buyerName,
					BuyerEmail = buyerEmail,
					BuyerPhone = buyerPhone,
					BuyerAddress = buyerAddress,
					ExpiredAt = expiredAt
				};

				// Create signature using SDK Crypto helper instead of manual HMAC
				var checksumKey = _configuration.GetValue<string>("PayOS:ChecksumKey");
				if (string.IsNullOrWhiteSpace(checksumKey))
					return BaseResponse<CreatePaymentLinkResponse>.Fail("Payment configuration error", ErrorType.ServerError);

				var signature = _payOS.Crypto.CreateSignatureOfPaymentRequest(paymentData, checksumKey);
				paymentData.Signature = signature;

				var createPayment = await _payOS.PaymentRequests.CreateAsync(paymentData);

				if (createPayment == null)
					return BaseResponse<CreatePaymentLinkResponse>.Fail("Failed to create payment link", ErrorType.ServerError);

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

					var paymentTransaction = new Domain.Entities.PaymentTransaction
					{
						Id = Guid.NewGuid(),
						UserId = request.UserId.Value,
						SubscriptionId = subscription.Id,
						PaymentLinkId = createPayment.PaymentLinkId,
						PaymentMethod = "PAYOS",
						TransactionCode = createPayment.OrderCode.ToString(),
						Amount = Convert.ToDecimal(createPayment.Amount),
						Currency = string.IsNullOrWhiteSpace(createPayment.Currency) ? "VND" : createPayment.Currency,
						Status = TransactionStatus.Pending,
						CreatedAt = DateTime.UtcNow
					};

					await _paymentTransactionRepository.AddAsync(paymentTransaction);

					// Persist via UnitOfWork (single SaveChanges)
					var saved = await _unitOfWork.SaveChangesAsync();
					if (!saved)
					{
						await _unitOfWork.RollbackAsync(tx2);
						return BaseResponse<CreatePaymentLinkResponse>.Fail("Failed to save subscription and transaction data", ErrorType.ServerError);
					}

					await _unitOfWork.CommitAsync(tx2);
				}
				catch (Exception dbEx)
				{
					await _unitOfWork.RollbackAsync(tx2);
					return BaseResponse<CreatePaymentLinkResponse>.Fail("Failed to save subscription and transaction data", ErrorType.ServerError, new List<string> { dbEx.Message });
				}

				return BaseResponse<CreatePaymentLinkResponse>.Ok(createPayment, "Payment link created");
			}
			catch (Exception ex)
			{
				var errors = new List<string> { ex.Message };
				return BaseResponse<CreatePaymentLinkResponse>.Fail("An unexpected error occurred.", ErrorType.ServerError, errors);
			}
		}

		public async Task<BaseResponse<ConfirmWebhookResponse>> ConfirmWebHook(ConfirmWebhookRequest request)
		{
			try
			{
				var confirmResult = await _payOS.Webhooks.ConfirmAsync(request.WebhookUrl);
				if (confirmResult == null)
				{
					return BaseResponse<ConfirmWebhookResponse>.Fail("Failed to confirm webhook", ErrorType.ServerError);
				}
				return BaseResponse<ConfirmWebhookResponse>.Ok(confirmResult, "Webhook confirmed successfully");
			}
			catch (Exception ex)
			{
				var errors = new List<string> { ex.Message };
				return BaseResponse<ConfirmWebhookResponse>.Fail("Failed to confirm webhook", ErrorType.ServerError, errors);
			}
		}

		public async Task<BaseResponse<WebhookData>> TransferHandler(Webhook body)
		{
			try
			{
				if (body == null)
					return BaseResponse<WebhookData>.Fail("Request body is required", ErrorType.Validation);

				// Verify webhook using SDK
				var data = await _payOS.Webhooks.VerifyAsync(body);
				if (data == null)
				{
					return BaseResponse<WebhookData>.Fail("Invalid webhook payload", ErrorType.Validation);
				}

				var transaction = await _paymentTransactionRepository.GetByConditionAsync(pt => pt.TransactionCode == data.OrderCode.ToString());
				if (transaction == null)
				{
					return BaseResponse<WebhookData>.Fail("Transaction not found for the given order code", ErrorType.NotFound);
				}

				var tx = await _unitOfWork.BeginTransactionAsync();
				try
				{
					transaction.ResponseCode = data.Code;
					var paymentSucceeded = data.Code == "00";
					var parsedComplete = DateTime.TryParse(data.TransactionDateTime ?? string.Empty, out var parsed) ? parsed : DateTime.UtcNow;
					Subscription? subscription = null;

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
