using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostApi.Application.DTOs.Responses
{
    public record PostResponse
    {
        public required string Id { get; init; }
        public string? Title { get; init; }
        public string? Content { get; init; }
        public required AuthorResponse Author { get; init; }
        public List<string>? ImageUrls { get; init; } = new();
        public List<CommentResponse> CommentsResponse { get; init; } = [];
        public int LikeCount { get; init; } = 0;
        public int CommentCount { get; init; } = 0;
    }
}
