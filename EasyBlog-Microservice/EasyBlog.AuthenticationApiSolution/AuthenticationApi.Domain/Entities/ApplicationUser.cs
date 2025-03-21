﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApi.Domain.Entities
{
    public class ApplicationUser
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; }
        public string FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string Address { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime? DateChange { get; set; }
        public bool IsDeleted { get; set; }
    }
}
