namespace studeehub.Application.DTOs.Requests.Workspace
{
	public class CreateWorkspaceRequest
	{
		public Guid OwnerId { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
	}
}
