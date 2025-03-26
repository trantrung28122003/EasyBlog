using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApi.Application.DTOs.Requests
{
    public record UserLoginRequest
    {
        public required string Email { get; init; }
        public required string Password { get; init; }
    }
}
