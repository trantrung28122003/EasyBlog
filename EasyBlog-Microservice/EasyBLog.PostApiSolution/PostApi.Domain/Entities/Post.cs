using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostApi.Domain.Entities
{
    public class Post
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Title { get; set; }            
        public string? Content { get; set; }         
        public required string AuthorId { get; set; }
        public List<PostImage> ImageUrls { get; set; } = new();
        public DateTime DateCreate { get; set; }      
        public DateTime? DateChange { get; set; }    
        public bool IsDeleted { get; set; }
     
    }
}
