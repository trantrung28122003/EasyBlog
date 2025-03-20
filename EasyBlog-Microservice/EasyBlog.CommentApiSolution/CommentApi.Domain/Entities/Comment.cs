
namespace CommentApi.Domain.Entities
{
    public class Comment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string PostId { get; set; }
        public string AuthorId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? ParentId { get; set; } 
        public DateTime DateCreate { get; set; } = DateTime.UtcNow;
        public DateTime? DateChange { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
