using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApi.Application.DTOs
{
    public record ApplicationUserDTO
    {
        public Guid Id { get; init; }
        public string Email { get; init; }
        public string FullName { get; init; }
        public string? PhoneNumber { get; init; }
        public string Address { get; init; }

        public string Password { get; init; }
        public string Role { get; init; }
        public DateTime DateCreate { get; init; }
        public DateTime? DateChange { get; init; }
        public bool IsDeleted { get; init; }
    }
}
