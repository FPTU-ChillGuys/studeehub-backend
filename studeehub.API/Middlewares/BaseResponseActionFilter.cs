using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using studeehub.Application.DTOs.Responses.Base;
using studeehub.Domain.Enums;

namespace studeehub.API.Middlewares
{
	public class BaseResponseActionFilter : IAsyncResultFilter
	{
		public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
		{
			if (context.Result is ObjectResult objectResult &&
				objectResult.Value is BaseResponse baseResponse)
			{
				// Map error type to proper status code
				if (!baseResponse.Success)
				{
					objectResult.StatusCode = baseResponse.ErrorType switch
					{
						ErrorType.Validation => StatusCodes.Status400BadRequest,
						ErrorType.NotFound => StatusCodes.Status404NotFound,
						ErrorType.Conflict => StatusCodes.Status409Conflict,
						ErrorType.ServerError => StatusCodes.Status500InternalServerError,
						ErrorType.Forbidden => StatusCodes.Status403Forbidden,
						ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
						_ => StatusCodes.Status400BadRequest
					};
				}
				else
				{
					objectResult.StatusCode = StatusCodes.Status200OK;
				}
			}

			await next();
		}
	}

}
