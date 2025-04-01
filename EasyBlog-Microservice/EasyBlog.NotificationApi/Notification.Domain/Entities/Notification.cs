using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotificationApi.Domain.Enums;

namespace NotificationApi.Domain.Entities
{
    public class Notification
    {
        public required Guid Id { get; set; }
        public required string UserId { get; set; }
        public required string Message { get; set; }
        public bool IsRead { get; set; }

        public NotificationType Type { get; set; }
        public DateTime DateCreate { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
    }
}
