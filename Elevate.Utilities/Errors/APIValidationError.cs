using System.Collections.Generic;

namespace Elevate.Utilities.Errors
{
    public class APIValidationError : APIResponse
    {
        public APIValidationError() : base("")
        {
            Errors= new List<string>();
        }
        public IEnumerable<string> Errors { get; set;}
    }
}