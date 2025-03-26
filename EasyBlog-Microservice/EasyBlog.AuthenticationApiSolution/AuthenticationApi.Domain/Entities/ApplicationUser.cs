using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthenticationApi.Domain.Enums;

namespace AuthenticationApi.Domain.Entities
{
    public class ApplicationUser
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Email { get; set; }
        public required string FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Avatar { get; set; }
        public required string PasswordHash { get; set; }
        public UserRole Role { get; set; } = UserRole.User;
        public DateTime DateCreate { get; set; } = DateTime.UtcNow;
        public DateTime? DateChange { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
