using SignalRGateway.Infrastructure.DependencyInjection;
using SignalRGateway.Presentation.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Thêm CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policyBuilder =>
    {
        policyBuilder
            .AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(_ => true)  // Duy trì dòng này
            .AllowCredentials();
    });
});

// Đăng ký SignalR
builder.Services.AddSignalR();

// Đăng ký MassTransit (và các cấu hình khác)
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Sử dụng CORS (phải được gọi trước UseRouting)
app.UseCors("CorsPolicy");

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<CommentHub>("/commentHub");
});


app.Run();
