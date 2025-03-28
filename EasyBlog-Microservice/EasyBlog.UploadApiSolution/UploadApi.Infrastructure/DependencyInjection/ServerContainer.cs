
using UploadApi.Application.Interfaces;
using UploadApi.Infrastructure.Data;
using UploadApi.Infrastructure.Repositories;
using EasyBlog.SharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UploadApi.Infrastructure.DependencyInjection
{
    public static class ServerContainer
    {
        public static IServiceCollection addInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            var logFileName = config["MySerilog:Filename"];
            if (string.IsNullOrEmpty(logFileName))
            {
                throw new Exception("Missing Seriloag filename configuration.");
            }
            //Add database connectivity nè!
            //Add authentication scheme
            SharedServiceContainer.AddShareServices<UploadDbContext>(services, config, logFileName);

            services.AddScoped<IFileMetadataRepository, FileMetadataRepository>();
            return services;
        }

        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {
            //Register middleware nè Trung tương lai
            //listen api GateWaty
            SharedServiceContainer.UseSharedPolicies(app);
            return app;


        }

        public static async Task SeedDataAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var scopedProvider = scope.ServiceProvider;
            try
            {
                await DataSeeder.SeedAsync(scopedProvider);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Seed Data] Lỗi khi seed dữ liệu: {ex.Message}");
            }
        }
    }
}
