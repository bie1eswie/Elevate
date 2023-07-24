using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevate.Model.HumanAPI
{
    public class AccessTokenResponse
    {
        public string ClientId { get; set; } = string.Empty;
        public string HumanId { get; set; } = string.Empty;
        public string PublicToken { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string ClientUserId { get; set; } = string.Empty;
    }
}
