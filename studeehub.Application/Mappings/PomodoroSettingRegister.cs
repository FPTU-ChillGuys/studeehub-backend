using Mapster;
using studeehub.Application.DTOs.Requests.Pomodoro;
using studeehub.Application.DTOs.Responses.PomodoroSetting;
using studeehub.Domain.Entities;

namespace studeehub.Application.Mappings
{
	public class PomodoroSettingRegister : IRegister
	{
		public void Register(TypeAdapterConfig config)
		{
			config.NewConfig<PomodoroSetting, GetSettingResponse>()
				.Map(dest => dest.UserId, src => src.UserId)
				.Map(dest => dest.WorkDuration, src => src.WorkDuration)
				.Map(dest => dest.ShortBreakDuration, src => src.ShortBreakDuration)
				.Map(dest => dest.LongBreakDuration, src => src.LongBreakDuration)
				.Map(dest => dest.LongBreakInterval, src => src.LongBreakInterval)
				.Map(dest => dest.AutoStartNext, src => src.AutoStartNext);

			config.NewConfig<UpdateSettingRequest, PomodoroSetting>()
				.Map(dest => dest.WorkDuration, src => src.WorkDuration)
				.Map(dest => dest.ShortBreakDuration, src => src.ShortBreakDuration)
				.Map(dest => dest.LongBreakDuration, src => src.LongBreakDuration)
				.Map(dest => dest.LongBreakInterval, src => src.LongBreakInterval)
				.Map(dest => dest.AutoStartNext, src => src.AutoStartNext);
		}
	}
}
