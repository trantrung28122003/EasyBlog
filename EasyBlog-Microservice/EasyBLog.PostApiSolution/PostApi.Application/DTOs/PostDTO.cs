using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostApi.Application.DTOs
{
    public record PostDTO
    {
        public required Guid Id { get; init; }
        public required string Title { get; init; }
        public string? Content { get; init; }
        public required string AuthorId { get; init; }
        public List<string>? ImageUrls { get; init; } = new();
    }


}
