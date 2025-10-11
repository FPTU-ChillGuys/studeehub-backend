using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Net.payOS;
using Net.payOS.Types;
using studeehub.Application.DTOs.Requests.PayOS;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Application.Interfaces.Services.ThirdPartyServices;
using studeehub.Application.Services;
using studeehub.Domain.Enums;
using System.Security.Cryptography;
using System.Text;

namespace studeehub.Infrastructure.Services
{
	public class PayOSService : IPayOSService
	{
		private readonly ILogger<PayTransactionService> _logger;
		private readonly PayOS _payOS;
		private readonly IValidator<CreatePaymentLinkRequest> _validator;
		private readonly IConfiguration _configuration;


		public PayOSService(
			ILogger<PayTransactionService> logger,
			PayOS payOS,
			IValidator<CreatePaymentLinkRequest> validator,
			IConfiguration configuration)
		{
			_logger = logger;
			_payOS = payOS;
			_validator = validator;
			_configuration = configuration;
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
					_logger.LogWarning("No payment information found for Payment ID: {PaymentId}", orderCode);
					return BaseResponse<PaymentLinkInformation>.Fail("Payment information not found.", ErrorType.NotFound);
				}
				return BaseResponse<PaymentLinkInformation>.Ok(data, "Payment information retrieved.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving payment information for Payment ID: {PaymentId}", orderCode);
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

				if (request.Items == null || request.Items.Count == 0)
				{
					return BaseResponse<CreatePaymentResult>.Fail("At least one item is required to create a payment link", ErrorType.Validation);
				}

				long orderCode = DateTimeOffset.Now.ToUnixTimeMilliseconds();
				List<ItemData> items = request.Items;
				int amount = items.Sum(i => i.price * i.quantity);
				var desciption = string.IsNullOrWhiteSpace(request.Description)
					? "Thanh toan don hang"
					: request.Description;

				// Build signature payload sorted alphabetically by key:
				// amount=$amount&cancelUrl=$cancelUrl&description=$description&orderCode=$orderCode&returnUrl=$returnUrl
				var payload = $"amount={amount}&cancelUrl={request.CancelUrl}&description={desciption}&orderCode={orderCode}&returnUrl={request.ReturnUrl}";

				// Compute HMAC-SHA256 using checksum key from configuration and produce lowercase hex string
				var checksumKey = _configuration["PayOS:ChecksumKey"] ?? throw new Exception("Missing PayOS ChecksumKey!!");
				var keyBytes = Encoding.UTF8.GetBytes(checksumKey);
				var payloadBytes = Encoding.UTF8.GetBytes(payload);
				using var hmac = new HMACSHA256(keyBytes);
				var hash = hmac.ComputeHash(payloadBytes);
				var signature = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

				var paymentData = new PaymentData(
					orderCode,
					amount,
					desciption,
					request.Items,
					request.CancelUrl,
					request.ReturnUrl,
					signature,
					request.BuyerName,
					request.BuyerEmail,
					request.BuyerPhone,
					request.BuyerAddress,
					request.ExpiredAt);

				var createPayment = await _payOS.createPaymentLink(paymentData);

				if (createPayment == null)
					return BaseResponse<CreatePaymentResult>.Fail("Failed to create payment link", ErrorType.ServerError);

				return BaseResponse<CreatePaymentResult>.Ok(createPayment, "Payment link created");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating payment link");
				var errors = new List<string> { ex.Message };
				return BaseResponse<CreatePaymentResult>.Fail("An error occurred while creating the payment link.", ErrorType.ServerError, errors);
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
					_logger.LogWarning("No payment information found for Payment ID: {PaymentId}", orderCode);
					return BaseResponse<PaymentLinkInformation>.Fail("Payment information not found.", ErrorType.NotFound);
				}
				return BaseResponse<PaymentLinkInformation>.Ok(data, "Payment link cancelled.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error cancelling payment link for Payment ID: {PaymentId}", orderCode);
				var errors = new List<string> { ex.Message };
				return BaseResponse<PaymentLinkInformation>.Fail("An error occurred while cancelling the payment link.", ErrorType.ServerError, errors);
			}
		}

		public async Task<BaseResponse<string>> ConfirmWebHook(ConfirmWebhookRequest request)
		{
			try
			{
				// confirmWebhook returns a URL or token depending on SDK; treat as string
				var confirmed = await _payOS.confirmWebhook(request.WebhookUrl);
				if (string.IsNullOrWhiteSpace(confirmed))
				{
					_logger.LogWarning("PayOS confirmWebhook returned empty value");
					return BaseResponse<string>.Fail("Failed to confirm webhook", ErrorType.ServerError);
				}

				return BaseResponse<string>.Ok(confirmed, "Webhook confirmed.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error confirming webhook");
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
					_logger.LogWarning("PayOS webhook verification returned null");
					return BaseResponse<WebhookData>.Fail("Invalid webhook payload", ErrorType.Validation);
				}

				// Return verified webhook data to caller for further processing by controller/service caller
				return BaseResponse<WebhookData>.Ok(data, "Webhook data verified");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error handling PayOS webhook");
				var errors = new List<string> { ex.Message };
				return BaseResponse<WebhookData>.Fail("Failed to process webhook", ErrorType.ServerError, errors);
			}
		}
	}
}
