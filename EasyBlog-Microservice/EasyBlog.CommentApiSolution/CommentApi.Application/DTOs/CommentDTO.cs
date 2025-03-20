using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentApi.Application.DTOs
{
    public record CommentDTO
    {
        public required Guid CommentId { get; init; }
        public required string PostId { get; init; }
        public required UserDTO Author { get; init; }
        public required string Content { get; init; }
        public string? ParentId { get; init; }
        public required DateTime DateCreate { get; init; }
        public DateTime? DateChange { get; init; }
    }

}
