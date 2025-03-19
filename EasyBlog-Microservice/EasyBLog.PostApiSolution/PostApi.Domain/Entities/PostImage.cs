using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostApi.Domain.Entities
{
    public class PostImage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ImageUrl { get; set; }
        public string PostId { get; set; }
        public Post? Post { get; set; }
    }
}
