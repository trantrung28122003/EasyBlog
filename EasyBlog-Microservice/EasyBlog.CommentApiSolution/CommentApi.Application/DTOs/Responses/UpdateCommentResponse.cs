using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentApi.Application.DTOs.Responses
{
    public record UpdateCommentResponse
    {
        public required string Content { get; init; }
    }
}
