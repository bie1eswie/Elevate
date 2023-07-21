using Elevate.Data.HumanAPI;
using Elevate.Interface.HumanAPI;
using Elevate.Interface.Identity;
using Elevate.Service;
using Elevate.Service.HumanAPI;
using Elevate.Service.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevate.Extentions.Extentions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddServiceDependencies(this IServiceCollection services)
        {
            services.AddScoped<IUserManagerService, UserManagerService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IHumanAPIService, HumanAPIService>();
            services.AddScoped<IHumanDataAPIService, HumanDataAPIService>();
            services.AddScoped<IHumanAPIRepository, HumanAPIRepository>();
            return services;
        }
    }
}
