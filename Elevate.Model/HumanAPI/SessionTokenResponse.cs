using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevate.Model.HumanAPI
{
    public class SessionTokenResponse
    {
        public int Expires_in { get; set; }
        public string Human_id { get; set; } = string.Empty;
        public string Session_token { get; set; } = string.Empty;
    }
}
