namespace Elevate.Utilities.Errors
{
    public class APIException : Exception
    {
        public APIException(int statusCode, string? details=null)
        {
            Details = details;
            StatusCode = statusCode;
        }
        public int StatusCode { get; set; }
        public string Details {get; set;}
        private string GetDefaultMessage(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad Request",
                401 => "Unauthorized",
                404 => "Not Found",
                500 => "Internal Server Error",
                _ => null
            };
        }
    }
}