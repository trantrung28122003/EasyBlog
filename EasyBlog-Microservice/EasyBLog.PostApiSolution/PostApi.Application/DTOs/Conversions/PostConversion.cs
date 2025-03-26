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
            ImageUrls = postDTO.ImageUrls?.Select(url => new PostImage
            {
                Id = Guid.NewGuid(),
                FileMeatadataId = "123",
                PostId = Guid.NewGuid()
            }).ToList() ?? new List<PostImage>()
        };


        public static PostResponse FromEntityToPostRespone(
            Post post, string authorName, 
            string authorAvatar, 
            List<string> imageUrls,
            int likeCount = 0, 
            int commentCount = 0)
        {
            return new PostResponse
            {
                Title = post.Title,
                Content = post.Content,
                Author = new AuthorResponse
                {
                    Id = post.AuthorId,
                    FullName = authorName,
                    Avatar = authorAvatar
                },
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
            ImageUrls = post.ImageUrls?.Select(img => img.FileMeatadataId).ToList() ?? new()
        };


    }
}
