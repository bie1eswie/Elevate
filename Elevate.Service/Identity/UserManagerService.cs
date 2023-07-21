using Elevate.Interface.Identity;
using Elevate.Model.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Elevate.Interface.HumanAPI;

namespace Elevate.Service.Identity
{
    public class UserManagerService : IUserManagerService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IHumanAPIRepository _humanAPIRepository;
        public UserManagerService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IHumanAPIRepository humanAPIRepository)
        {
             _signInManager = signInManager;
            _userManager = userManager;
             _humanAPIRepository = humanAPIRepository;
        }

        public  async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email) != null;
        }

        public async Task<SignInResult> CheckPasswordSignIn(AppUser user, string password)
        {
            return await _signInManager.CheckPasswordSignInAsync(user, password, false);
        }

        public async Task<IdentityResult> CreateUser(AppUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
               await this._humanAPIRepository.AddHumanAPIUser(new Model.HumanAPI.HumanAPIUser() { UserId = user.Id });
            }
            return result;
        }

        public async Task<AppUser> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<AppUser> FindByEmailFromClaimsPrinciple(ClaimsPrincipal user)
        {
            var email = user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
            return await _userManager.Users.SingleOrDefaultAsync(x => x.Email == email);
        }
    }
}
