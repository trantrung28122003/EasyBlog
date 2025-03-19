using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
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
                ImageUrl = url
            }).ToList() ?? new List<PostImage>()
        };


        public static (PostDTO?, IEnumerable<PostDTO>?) FormEntity(Post? post, IEnumerable<Post>? posts)
        {
            if (post is not null && posts is null)
                return (ConvertToPostDTO(post), null);

            if (posts is not null && post is null)
                return (null, posts.Select(ConvertToPostDTO));

            return (null, null);
        }

        private static PostDTO ConvertToPostDTO(Post post) => new()
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            AuthorId = post.AuthorId,
            ImageUrls = post.ImageUrls?.Select(img => img.ImageUrl).ToList() ?? new()
        };


    }
}
