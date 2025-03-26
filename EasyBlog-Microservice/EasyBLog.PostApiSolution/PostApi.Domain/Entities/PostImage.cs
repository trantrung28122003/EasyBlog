using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostApi.Domain.Entities
{
    public class PostImage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string FileMeatadataId { get; set; }
        public required Guid PostId { get; set; }

        [ForeignKey("PostId")]
        public Post? Post { get; set; }
    }
}
