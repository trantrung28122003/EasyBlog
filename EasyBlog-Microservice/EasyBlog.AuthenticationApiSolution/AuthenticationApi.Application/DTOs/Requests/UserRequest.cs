using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApi.Application.DTOs.Requests
{
    public record UserRequest
    {
        public required string Email { get; init; }
        public required string FullName { get; init; }
        public string? PhoneNumber { get; init; }
        public string? Address { get; init; }
        public required string Password { get; init; }
        public string? Avatar { get; set; }
    }
}
