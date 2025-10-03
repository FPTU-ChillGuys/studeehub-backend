namespace studeehub.Domain.Enums
{
	public enum ErrorType
	{
		None = 0,
		Validation = 400,
		Unauthorized = 401,
		Forbidden = 403,
		NotFound = 404,
		Conflict = 409,
		ServerError = 500
	}
}
