namespace studeehub.Application.DTOs.Requests.Pomodoro
{
	public class UpdateSettingRequest
	{
		public int? WorkDuration { get; set; }
		public int? ShortBreakDuration { get; set; }
		public int? LongBreakDuration { get; set; }
		public int? LongBreakInterval { get; set; }
		public bool? AutoStartNext { get; set; }
	}
}
