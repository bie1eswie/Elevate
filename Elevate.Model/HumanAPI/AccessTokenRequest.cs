using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevate.Model.HumanAPI
{
    public class AccessTokenRequest
    {
        public string HumanId { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string SessionToken { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
    }
}
