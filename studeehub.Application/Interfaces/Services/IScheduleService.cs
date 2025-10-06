using studeehub.Application.DTOs.Requests.Schedule;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Domain.Entities;

namespace studeehub.Application.Interfaces.Services
{
	public interface IScheduleService
	{
		public Task<BaseResponse<string>> CreateScheduleAsync(CreateScheduleRequest request);
		public Task<BaseResponse<string>> UpdateScheduleAsync(Guid id, UpdateScheduleRequest request);
		public Task UpdateAsync(Schedule schedule);
		public Task<BaseResponse<string>> DeleteScheduleAsync(Guid id);
		public Task<BaseResponse<string>> CheckIn(Guid id);
		public Task<IEnumerable<Schedule>> GetCheckinSchedulesToRemindAsync(DateTime now);
		public Task<IEnumerable<Schedule>> GetUpcomingSchedulesToRemindAsync(DateTime now);
	}
}
