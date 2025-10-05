using studeehub.Application.DTOs.Requests.Achievement;
using studeehub.Application.DTOs.Responses.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace studeehub.Application.Interfaces.Services
{
	public interface IAchievementService
	{
		public Task<BaseResponse<string>> CreateAchievementAsync(CreateAchievemRequest request);
		public Task<BaseResponse<string>> UpdateAchievementAsync(Guid id, UpdateAchievemRequest request);
    }
}
