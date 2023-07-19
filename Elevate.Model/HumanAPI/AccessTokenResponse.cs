using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevate.Model.HumanAPI
{
    public class AccessTokenResponse
    {
        public int expires_in { get; set; }
        public string human_id { get; set; } = string.Empty;
        public string session_token { get; set; } = string.Empty;
    }
}
