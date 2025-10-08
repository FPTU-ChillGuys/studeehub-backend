using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using studeehub.Application.DTOs.Requests.PaymentTransaction;
using studeehub.Application.DTOs.Requests.VnPay;
using studeehub.Application.DTOs.Responses.Base;
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
		private readonly ILogger<PayTransactionService> _logger;
		private readonly IUnitOfWork _unitOfWork;

		public PayTransactionService(
			IGenericRepository<PaymentTransaction> paymentTransactionRepository,
			IGenericRepository<Subscription> subscriptionRepository,
			IGenericRepository<SubscriptionPlan> subscriptionPlanRepository,
			IVnPayService vnPayService,
			ILogger<PayTransactionService> logger,
			IUnitOfWork unitOfWork)
		{
			_paymentTransactionRepository = paymentTransactionRepository;
			_subscriptionRepository = subscriptionRepository;
			_subscriptionPlanRepository = subscriptionPlanRepository;
			_vnPayService = vnPayService;
			_logger = logger;
			_unitOfWork = unitOfWork;
		}

		public async Task<BaseResponse<string>> CreatePaymentSessionAsync(CreatePaymentSessionRequest request, HttpContext httpContext)
		{
			// 1️⃣ Validate plan
			var plan = await _subscriptionPlanRepository.GetByIdAsync(p => p.Id == request.SubscriptionPlanId);
			if (plan == null)
				return BaseResponse<string>.Fail("Subscription plan not found", ErrorType.NotFound);

			// 2️⃣ Prevent duplicate active subscription
			var existing = await _subscriptionRepository.GetByIdAsync(
				s => s.UserId == request.UserId && s.Status == SubscriptionStatus.Active);
			if (existing != null)
				return BaseResponse<string>.Fail("User already has an active subscription", ErrorType.Validation);

			// 3️⃣ Start transaction (atomic operation) using UnitOfWork
			using var transaction = await _unitOfWork.BeginTransactionAsync();
			try
			{
				// 4️⃣ Create subscription
				var subscription = new Subscription
				{
					Id = Guid.NewGuid(),
					UserId = request.UserId,
					SubscriptionPlanId = plan.Id,
					StartDate = DateTime.UtcNow,
					EndDate = DateTime.UtcNow,
					Status = SubscriptionStatus.Pending
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
            var payment = await _paymentTransactionRepository.GetByIdAsync(p => p.Id == vnResponse.TransactionId);
            if (payment == null)
                return BaseResponse<string>.Fail("Payment transaction not found", ErrorType.NotFound);

            // Avoid double processing
            if (payment.Status == TransactionStatus.Success)
                return BaseResponse<string>.Ok("Payment already processed");

            // STEP 3: Start transaction
            using var transaction = await _unitOfWork.BeginTransactionAsync();

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
                    var subscription = await _subscriptionRepository.GetByIdAsync(
                        s => s.Id == payment.SubscriptionId,
                        q => q.Include(s => s.SubscriptionPlan),
                        asNoTracking: false
                    ) ?? throw new Exception("Subscription not found for payment");

					subscription.Status = SubscriptionStatus.Active;

                    // Extend subscription duration properly
                    var startDate = subscription.EndDate > DateTime.UtcNow
                        ? subscription.EndDate
                        : DateTime.UtcNow;

                    subscription.StartDate = startDate;
                    subscription.EndDate = startDate.AddDays(subscription.SubscriptionPlan.DurationInDays);

                    _subscriptionRepository.Update(subscription);
                    await _unitOfWork.SaveChangesAsync();
				}
				else
				{
                    var subscription = await _subscriptionRepository.GetByIdAsync(
                        s => s.Id == payment.SubscriptionId,
                        asNoTracking: false
                    ) ?? throw new Exception("Subscription not found for payment");

					subscription.Status = SubscriptionStatus.Failed;

                    _subscriptionRepository.Update(subscription);
                    await _unitOfWork.SaveChangesAsync();
                }

				await _unitOfWork.CommitAsync(transaction);

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
