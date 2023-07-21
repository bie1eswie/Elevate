using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevate.Utilities.Helpers
{
    public static class Constants
    {
        public const string GET_ACCOUNT_ID = "/sources";
        public const string GET_ACTIVITY_SUMMARY = "/activities/summaries";
        public static Dictionary<string, string> VitalsDataEndPoints = new Dictionary<string, string>
        {
            {"heart_rate","/heart_rate/readings" },
        };
    }
}
