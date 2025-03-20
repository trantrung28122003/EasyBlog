using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentApi.Application.DTOs
{
    public record UserDTO
    {
        public required string FullName { get; init; }
        public string? Avatar { get; init; }
    }
}
