using studeehub.Domain.Enums;

namespace studeehub.Application.DTOs.Responses.Base
{
	public class PagedResponse<T> : BaseResponse<List<T>>
	{
		public int TotalCount { get; set; }
		public int Page { get; set; }
		public int PageSize { get; set; }

		public static PagedResponse<T> Ok(List<T> data, int total, int page, int pageSize, string message = "Success")
		{
			return new PagedResponse<T>
			{
				Success = true,
				Message = message,
				Data = data,
				TotalCount = total,
				Page = page,
				PageSize = pageSize
			};
		}

		public static PagedResponse<T> Fail(
			string message,
			ErrorType errorType = ErrorType.ServerError,
			List<string>? errors = null,
			int page = 1,
			int pageSize = 10)
		{
			return new PagedResponse<T>
			{
				Success = false,
				Message = message,
				Errors = errors,
				ErrorType = errorType,
				Data = new List<T>(),
				TotalCount = 0,
				Page = page,
				PageSize = pageSize
			};
		}
	}
}
