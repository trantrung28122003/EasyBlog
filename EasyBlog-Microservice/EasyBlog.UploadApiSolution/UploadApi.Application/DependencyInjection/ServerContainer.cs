
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EasyBlog.SharedLibrary.Logs;
using UploadApi.Application.Interfaces;
using UploadApi.Application.Services;

namespace UploadApi.Application.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IFileMetadataService, FileMetadataService>();
            services.AddScoped<IUploadFileService, UploadFileService>();

            return services;
        }
    }
}