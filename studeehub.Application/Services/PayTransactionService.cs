using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using studeehub.Application.DTOs.Requests.PaymentTransaction;
using studeehub.Application.DTOs.Requests.VnPay;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.DTOs.Responses.PayTransaction;
using studeehub.Application.Interfaces.Repositories;
using studeehub.Application.Interfaces.Services;
using studeehub.Application.Interfaces.Services.ThirdPartyServices;
using studeehub.Domain.Entities;
using studeehub.Domain.Enums;
using studeehub.Domain.Enums.Subscriptions;
using studeehub.Domain.Enums.TransactionStatus;

namespace studeehub.Application.Services
{
	public class PayTransactionService : IPayTransactionService
	{
		private readonly IGenericRepository<PaymentTransaction> _paymentTransactionRepository;
		private readonly IGenericRepository<Subscription> _subscriptionRepository;
		private readonly IGenericRepository<SubscriptionPlan> _subscriptionPlanRepository;
		private readonly IVnPayService _vnPayService;
		private readonly IEmailService _emailService;
		private readonly IEmailTemplateService _emailTemplateService;
		private readonly ILogger<PayTransactionService> _logger;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public PayTransactionService(
			IGenericRepository<PaymentTransaction> paymentTransactionRepository,
			IGenericRepository<Subscription> subscriptionRepository,
			IGenericRepository<SubscriptionPlan> subscriptionPlanRepository,
			IVnPayService vnPayService,
			ILogger<PayTransactionService> logger,
			IUnitOfWork unitOfWork,
			IEmailService emailService,
			IEmailTemplateService emailTemplateService,
			IMapper mapper)
		{
			_paymentTransactionRepository = paymentTransactionRepository;
			_subscriptionRepository = subscriptionRepository;
			_subscriptionPlanRepository = subscriptionPlanRepository;
			_vnPayService = vnPayService;
			_logger = logger;
			_unitOfWork = unitOfWork;
			_emailService = emailService;
			_emailTemplateService = emailTemplateService;
			_mapper = mapper;
		}

		public async Task<BaseResponse<string>> CreatePaymentSessionAsync(CreatePaymentSessionRequest request, HttpContext httpContext)
		{
			// 1️⃣ Validate plan
			var plan = await _subscriptionPlanRepository.GetByConditionAsync(p => p.Id == request.SubscriptionPlanId && p.IsActive);
			if (plan == null)
				return BaseResponse<string>.Fail("Subscription plan not found or Inactive", ErrorType.NotFound);

			// 2️⃣ Prevent duplicate active subscription
			var existing = await _subscriptionRepository.GetByConditionAsync(
				s => s.UserId == request.UserId && s.Status == SubscriptionStatus.Active);
			if (existing != null)
				return BaseResponse<string>.Fail("User already has an active subscription", ErrorType.Validation);

			// 3️⃣ Start transaction (atomic operation) using UnitOfWork
			using var transaction = await _unitOfWork.BeginTransactionAsync();
			try
			{
				// 4️⃣ Create subscription
				var now = DateTime.UtcNow;
				var subscription = new Subscription
				{
					Id = Guid.NewGuid(),
					UserId = request.UserId,
					SubscriptionPlanId = plan.Id,
					StartDate = now,
					EndDate = now,
					Status = SubscriptionStatus.Pending,
					IsPreExpiryNotified = false,
					IsPostExpiryNotified = false,
					CreatedAt = now,
					UpdatedAt = now // <-- ensure UpdatedAt is initialized
				};

				await _subscriptionRepository.AddAsync(subscription);
				await _unitOfWork.SaveChangesAsync();

				// 5️⃣ Create payment transaction
				var paymentTransaction = new PaymentTransaction
				{
					Id = Guid.NewGuid(),
					UserId = request.UserId,
					SubscriptionId = subscription.Id,
					Amount = plan.Price,
					Currency = string.IsNullOrWhiteSpace(plan.Currency) ? "VND" : plan.Currency,
					PaymentMethod = "VNPAY",
					Status = TransactionStatus.Pending,
					CreatedAt = DateTime.UtcNow
				};
				await _paymentTransactionRepository.AddAsync(paymentTransaction);
				await _unitOfWork.SaveChangesAsync();

				// 6️⃣ Generate VNPay URL
				var vnRequest = new VnPaymentRequest
				{
					PaymentTransactionId = paymentTransaction.Id,
					SubscriptionId = subscription.Id,
					Amount = (int)Math.Round(plan.Price)
				};
				var paymentUrl = _vnPayService.CreatePaymentUrl(httpContext, vnRequest);

				// 7️⃣ Commit transaction
				await _unitOfWork.CommitAsync(transaction);

				return BaseResponse<string>.Ok(paymentUrl, "VNPay payment URL generated");
			}
			catch (Exception ex)
			{
				await _unitOfWork.RollbackAsync(transaction);
				return BaseResponse<string>.Fail($"Error creating payment session: {ex.Message}", ErrorType.ServerError);
			}
		}

		public async Task<BaseResponse<List<GetPayTXNResponse>>> GetPayTransactionsBySubscriptionId(Guid subscriptionId)
		{
			var transactions = await _paymentTransactionRepository.GetAllAsync(
				p => p.SubscriptionId == subscriptionId,
				orderBy: q => q.OrderByDescending(p => p.CreatedAt)
			);
			if (transactions == null || !transactions.Any())
				return BaseResponse<List<GetPayTXNResponse>>.Fail("No payment transactions found for this subscription", ErrorType.NotFound);

			var response = _mapper.Map<List<GetPayTXNResponse>>(transactions);
			return BaseResponse<List<GetPayTXNResponse>>.Ok(response, "Payment transactions retrieved");
		}

		public async Task<BaseResponse<string>> HandleVnPayCallbackAsync(IQueryCollection query)
		{
			// STEP 1: Verify checksum & parse response
			var vnResponse = _vnPayService.PaymentExcute(query);
			if (vnResponse == null)
				return BaseResponse<string>.Fail("Invalid VNPay response", ErrorType.Validation);

			// If signature invalid, do not trust anything
			if (!vnResponse.Success && vnResponse.VnPayResponseCode == null)
				return BaseResponse<string>.Fail("Signature validation failed", ErrorType.Validation);

			// STEP 2: Load the payment transaction
			var payment = await _paymentTransactionRepository.GetByConditionAsync(p => p.Id == vnResponse.TransactionId);
			if (payment == null)
				return BaseResponse<string>.Fail("Payment transaction not found", ErrorType.NotFound);

			// Avoid double processing
			if (payment.Status == TransactionStatus.Success)
				return BaseResponse<string>.Ok("Payment already processed");

			// STEP 3: Start transaction
			using var transaction = await _unitOfWork.BeginTransactionAsync();

			Subscription? subscription = null;
			var paymentSucceeded = false;

			try
			{
				// --- Always update transaction state, even on failure ---
				payment.Status = vnResponse.VnPayResponseCode == "00"
					? TransactionStatus.Success
					: TransactionStatus.Failed;

				payment.ResponseCode = vnResponse.VnPayResponseCode;
				payment.TransactionCode = vnResponse.TransactionNo;
				payment.CompletedAt = DateTime.UtcNow;

				_paymentTransactionRepository.Update(payment);
				await _unitOfWork.SaveChangesAsync();

				// --- Only activate subscription when payment succeeded ---
				if (payment.Status == TransactionStatus.Success)
				{
					subscription = await _subscriptionRepository.GetByConditionAsync(
						s => s.Id == payment.SubscriptionId,
						q => q.Include(s => s.SubscriptionPlan).Include(s => s.User),
						asNoTracking: false
					);

					if (subscription == null)
					{
						return BaseResponse<string>.Fail("Subscription not found for payment", ErrorType.NotFound);
					}

					subscription.Status = SubscriptionStatus.Active;

					// Defensive check: ensure SubscriptionPlan is present
					if (subscription.SubscriptionPlan == null)
					{
						return BaseResponse<string>.Fail("Subscription plan data missing", ErrorType.ServerError);
					}

					// Extend subscription duration properly
					var startDate = subscription.EndDate > DateTime.UtcNow
						? subscription.EndDate
						: DateTime.UtcNow;

					subscription.StartDate = startDate;
					subscription.EndDate = startDate.AddDays(subscription.SubscriptionPlan.DurationInDays);
					subscription.UpdatedAt = DateTime.UtcNow; // <-- set UpdatedAt

					_subscriptionRepository.Update(subscription);
					await _unitOfWork.SaveChangesAsync();

					paymentSucceeded = true;
				}
				else
				{
					subscription = await _subscriptionRepository.GetByConditionAsync(
						s => s.Id == payment.SubscriptionId,
						asNoTracking: false
					);

					if (subscription == null)
					{
						return BaseResponse<string>.Fail("Subscription not found for payment", ErrorType.NotFound);
					}

					subscription.Status = SubscriptionStatus.Failed;
					subscription.UpdatedAt = DateTime.UtcNow; // <-- set UpdatedAt

					_subscriptionRepository.Update(subscription);
					await _unitOfWork.SaveChangesAsync();
				}

				await _unitOfWork.CommitAsync(transaction);

				// --- Send notification email if payment succeeded and we have user email ---
				if (paymentSucceeded && subscription != null)
				{
					try
					{
						var userEmail = subscription.User?.Email;
						if (!string.IsNullOrWhiteSpace(userEmail))
						{
							// Prefer a template if available. There is no explicit "activation" template,
							// reuse upcoming-expiry template to communicate activation + expiry date.
							var fullName = subscription.User?.UserName ?? userEmail;
							var planName = subscription.SubscriptionPlan?.Name ?? "your plan";
							var endDate = subscription.EndDate;

							var subject = $"Your subscription \"{planName}\" is now active";
							var htmlBody = _emailTemplateService.SubscriptionActivatedTemplate(fullName, planName, endDate);

							await _emailService.SendEmailAsync(userEmail, subject, htmlBody);
						}
						else
						{
							_logger.LogWarning("Subscription {SubscriptionId} activated but user email is missing", subscription.Id);
						}
					}
					catch (Exception ex)
					{
						// Log errors while sending email but do not fail the whole callback processing.
						_logger.LogError(ex, "Failed to send subscription activation email for subscription {SubscriptionId}", subscription.Id);
					}
				}

				var message = payment.Status == TransactionStatus.Success
					? "VNPay payment successful"
					: $"VNPay payment failed (code: {vnResponse.VnPayResponseCode})";

				return BaseResponse<string>.Ok(message);
			}
			catch (Exception ex)
			{
				await _unitOfWork.RollbackAsync(transaction);
				_logger.LogError(ex, "VNPay callback processing failed");
				return BaseResponse<string>.Fail("VNPay callback processing failed", ErrorType.ServerError);
			}
		}
	}
}
