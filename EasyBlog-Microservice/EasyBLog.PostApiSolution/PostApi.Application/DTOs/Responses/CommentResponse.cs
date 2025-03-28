using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostApi.Application.DTOs.Responses
{
    public record CommentResponse
    {
        public required Guid CommentId { get; init; }
        public required string PostId { get; init; }
        public required AuthorResponse Author { get; init; }
        public required string Content { get; init; }
        public string? ParentId { get; init; }
        public required DateTime DateCreate { get; init; }
        public List<CommentResponse> Replies { get; init; } = [];
    }
}
