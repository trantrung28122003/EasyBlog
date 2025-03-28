using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentApi.Application.DTOs.Resquest
{
    public record CreateCommentRequest
    {
        public required string PostId { get; init; }
        public required string Content { get; init; }
        public string? ParentId { get; init; }
    }
}
