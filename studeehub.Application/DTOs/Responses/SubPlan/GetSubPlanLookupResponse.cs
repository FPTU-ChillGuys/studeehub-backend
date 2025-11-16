namespace studeehub.Application.DTOs.Responses.SubPlan
{
	public class GetSubPlanLookupResponse
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Code { get; set; } = string.Empty;
	}
}
