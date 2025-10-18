namespace studeehub.Application.DTOs.Responses.PomodoroSetting
{
	public class GetSettingResponse
	{
		public Guid UserId { get; set; }
		public int WorkDuration { get; set; }
		public int ShortBreakDuration { get; set; }
		public int LongBreakDuration { get; set; }
		public int LongBreakInterval { get; set; }
		public bool AutoStartNext { get; set; }
	}
}
