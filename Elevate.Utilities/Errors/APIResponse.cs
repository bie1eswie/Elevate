using System;

namespace Elevate.Utilities.Errors
{
    public class APIResponse
    {
        public APIResponse(string message)
        {
            Message = message;
        }
        public string Message { get; set; }
    }
}