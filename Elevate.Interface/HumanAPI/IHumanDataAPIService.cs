using Elevate.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevate.Interface.HumanAPI
{
    public interface IHumanDataAPIService
    {
        Task<IReadOnlyList<HeartRateReading>> GetVitals(string accessToken, string vitalName);
        Task ResyncData(string accessToken, string human_id);
    }
}
