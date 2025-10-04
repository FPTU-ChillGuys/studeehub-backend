using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace studeehub.Application.Interfaces.Services
{
	public interface IUserService
	{
		public Task<bool> IsUserExistAsync(Guid userId);
    }
}
