
using CommentApi.Application.DTOs.Responses;
using CommentApi.Application.DTOs.Resquest;
using CommentApi.Domain.Entities;
namespace CommentApi.Application.DTOs.Conversions
{
    public static class CommentConversion
    {
       
        public static CommentResponse? FormEntityToCommentReponse(Comment? comment, AuthorResponse author)
        {
            if (comment is null)
                return null;

            return ConvertToCommentResponse(comment, author);
        }


        private static CommentResponse ConvertToCommentResponse(Comment comment, AuthorResponse author) => new()
        {
            CommentId = comment.Id.ToString(),
            PostId = comment.PostId,
            Author = author,
            Content = comment.Content,
            ParentId = comment.ParentId,
            DateCreate = comment.DateCreate,
        };
    }
}
