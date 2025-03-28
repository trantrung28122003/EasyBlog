using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostApi.Application.DTOs.Requests
{
    public record PostIdsRequest
    {
        public List<string> PostIds { get; set; } = new();
    }
}
