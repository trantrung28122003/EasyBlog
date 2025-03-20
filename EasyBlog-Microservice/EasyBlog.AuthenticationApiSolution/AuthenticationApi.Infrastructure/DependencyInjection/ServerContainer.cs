using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Application.Services;
using AuthenticationApi.Infrastructure.Data;
using AuthenticationApi.Infrastructure.Respositories;
using EasyBlog.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationApi.Infrastructure.DependencyInjection
{
    public static class ServerContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            var logFileName = config["MySerilog:Filename"];
            if (string.IsNullOrEmpty(logFileName))
            {
                throw new Exception("Missing Serilog filename configuration.");
            }
            //Add database connectivity nè!
            //Add authentication scheme
            SharedServiceContainer.AddShareServices<AuthenticationDbContext>(services, config, logFileName);

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserService, UserService>();
            return services;

        }

        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {
            //Register middleware nè Trung tương lai
            //listen api GateWaty
            SharedServiceContainer.UseSharedPolicies(app);
            return app;
        }
    }
}
