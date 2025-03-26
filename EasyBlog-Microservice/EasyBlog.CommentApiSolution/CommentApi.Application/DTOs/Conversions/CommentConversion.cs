
using CommentApi.Domain.Entities;
namespace CommentApi.Application.DTOs.Conversions
{
    public static class CommentConversion
    {
        public static Comment ToEntity(CommentDTO commentDTO, string userId) => new()
        {
            Id = Guid.NewGuid(),
            PostId = commentDTO.PostId,
            AuthorId = userId,
            Content = commentDTO.Content,
            ParentId = commentDTO.ParentId,
            DateCreate = DateTime.UtcNow,
            IsDeleted = false
        };


        public static CommentDTO? FormEntity(Comment? comment, UserDTO getUser)
        {
            if (comment is null)
                return null;

            return ConvertToCommentDTO(comment, getUser);
        }


        private static CommentDTO ConvertToCommentDTO(Comment comment, UserDTO user) => new()
        {
            CommentId = comment.Id,
            PostId = comment.PostId,
            Author = user,
            Content = comment.Content,
            ParentId = comment.ParentId,
            DateCreate = comment.DateCreate,
            DateChange = comment.DateChange,
        };
    }
}
