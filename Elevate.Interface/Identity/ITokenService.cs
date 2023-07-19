using Elevate.Model.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevate.Interface.Identity
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
