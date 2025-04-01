

using ApiGateway.Presentation.Middleware;
using EasyBlog.SharedLibrary.DependencyInjection;
using Microsoft.Extensions.Options;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot().AddCacheManager(x => x.WithDictionaryHandle());
JWTAuthenticationScheme.AddJWTAuthenticationScheme(builder.Services, builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") 
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); 
        });
});

var app = builder.Build();
app.UseCors("AllowFrontend");

app.UseHttpsRedirection();
app.UseMiddleware<AttachSignatureToRequest>();

app.UseWebSockets();
app.UseOcelot().Wait();
app.Run();


