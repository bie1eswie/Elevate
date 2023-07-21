using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevate.Model.HumanAPI
{
    public class ClientTokenRequest
    {
        public string client_id { get; set; } = string.Empty;
        public string client_secret { get; set; } = string.Empty;
        public string type { get; set; } = "client";

    }
}
