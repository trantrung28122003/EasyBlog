using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using UploadApi.Domain.Entites;

namespace UploadApi.Infrastructure.Data
{
    public class UploadDbContext : DbContext
    {
        public UploadDbContext(DbContextOptions<UploadDbContext> options) : base(options)
        {
        }
        public UploadDbContext() { }
        public DbSet<FileMetadata> FileMetadatas { get; set; }
    }
}
