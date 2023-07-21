using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevate.Model.HumanAPI
{
    public class PublicTokenResponse
    {
        public string humanId { get; set; } = string.Empty;
        public string publicToken { get; set; } = string.Empty;
    }
}
