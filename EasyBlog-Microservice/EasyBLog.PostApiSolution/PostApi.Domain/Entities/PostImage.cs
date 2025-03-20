using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostApi.Domain.Entities
{
    public class PostImage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ImageUrl { get; set; }
        public Guid PostId { get; set; }
        public Post? Post { get; set; }
    }
}
