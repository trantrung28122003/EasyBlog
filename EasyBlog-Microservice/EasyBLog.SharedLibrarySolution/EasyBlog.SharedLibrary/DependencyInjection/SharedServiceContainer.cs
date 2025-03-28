

using EasyBlog.SharedLibrary.HttpClients;
using EasyBlog.SharedLibrary.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace EasyBlog.SharedLibrary.DependencyInjection
{
    public static class SharedServiceContainer
    {
        public static IServiceCollection AddShareServices<TContext>(this IServiceCollection services, 
            IConfiguration config, string filename) where TContext :DbContext
        {
            services.AddDbContext<TContext>(option => 
                option.UseSqlServer(
                    config.GetConnectionString("eBlogConnection"), 
                    sqlserverOption => sqlserverOption
                        .EnableRetryOnFailure()
                        .MigrationsHistoryTable("__EFMigrationsHistory_" + typeof(TContext).Name)
                    ));

            Log.Logger = new LoggerConfiguration()
             .MinimumLevel.Information()
             .WriteTo.Debug()
             .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
             .WriteTo.File(
                 path: Path.Combine(AppContext.BaseDirectory, "logs", "log-.txt"),
                 restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
                 outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                 rollingInterval: RollingInterval.Day)
                .CreateLogger();

            JWTAuthenticationScheme.AddJWTAuthenticationScheme(services, config);

            services.AddHttpContextAccessor();  // Để lấy token từ request

            // Đăng ký AuthHttpMessageHandler
            services.AddTransient<AuthHttpMessageHandler>();
            return services;
        }

        public static IApplicationBuilder  UseSharedPolicies (this IApplicationBuilder app)
        {
            app.UseMiddleware<GlobalException>();

            app.UseMiddleware<ListenToOnlyApiGateway>();

            return app;
        }
    }
}
