using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using PostApi.Application.DTOs.Responses;
using PostApi.Domain.Entities;

namespace PostApi.Application.DTOs.Conversions
{
    public static class PostConversion
    {
        public static Post ToEntity(PostDTO postDTO, string userId) => new()
        {

            Id = Guid.NewGuid(),
            Title = postDTO.Title,
            Content = postDTO.Content,
            AuthorId = userId,
            DateCreate = DateTime.UtcNow,
            IsDeleted = false,
            Images = postDTO.ImageUrls?.Select(url => new PostImage
            {
                FileMetadataId = "123",
                PostId = Guid.NewGuid()
            }).ToList() ?? new List<PostImage>()
        };


        public static PostResponse FromEntityToPostRespone(
            Post post, 
            AuthorResponse authorResponse,
            List<string> imageUrls,
            List<CommentResponse> commentsResponse,
            int likeCount = 0, 
            int commentCount = 0)
        {
            return new PostResponse
            {
                Id = post.Id.ToString(),
                Title = post.Title,
                Content = post.Content,
                Author = authorResponse,
                CommentsResponse = commentsResponse,
                ImageUrls = imageUrls,
                LikeCount = likeCount,
                CommentCount = commentCount
            };
        }

        private static PostDTO ConvertToPostDTO(Post post) => new()
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            AuthorId = post.AuthorId,
            ImageUrls = post.Images?.Select(img => img.FileMetadataId).ToList() ?? new()
        };


    }
}
