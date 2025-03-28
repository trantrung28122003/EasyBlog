using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostApi.Application.DTOs.Requests
{
    public class UserIdsRequest
    {
        public List<string> UserIds { get; set; } = new();
    }
}
