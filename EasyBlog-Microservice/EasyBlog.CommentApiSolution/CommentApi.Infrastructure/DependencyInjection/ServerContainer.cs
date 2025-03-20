
using CommentApi.Application.Interfaces;
using CommentApi.Infrastructure.Data;
using CommentApi.Infrastructure.Repositories;
using EasyBlog.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CommentApi.Infrastructure.DependencyInjection
{
    public static class ServerContainer
    {
        public static IServiceCollection addInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            var logFileName = config["MySerilog:Filename"];
            if (string.IsNullOrEmpty(logFileName))
            {
                throw new Exception("Missing Serilog filename configuration.");
            }
            //Add database connectivity nè!
            //Add authentication scheme
            SharedServiceContainer.AddShareServices<CommentDbContext>(services, config, logFileName);

            services.AddScoped<ICommentRepository, CommentRepository>();
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
