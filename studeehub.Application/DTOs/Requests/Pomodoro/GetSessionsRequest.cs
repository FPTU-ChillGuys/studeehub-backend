using studeehub.Application.DTOs.Requests.Base;
using studeehub.Domain.Enums.Pomodoros;

namespace studeehub.Application.DTOs.Requests.Pomodoro
{
	public class GetSessionsRequest : PagedAndSortedRequest
	{
		public PomodoroType? Type { get; set; } // Optional filter
		public PomodoroStatus? Status { get; set; } // Optional filter

		public DateTime? Start { get; set; } // Filter sessions that started after this
		public DateTime? End { get; set; }   // Filter sessions that ended before this
	}
}
