using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationApi.Application.DTOs.Responses
{
    public record AuthorResponse
    {
        public required string Id { get; init; }
        public required string FullName { get; init; }
        public required string Avatar { get; init; }
    }
}
