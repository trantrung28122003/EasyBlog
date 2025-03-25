using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CommentApi.Application.DTOs;
using CommentApi.Application.DTOs.Conversions;
using CommentApi.Application.Interfaces;
using CommentApi.Domain.Entities;
using EasyBlog.SharedLibrary.Response;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Registry;

namespace CommentApi.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly HttpClient _httpClient;
        private readonly ResiliencePipelineProvider<string> _resiliencePipeline;
        private readonly ICommentRepository _commentRepository;

        public CommentService(HttpClient httpClient, ResiliencePipelineProvider<string> resiliencePipeline, ICommentRepository commentRepository)
        {
            _httpClient = httpClient;
            _resiliencePipeline = resiliencePipeline;
            _commentRepository = commentRepository;
        }


        public async Task<PostDTO> GetPostAsync(string postId)
        {
            var getPost = await _httpClient.GetAsync($"/api/posts/{postId}");
            if (!getPost.IsSuccessStatusCode)
            {
                return null!;
            }
            var post = await getPost.Content.ReadFromJsonAsync<PostDTO>();
            return post!;


        }

        public async Task<UserDTO> GetUserSafeAsync(string userId)
        {
            var getUser = await _httpClient.GetAsync($"/api/authentication/{userId}");
            if (!getUser.IsSuccessStatusCode)
            {
                return null!;
            }
            var user = await getUser.Content.ReadFromJsonAsync<UserDTO>();
            return user!;

        }

        public async Task<ApiResponse<List<CommentDTO>>> GetCommentsByPostIdAsync(string postId)
        {
            try
            {
                var comments = await _commentRepository.FindByConditionAsync(c => c.PostId.ToString() == postId);
                if (!comments.Any())
                    return new ApiResponse<List<CommentDTO>>(false, "Không có bình luận nào");

                var retryPipeline = _resiliencePipeline.GetPipeline("my-retry-pipeline");

                // Lấy thông tin bài viết để tìm chủ bài viết
                //var postDTO = await retryPipeline.ExecuteAsync(async _ => await GetPostAsync(postId));
                //if (postDTO == null)
                //    return new ApiResponse<List<CommentDTO>>(false, "Không tìm thấy bài viết");

                // Lấy danh sách userId từ comments để tránh gọi API quá nhiều lần
                var userIds = comments.Select(c => c.AuthorId).Distinct().ToList();
                var userResponses = await Task.WhenAll(userIds.Select(async userId =>
                    new { UserId = userId, User = await retryPipeline.ExecuteAsync(async _ => await GetUserSafeAsync(userId)) }
                ));

                // Chuyển danh sách kết quả thành Dictionary
                var userDictionary = userResponses.ToDictionary(u => u.UserId, u => u.User ?? new UserDTO { FullName = "Người dùng không tồn tại", Avatar = "" });

                // Chuyển đổi danh sách comment sang DTO bằng `CommentConversion`
                var commentDTOs = comments
                    .Select(comment => CommentConversion.FormEntity(comment, userDictionary[comment.AuthorId]))
                    .Where(dto => dto is not null)
                    .ToList();

                return new ApiResponse<List<CommentDTO>>(true, "Lấy danh sách bình luận thành công", commentDTOs);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<CommentDTO>>(false, "Lỗi khi lấy bình luận", null, new List<string> { ex.Message });
            }
        }



        public async Task<ApiResponse<bool>> CreateAsync(CommentDTO commentDTO)
        {
            try
            {
                var retryPipeline = _resiliencePipeline.GetPipeline("my-retry-pipeline");
                var postDTO = await retryPipeline.ExecuteAsync(async _ => await GetPostAsync(commentDTO.PostId));
                if (postDTO == null)
                {
                    return new ApiResponse<bool>(false, "Bài viết không tồn tại", false);
                }
                var comment = CommentConversion.ToEntity(commentDTO, "123213");
                await _commentRepository.CreateAsync(comment);
                return new ApiResponse<bool>(true, "Thêm bình luận vào bài viết thành công", true);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(false, "Lỗi khi thêm bình luận vào bài viết", false, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<CommentDTO?>> GetByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out Guid guidId))
            {
                return new ApiResponse<CommentDTO?>(false, "ID không hợp lệ");
            }
            //test
            var userDTO = new UserDTO
            {
                FullName = "Trung Tương Lai",
                Avatar = "https://example.com/avatar.jpg" // Có thể để null nếu không có
            };

            var result = await _commentRepository.GetByIdAsync(guidId);

            var commentDTO = CommentConversion.FormEntity(result, userDTO);
            if (result == null)
                return new ApiResponse<CommentDTO?>(false, "Không tìm thấy bình luận đó");

            return new ApiResponse<CommentDTO?>(true, "Lấy bình luận thành công", commentDTO);
        }
    }
}
