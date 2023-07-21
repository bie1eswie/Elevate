using Elevate.Interface.HumanAPI;
using Elevate.Model.HumanAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Elevate.Data.HumanAPI
{
    public class HumanAPIRepository : IHumanAPIRepository
    {
        private readonly HumanAPIContext _humanAPIContext;
        public HumanAPIRepository(HumanAPIContext humanAPIContext)
        {
            _humanAPIContext = humanAPIContext;
        }

        public async Task<HumanAPIUser> AddHumanAPIUser(HumanAPIUser humanAPIUser)
        {
            humanAPIUser.ID = Guid.NewGuid().ToString();
            this._humanAPIContext.HumanAPIUsers.Add(humanAPIUser);
            await this._humanAPIContext.SaveChangesAsync();
            return humanAPIUser;
        }

        public async Task<HumanAPIUser> GetHumanAPIUser(string userId)
        {
            return await _humanAPIContext.HumanAPIUsers.FirstOrDefaultAsync(x=>x.UserId == userId);
        }
        public async Task<HumanAPIUser> UpdateHumanAPIUser(HumanAPIUser humanAPIUser)
        {
            _humanAPIContext.HumanAPIUsers.Update(humanAPIUser);
            await this._humanAPIContext.SaveChangesAsync();
            return humanAPIUser;
        }
    }
}
