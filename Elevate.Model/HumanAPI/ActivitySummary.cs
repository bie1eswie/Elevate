using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevate.Model.HumanAPI
{
    public class ActivitySummary
    {
        public DateTime Date { get; set; }
        public int Duration { get; set; }
        public int Distance { get; set; }
        public int Steps { get; set; }
        public int Calories { get; set; }
        public string Source { get; set; } = string.Empty;
    }
}
