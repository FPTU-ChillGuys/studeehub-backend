using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using studeehub.Application.DTOs.Requests.VnPay;
using studeehub.Application.DTOs.Responses.VnPay;
using studeehub.Application.Interfaces.Services.ThirdPartyServices;
using studeehub.Infrastructure.Extensions;
using System.Text.RegularExpressions;

namespace studeehub.Infrastructure.Services
{
	public class VnPayService : IVnPayService
	{
		private readonly IConfiguration _config;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public VnPayService(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
		{
			_config = configuration;
			_webHostEnvironment = webHostEnvironment;
		}

		public string CreatePaymentUrl(HttpContext context, VnPaymentRequest requestModel)
		{
			var returnUrl = _webHostEnvironment.IsDevelopment()
				? _config["VnPay:ReturnUrl"]
				: _config["VnPay:ReturnUrlProduct"];

			var vnpay = new VnPayLibrary();
			vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"] ?? string.Empty);
			vnpay.AddRequestData("vnp_Command", _config["VnPay:Command"] ?? string.Empty);
			vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"] ?? string.Empty);

			// ensure null-coalesce happens BEFORE multiplying by 100 (VNPay expects amount in smallest unit)
			vnpay.AddRequestData("vnp_Amount", ((requestModel.Amount ?? 0) * 100).ToString());

			// Use utc now for create date
			vnpay.AddRequestData("vnp_CreateDate", DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
			vnpay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"] ?? string.Empty);
			vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context) ?? string.Empty);
			vnpay.AddRequestData("vnp_Locale", _config["VnPay:Locale"] ?? string.Empty);

			// include subscription id and txn id in order info for later reconciliation
			var orderInfo = $"Thanh toan don hang: {requestModel.SubscriptionId} | Txn:{requestModel.PaymentTransactionId}";
			vnpay.AddRequestData("vnp_OrderInfo", orderInfo);
			vnpay.AddRequestData("vnp_OrderType", "190000"); // intertainment and education
            vnpay.AddRequestData("vnp_ReturnUrl", returnUrl ?? string.Empty);

			// Use GUID string for txn reference (matches PaymentTransaction.Id)
			vnpay.AddRequestData("vnp_TxnRef", requestModel.PaymentTransactionId.ToString());

			string paymentUrl = vnpay.CreateRequestUrl(_config["VnPay:PaymentUrl"] ?? string.Empty, _config["VnPay:HashSecret"] ?? string.Empty);
			return paymentUrl;
		}

		public VnPaymentResponse PaymentExcute(IQueryCollection collections)
		{
			var vnpay = new VnPayLibrary();
			foreach (var item in collections)
			{
				var key = item.Key;
				var value = item.Value;
				if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
				{
					vnpay.AddResponseData(key, value.ToString());
				}
			}

			// Parse vnp_TxnRef as Guid (this is the PaymentTransaction.Id)
			if (!Guid.TryParse(vnpay.GetResponseData("vnp_TxnRef"), out var txnRef))
			{
				return new VnPaymentResponse
				{
					Success = false,
					Message = "Invalid or missing vnp_TxnRef in VNPay response"
				};
			}

			// provider transaction number (may be numeric or string)
			var vnp_TranNo = vnpay.GetResponseData("vnp_TransactionNo");

			// response code and order info
			var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
			var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");

			// attempt to extract SubscriptionId from OrderInfo (we put it there during request)
			Guid subscriptionId = Guid.Empty;
			if (!string.IsNullOrWhiteSpace(vnp_OrderInfo))
			{
				// find first GUID in the OrderInfo string
				var m = Regex.Match(vnp_OrderInfo, @"\b[0-9a-fA-F]{8}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{4}\b-[0-9a-fA-F]{12}\b");
				if (m.Success)
				{
					Guid.TryParse(m.Value, out subscriptionId);
				}
			}

			// Parse amount safely and convert to actual value (divide by 100)
			decimal amount = 0m;
			var rawAmountStr = vnpay.GetResponseData("vnp_Amount");
			if (long.TryParse(rawAmountStr, out var rawAmount))
			{
				amount = rawAmount / 100m;
			}

			// Get secure hash
			var secureHashString = vnpay.GetResponseData("vnp_SecureHash");
			if (string.IsNullOrEmpty(secureHashString))
			{
				// fallback to query collection (defensive)
				secureHashString = collections.TryGetValue("vnp_SecureHash", out var sec) ? sec.ToString() : string.Empty;
			}

			if (string.IsNullOrEmpty(secureHashString))
			{
				return new VnPaymentResponse
				{
					Success = false,
					Message = "Missing vnp_SecureHash in VNPay response"
				};
			}

			var secret = _config["VnPay:HashSecret"] ?? string.Empty;
			bool checkSignature = vnpay.ValidateSignature(secureHashString, secret);

			// Consider request successful only when signature is valid and VNPay response code indicates success (usually "00")
			var success = checkSignature && string.Equals(vnp_ResponseCode, "00", StringComparison.OrdinalIgnoreCase);

			if (!checkSignature)
			{
				return new VnPaymentResponse
				{
					Success = false,
					Message = "Signature validation failed"
				};
			}

			return new VnPaymentResponse
			{
				Success = success,
				OrderDescription = vnp_OrderInfo,
				SubscriptionId = subscriptionId,
				TransactionId = txnRef,
				TransactionNo = vnp_TranNo,
				VnPayResponseCode = vnp_ResponseCode,
				Token = secureHashString,
				Amount = amount,
				Message = success ? "Payment succeeded" : $"Payment failed (code: {vnp_ResponseCode})"
			};
		}
	}
}
