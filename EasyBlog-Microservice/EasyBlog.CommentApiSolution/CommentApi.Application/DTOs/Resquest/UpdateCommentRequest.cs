using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommentApi.Application.DTOs.Responses;

namespace CommentApi.Application.DTOs.Resquest
{
    public class UpdateCommentRequest
    {
        public required string Content { get; init; }
    }
}
