using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommentApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommentApi.Infrastructure.Data
{
    public class CommentDbContext : DbContext
    {
        public CommentDbContext(DbContextOptions<CommentDbContext> options) : base(options) 
        { }

        public CommentDbContext() { }

        public DbSet<Comment> Comments { get; set; } 
    }
}
