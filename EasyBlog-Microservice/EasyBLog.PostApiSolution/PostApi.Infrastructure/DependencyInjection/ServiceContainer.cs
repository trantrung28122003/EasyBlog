using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyBlog.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PostApi.Application.Interfaces;
using PostApi.Application.Services;
using PostApi.Infrastructure.Data;
using PostApi.Infrastructure.Repositories;

namespace PostApi.Infrastructure.DependencyInjection
{
    public static class ServiceContainer
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
            SharedServiceContainer.AddShareServices<PostDBContext>(services, config, logFileName);

            services.AddScoped<IPostRepository, PostRepository>();

            services.AddScoped<IPostService, PostService>();

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
