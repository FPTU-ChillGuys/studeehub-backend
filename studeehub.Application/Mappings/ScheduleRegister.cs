using Mapster;
using studeehub.Application.DTOs.Requests.Schedule;
using studeehub.Domain.Entities;

namespace studeehub.Application.Mappings
{
	public class ScheduleRegister : IRegister
	{
		public void Register(TypeAdapterConfig config)
		{
			config.NewConfig<CreateScheduleRequest, Schedule>()
				.Map(dest => dest.Id, src => Guid.NewGuid())
				.Map(dest => dest.UserId, src => src.UserId)
				.Map(dest => dest.Title, src => src.Title)
				.Map(dest => dest.IsCheckin, src => false)
				.Map(dest => dest.StartTime, src => src.StartTime)
				.Map(dest => dest.EndTime, src => src.EndTime)
				.Map(dest => dest.ReminderMinutesBefore, src => src.ReminderMinutesBefore)
				.Map(dest => dest.IsReminded, src => false)
				.Map(dest => dest.Description, src => src.Description);

			config.NewConfig<UpdateScheduleRequest, Schedule>()
				.Map(dest => dest.Title, src => src.Title)
				.Map(dest => dest.StartTime, src => src.StartTime)
				.Map(dest => dest.EndTime, src => src.EndTime)
				.Map(dest => dest.ReminderMinutesBefore, src => src.ReminderMinutesBefore)
				.Map(dest => dest.Description, src => src.Description);
		}
	}
}
