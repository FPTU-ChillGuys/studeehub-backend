using Hangfire.Dashboard;

namespace studeehub.API.Extensions
{
	public class DashboardAuthorizationFilter : IDashboardAuthorizationFilter
	{
		private readonly string _role;

		public DashboardAuthorizationFilter(string role)
		{
			_role = role;
		}

		//public bool Authorize(DashboardContext context)
		//{
		//    var httpContext = context.GetHttpContext();

		//    return httpContext.User.Identity?.IsAuthenticated == true &&
		//           httpContext.User.IsInRole(_role);
		//}

		public bool Authorize(DashboardContext dashboardContext)
		{
			return true;
		}
	}
}
