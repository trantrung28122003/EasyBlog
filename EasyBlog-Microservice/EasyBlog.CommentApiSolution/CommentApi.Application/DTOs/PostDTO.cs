using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentApi.Application.DTOs
{
    public record PostDTO
    {
        public required string Id { get; init; }
        public string? AuthorId { get; init; }
    }
}
