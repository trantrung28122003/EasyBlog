using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UploadApi.Domain.Entites;
using UploadApi.Domain.Enums;

namespace UploadApi.Infrastructure.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<UploadDbContext>();
            
            Guid fixedId = Guid.Parse("4278876e-1458-4f05-9941-f9dda054f640");
            if (!await dbContext.FileMetadatas.AnyAsync(p => p.Id == fixedId))
            {
                var fileMetadata = new FileMetadata
                {
                    Id = fixedId,
                    FileName = "eBlog_avatarDefault_20250326084901_35b7f610.jpg",
                    FileSize = 1024,
                    FileType = FileType.Image,
                    FileUrl = "https://res.cloudinary.com/dofr3xzmi/image/upload/v1742978943/eBlog/eBlog_avatarDefault_20250326084901_35b7f610.jpg.jpg",
                    DateCreate = DateTime.UtcNow,
                    ChangeBy = "System"
                };
                
                dbContext.FileMetadatas.Add(fileMetadata);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
