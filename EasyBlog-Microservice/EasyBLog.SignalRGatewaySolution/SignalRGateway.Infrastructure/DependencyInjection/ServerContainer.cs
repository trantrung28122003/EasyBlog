using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MassTransit;
using SignalRGateway.Application.Consumers;
namespace SignalRGateway.Infrastructure.DependencyInjection
{
    public static class ServerContainer
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            string rabbitMqHost = configuration["RabbitMQ:Host"] ?? "localhost";

            services.AddMassTransit(config =>
            {
                config.AddConsumer<CommentCreatedConsumer>();

                config.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(rabbitMqHost);
                    cfg.ReceiveEndpoint("comment-created-queue", e =>
                    {
                        e.ConfigureConsumer<CommentCreatedConsumer>(context);
                    });
                });
            });

            return services;
        }
    }
}
