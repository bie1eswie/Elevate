using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevate.Model.Data
{
    public class HeartRateReading
    {
        public string Timestamp { get; set; } = string.Empty;
        public string TzOffset { get; set; } = string.Empty;
        public int Value { get; set; }
        public string Unit { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
