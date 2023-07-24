using Elevate.Model.HumanAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevate.Interface.HumanAPI
{
    public interface IHumanAPIRepository
    {
        Task<HumanAPIUser> GetHumanAPIUser(string userId);
        Task<HumanAPIUser> AddHumanAPIUser(HumanAPIUser humanAPIUser);
        Task<HumanAPIUser> UpdateHumanAPIUser(HumanAPIUser humanAPIUser);
        Task<HumanAPIUser> GetHumanAPIUserByHumanID(string humanID);
    }
}
