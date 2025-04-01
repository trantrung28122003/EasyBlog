using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommentApi.Application.DTOs.Conversions;
using CommentApi.Application.DTOs.Responses;
using CommentApi.Application.DTOs.Resquest;
using CommentApi.Application.Hubs;
using CommentApi.Application.Interfaces;
using CommentApi.Domain.Entities;
using EasyBlog.SharedLibrary.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Registry;

namespace CommentApi.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ResiliencePipelineProvider<string> _resiliencePipeline;
        private readonly ICommentRepository _commentRepository;
        private readonly IHubContext<CommentHub> _hubContext;

        public CommentService(HttpClient httpClient, ResiliencePipelineProvider<string> resiliencePipeline, 
            ICommentRepository commentRepository, IHttpContextAccessor httpClientAccessor, IHubContext<CommentHub> hubContext

            )
        {
            _httpClient = httpClient;
            _resiliencePipeline = resiliencePipeline;
            _commentRepository = commentRepository;
            _httpContextAccessor = httpClientAccessor;
            _hubContext = hubContext;
        }

        public async Task<ApiResponse<CommentResponse?>> GetByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out Guid guidId))
            {
                return new ApiResponse<CommentResponse?>(false, "ID không hợp lệ");
            }

            var retryPipeline = _resiliencePipeline.GetPipeline("my-retry-pipeline");
            var authorResponse = await retryPipeline.ExecuteAsync(async _ => await GetCurrentUserAsync());

            if (authorResponse is null)
            {
                return new ApiResponse<CommentResponse?>(false, "Không tìm thấy thông tin người dùng đang đăng nhập", null);
            }
            var commentById = await _commentRepository.GetByIdAsync(guidId);

            if (commentById == null)
                return new ApiResponse<CommentResponse?>(false, "Không tìm thấy bình luận đó");
            var commentReponse = CommentConversion.FormEntityToCommentReponse(commentById, authorResponse);
            return new ApiResponse<CommentResponse?>(true, "Lấy bình luận thành công", commentReponse);
        }

        public async Task<ApiResponse<List<CommentResponse>>> GetCommentsByPostIdAsync(string postId)
        {
            return await GetCommentsAsync(new List<string> { postId });
        }

        public async Task<ApiResponse<List<CommentResponse>>> GetCommentsByPostIdsAsync(PostIdsRequest request)
        {
            return await GetCommentsAsync(request.PostIds);
        }
        

        public async Task<ApiResponse<CommentResponse>> CreateAsync(CreateCommentRequest request)
        {
            try
            {
                var retryPipeline = _resiliencePipeline.GetPipeline("my-retry-pipeline");
                var authorResponse = await retryPipeline.ExecuteAsync(async _ => await GetCurrentUserAsync());

                if (authorResponse is null)
                {
                    return new ApiResponse<CommentResponse>(false, "Không tìm thấy thông tin người dùng đang đăng nhập", null);
                }

                string? parentId = string.IsNullOrWhiteSpace(request.ParentId) ? null : request.ParentId;
                var comment = new Comment
                {
                    Content = request.Content,
                    PostId = request.PostId,
                    AuthorId = authorResponse.Id,
                    ParentId = parentId
                };
                await _commentRepository.CreateAsync(comment);

                var commentResponse = CommentConversion.FormEntityToCommentReponse(comment, authorResponse);
                await _hubContext.Clients.Group(request.PostId).SendAsync("ReceiveComment", commentResponse);


                var postResponse = await retryPipeline.ExecuteAsync(async _ => await GetPostByIdAsync(request.PostId));
                if (postResponse is not null && postResponse.Author.Id != authorResponse.Id)
                {
                    
                    var notificationRequest = new CreateNotificationRequest
                    {
                        UserIds = new List<string> { postResponse.Author.Id },
                        TypeNotification = "NewComment",
                        Message = $"{authorResponse.FullName} đã bình luận bài viết của bạn: {comment.Content}"
                    };


                    bool isNotificationSent = await retryPipeline.ExecuteAsync(async _ => await SendNotificationAsync(notificationRequest));
                    if (!isNotificationSent)
                    {
                        return new ApiResponse<CommentResponse>(true, "Thêm bình luận thành công nhưng không thể gửi thông báo", commentResponse);
                    }
                }

                return new ApiResponse<CommentResponse>(true, "Thêm bình luận vào bài viết thành công", commentResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<CommentResponse>(false, "Lỗi khi thêm bình luận vào bài viết", null, new List<string> { ex.Message });
            }
        }



        public async Task<ApiResponse<UpdateCommentResponse>> UpdateAsync(UpdateCommentRequest request, string id)
        {
            try
            {
                if(!Guid.TryParse(id, out Guid guidCommentId))
                {
                    return new ApiResponse<UpdateCommentResponse>(false, "ID không hợp lệ", null);
                }
                var extingComment = await _commentRepository.GetByIdAsync(guidCommentId);

                var existingComment = await _commentRepository.GetByIdAsync(guidCommentId);
                if (existingComment == null)
                {
                    return new ApiResponse<UpdateCommentResponse>(false, "Không tìm thấy bình luận", null);
                }
                var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return new ApiResponse<UpdateCommentResponse>(false, "Không tìm thấy thông tin người dùng đang đăng nhập", null);
                }

                if (existingComment.AuthorId != userId)
                {
                    return new ApiResponse<UpdateCommentResponse>(false, "Bạn không có quyền chỉnh sửa bình luận này", null);
                }

                existingComment.Content = request.Content;
                existingComment.DateChange = DateTime.UtcNow;
                await _commentRepository.UpdateAsync(existingComment);

                var updateCommmentResponse = new UpdateCommentResponse()
                {
                    Content = existingComment.Content,
                };
                return new ApiResponse<UpdateCommentResponse>(true, "Sửa bình luận của bạn thành công", updateCommmentResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<UpdateCommentResponse>(false, "Lỗi khi thêm bình luận vào bài viết", null, new List<string> { ex.Message });
            }
        }
        public async Task<ApiResponse<string>> SoftDeleteAsync(string commentId)
        {
            if (!Guid.TryParse(commentId, out Guid guidCommentId))
            {
                return new ApiResponse<string>(false, "ID không hợp lệ", null);
            }

            var comment = await _commentRepository.GetByIdAsync(guidCommentId);
            if (comment == null)
            {
                return new ApiResponse<string>(false, "Không tìm thấy bình luận", null);
            }

            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return new ApiResponse<string>(false, "Không tìm thấy thông tin người dùng đang đăng nhập", null);
            }

            if (comment.AuthorId != userId)
            {
                return new ApiResponse<string>(false, "Bạn không có quyền xóa bình luận này", null);
            }

            await _commentRepository.SoftDeleteAsync(guidCommentId);
            return new ApiResponse<string>(true, "Xóa mềm bình luận thành công", commentId);
        }


        public async Task<ApiResponse<string>> DeleteAsync(string commentId)
        {
            if (!Guid.TryParse(commentId, out Guid guidCommentId))
            {
                return new ApiResponse<string>(false, "ID không hợp lệ", null);
            }

            var comment = await _commentRepository.GetByIdAsync(guidCommentId);
            if (comment == null)
            {
                return new ApiResponse<string>(false, "Không tìm thấy bình luận", null);
            }

            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return new ApiResponse<string>(false, "Không tìm thấy thông tin người dùng đang đăng nhập", null);
            }

            if (comment.AuthorId != userId)
            {
                return new ApiResponse<string>(false, "Bạn không có quyền xóa bình luận này", null);
            }

            await _commentRepository.DeleteAsync(guidCommentId);
            return new ApiResponse<string>(true, "Xóa vĩnh viễn bình luận thành công", commentId);
        }


        private async Task<ApiResponse<List<CommentResponse>>> GetCommentsAsync(List<string> postIds)
        {
            try
            {
                var comments = await _commentRepository.FindByConditionAsync(c => postIds.Contains(c.PostId.ToString()));
                if (!comments.Any())
                    return new ApiResponse<List<CommentResponse>>(false, "Không có bình luận nào", []);
                var userIds = comments.Select(c => c.AuthorId).Distinct().ToList();
                var retryPipeline = _resiliencePipeline.GetPipeline("my-retry-pipeline");
                var authorsResponse = await retryPipeline.ExecuteAsync(async _ => await GetUsersByIdsAsync(userIds));

                var authorsDictionary = authorsResponse?.ToDictionary(p => p.Id, p => p) ?? new Dictionary<string, AuthorResponse>();

                var commentDictionary = comments.Select(comment =>
                {
                    var author = (authorsDictionary != null && authorsDictionary.ContainsKey(comment.AuthorId))
                        ? authorsDictionary[comment.AuthorId]
                        : new AuthorResponse { Id = "Không tồn tại", FullName = "Người dùng không tồn tại", Avatar = "" };
                    return CommentConversion.FormEntityToCommentReponse(comment, author);
                }).ToList();


                var commentDict = comments.ToDictionary(
                    comment => comment.Id,
                    comment =>
                    {
                        var author = authorsDictionary.TryGetValue(comment.AuthorId, out var authorInfo)
                            ? authorInfo
                            : new AuthorResponse { Id = "Không tồn tại", FullName = "Người dùng không tồn tại", Avatar = "" };

                        return CommentConversion.FormEntityToCommentReponse(comment, author);
                    });

                foreach (var comment in commentDict.Values)
                {
                    if (!string.IsNullOrEmpty(comment?.ParentId) && commentDict.TryGetValue(Guid.Parse(comment.ParentId), out var parentComment))
                    {
                        parentComment?.Replies.Add(comment);
                    }
                }
                var rootComments = commentDict.Values.Where(p => string.IsNullOrEmpty(p?.ParentId)).ToList();
                return new ApiResponse<List<CommentResponse>>(true, "Lấy danh sách bình luận theo postId thành công", rootComments!);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<CommentResponse>>(false, "Lỗi khi lấy bình luận", null, new List<string> { ex.Message });
            }
        }

        private async Task<AuthorResponse?> GetCurrentUserAsync()
        {
       
            var response = await _httpClient.GetAsync($"/api/users/profile");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<AuthorResponse>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return apiResponse?.Results;
        }

        private async Task<PostResponse?> GetPostByIdAsync(string id)
        {

            var response = await _httpClient.GetAsync($"/api/posts/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<PostResponse>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return apiResponse?.Results;

        }

        private async Task<List<AuthorResponse>?> GetUsersByIdsAsync(List<string> userIds)
        {
            var userIdRequest = new UserIdsRequest() { UserIds = userIds };  

            var response = await _httpClient.PostAsJsonAsync("/api/users/getUsersByIds", userIdRequest);

            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = await response.Content.ReadAsStringAsync();
            var authorsResponse = JsonSerializer.Deserialize<ApiResponse<List<AuthorResponse>>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return authorsResponse?.Results;
        }

        private async Task<bool> SendNotificationAsync(CreateNotificationRequest request)
        {
            try
            {
                
                var response = await _httpClient.PostAsJsonAsync("/api/notifications/create", request);

           
                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error sending notification: {errorResponse}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return false;
            }
        }

    }
}
