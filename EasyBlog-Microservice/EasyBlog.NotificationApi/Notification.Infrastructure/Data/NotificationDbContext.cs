using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NotificationApi.Domain.Entities;

namespace NotificationApi.Infrastructure.Data
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) 
        { }

        public NotificationDbContext() { }

        public DbSet<Notification> Notifications { get; set; } 
    }
}
