using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuthenticationApi.Infrastructure.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>();
            if (!await dbContext.Users.AnyAsync(u => u.Role == UserRole.Admin))
            {
                var adminUser = new ApplicationUser
                {
                    Email = "admin@gmail.com",
                    FullName = "Administrator",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password@1234"),
                    Role = UserRole.Admin,
                    PhoneNumber = "0999999999",
                    Address = "Admin - đường Admin, Quận Admin, TP.HCM",
                    Avatar = "https://www.gravatar.com/avatar/205e460b479e2e5b48aec07710c08d50",
                    DateCreate = DateTime.UtcNow,
                    DateChange = DateTime.UtcNow,
                    IsDeleted = false
                };
                dbContext.Users.Add(adminUser);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
