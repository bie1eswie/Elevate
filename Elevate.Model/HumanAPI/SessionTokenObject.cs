using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevate.Model.HumanAPI
{
    public class SessionTokenObject
    {
        public string humanId { get; set; } = string.Empty;
        public string clientId { get; set; } = string.Empty;
        public string sessionToken { get; set; } = string.Empty;
        public string userId { get; set; } = string.Empty;
    }
}
