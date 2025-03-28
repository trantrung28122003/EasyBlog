using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApi.Application.DTOs.Request
{
    public class UserIdsRequest
    {
        public List<string> UserIds { get; set; } = [];
    }
}
