using Elevate.Model.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Elevate.Interface.Identity
{
    public interface IUserManagerService
    {
        Task<AppUser> FindByEmailFromClaimsPrinciple(ClaimsPrincipal user);
        Task<bool> CheckEmailExistsAsync(string email);
        Task<AppUser> FindByEmailAsync(string email);
        Task<SignInResult> CheckPasswordSignIn(AppUser user, string password);
        Task<IdentityResult> CreateUser(AppUser user, string password);
    }
}
