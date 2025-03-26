
namespace CommentApi.Domain.Entities
{
    public class Comment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string PostId { get; set; }
        public required string AuthorId { get; set; }
        public required string Content { get; set; }
        public string? ParentId { get; set; } 
        public DateTime DateCreate { get; set; } = DateTime.UtcNow;
        public DateTime? DateChange { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
