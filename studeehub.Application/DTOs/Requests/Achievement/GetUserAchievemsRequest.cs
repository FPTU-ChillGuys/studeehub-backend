namespace studeehub.Application.DTOs.Requests.Achievement
{
	public class GetUserAchievemsRequest : GetAchievemsRequest
	{
		public Guid UserId { get; set; }
	}
}
