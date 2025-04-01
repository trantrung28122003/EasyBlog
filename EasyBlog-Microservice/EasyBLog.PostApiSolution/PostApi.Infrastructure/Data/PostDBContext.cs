using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PostApi.Domain.Entities;

namespace PostApi.Infrastructure.Data
{
    public class PostDBContext : DbContext
    {
        public PostDBContext(DbContextOptions<PostDBContext> options) : base(options)
        {
        }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostImage> PostImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Post>()
                .HasMany(p => p.Images)
                .WithOne(i => i.Post)
                .HasForeignKey(i => i.PostId)
                .OnDelete(DeleteBehavior.Cascade);


        }

    }
}
