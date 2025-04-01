
using EasyBlog.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationApi.Application.Interfaces;
using NotificationApi.Infrastructure.Data;
using NotificationApi.Infrastructure.Repositories;

namespace NotificationApi.Infrastructure.DependencyInjection
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
            SharedServiceContainer.AddShareServices<NotificationDbContext>(services, config, logFileName);

            services.AddScoped<INotificationRepository, NotificationRepository>();
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
