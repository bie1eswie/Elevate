using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevate.Interface.HumanAPI
{
    public interface IHumanAPIService
    {
        Task<string> RequestToken(string email);
    }
}
