using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace studeehub.Application.DTOs.Requests.Auth
{
	public class ForgotPasswordRequest
	{
        public string? Email { get; set; }
        public string? ClientUri { get; set; } //https://localhost:7114/api/auth/reset-password
    }
}
