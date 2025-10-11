using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace studeehub.Application.DTOs.Requests.PayOS
{
	public class CreatePaymentLinkRequest
	{
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public double Amount { get; set; }
        public string CancelUrl { get; set; } = string.Empty;
    }
}
