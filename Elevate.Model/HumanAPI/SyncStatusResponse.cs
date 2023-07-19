using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevate.Model.HumanAPI
{
    public class SyncStatusResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string SourceName { get; set; } = string.Empty;
    }
}
