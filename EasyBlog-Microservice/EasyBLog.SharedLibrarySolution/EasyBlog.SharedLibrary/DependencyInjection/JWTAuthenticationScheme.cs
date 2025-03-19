using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace EasyBlog.SharedLibrary.DependencyInjection
{
    public static class JWTAuthenticationScheme
    {
         public static IServiceCollection AddJWTAuthenticationScheme(this IServiceCollection services, IConfiguration config)
         {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer("Bearer", options =>
               {
                   var key = Encoding.UTF8.GetBytes(config.GetSection("Authentication:Key").Value!);
                   string issuer = config.GetSection("Authentication:Issuer").Value!;
                   string audience = config.GetSection("Authentication:audience").Value!;


                   options.RequireHttpsMetadata = false;
                   options.SaveToken = false;
                   options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = issuer,
                       ValidAudience = audience,
                       IssuerSigningKey = new SymmetricSecurityKey(key)
                   };

                   });
            return services;
         }
    }
}
