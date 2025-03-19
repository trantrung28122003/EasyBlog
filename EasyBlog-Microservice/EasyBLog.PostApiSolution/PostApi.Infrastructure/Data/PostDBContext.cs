using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PostApi.Domain.Entities;

namespace PostApi.Infrastructure.Data
{
    public class PostDBContext(DbContextOptions<PostDBContext> options) : DbContext(options)
    {
        public DbSet<Post> Posts { get; set; } 
    }
}
