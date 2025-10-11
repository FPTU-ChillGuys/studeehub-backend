using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace studeehub.Application.DTOs.Requests.PayOS
{
	public class ConfirmWebhookRequest
	{
		public string WebhookUrl { get; set; } = string.Empty;
    }
}
