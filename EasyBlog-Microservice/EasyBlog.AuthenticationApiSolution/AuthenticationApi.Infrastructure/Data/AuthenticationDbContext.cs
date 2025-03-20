using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthenticationApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationApi.Infrastructure.Data
{
    public class AuthenticationDbContext : DbContext
    {
        public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) : base(options)
        {
        }
        public AuthenticationDbContext() { }

        public DbSet<ApplicationUser>  Users { get; set; }  
    }
}
