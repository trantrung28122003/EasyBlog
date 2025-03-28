using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommentApi.Application.DTOs.Resquest
{
    public record PostIdsRequest
    {
        public List<string> PostIds { get; set; } = new();
    }
}
