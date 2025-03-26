using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApi.Application.DTOs.Reponses
{
    public record UserResponse
    {
        public required string Id { get; init; }
        public required string FullName { get; init; }
        public required string Email { get; init; }
        public string? PhoneNumber { get; init; }
        public string? Address { get; init; }
        public string? Avatar { get; init; }
        public required string Role { get; init; }
    }
}
