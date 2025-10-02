using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace studeehub.Application.DTOs.Requests.Auth
{
    public class RefreshTokenRequest
    {
        public required Guid UserId { get; set; }
        public required string RefreshToken { get; set; }
    }
}
