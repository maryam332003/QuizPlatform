namespace QuizPlatform.Core.Common
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public ApiResponse(int statusCode, string? message = null, object? data = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(StatusCode);
            Data = data;
        }

        private string? GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "a bad request, you have made",
                401 => "You are not Authorized",
                404 => "Resources was  not found",
                500 => "Server Error",
                200 => "Request successful",
                201 => "Resource created successfully",
                403 => "Access denied",
                _ => null,
            };
        }
    }
}
