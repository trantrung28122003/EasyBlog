﻿
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly.Retry;
using Polly;
using EasyBlog.SharedLibrary.Logs;
using EasyBlog.SharedLibrary.HttpClients;
using NotificationApi.Application.Interfaces;
using NotificationApi.Application.Services;

namespace NotificationApi.Application.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration config)
        {

            services.AddTransient<AuthHttpMessageHandler>();
            // Đăng ký dịch vụ HttpClient với Dependency Injection
            services.AddHttpClient<INotificationService, NotificationService>(options =>
            {
                options.BaseAddress = new Uri(config["ApiGateway:BaseAddress"]!);
                options.Timeout = TimeSpan.FromSeconds(10);
            }).AddHttpMessageHandler<AuthHttpMessageHandler>();

            // Tạo chiến lược retry
            var retryStrategy = new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<TaskCanceledException>(),
                BackoffType = DelayBackoffType.Constant,
                UseJitter = true,
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(500),
                OnRetry = args =>
                {
                    string message = $"OnRetry, Attempt: {args.AttemptNumber} Outcome: {args.Outcome}";
                    LogException.LogToConsole(message);
                    LogException.LogToDebugger(message);
                    return ValueTask.CompletedTask;
                }
            };

            // Đăng ký Resilience Pipeline (đường ống chịu lỗi) sử dụng chiến lược retry
            services.AddResiliencePipeline("my-retry-pipeline", builder =>
            {
                builder.AddRetry(retryStrategy);
            });
            return services;
        }
    }
}