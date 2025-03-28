
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using EasyBlog.SharedLibrary.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.Extensions.Hosting;
using Polly.Registry;
using PostApi.Application.DTOs.Conversions;
using PostApi.Application.DTOs.Reponses;
using PostApi.Application.DTOs.Requests;
using PostApi.Application.DTOs.Responses;
using PostApi.Application.Interfaces;
using PostApi.Domain.Entities;
namespace PostApi.Application.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly HttpClient _httpClient;
        private readonly ResiliencePipelineProvider<string> _resiliencePipeline;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PostService(IPostRepository postRepository, HttpClient httpClient, 
            ResiliencePipelineProvider<string> resiliencePipeline, IHttpContextAccessor httpContextAccessor
            )
        {
            _postRepository = postRepository;
            _httpClient = httpClient;
            _resiliencePipeline = resiliencePipeline;
            _httpContextAccessor = httpContextAccessor;

        }

        public async Task<ApiResponse<List<PostResponse>>> GetPostsByPageAsync(int offset, int limit)
        {
            var posts = await _postRepository.GetPostsByPageAsync(offset, limit);
            if (posts == null || !posts.Any())
            {
                return new ApiResponse<List<PostResponse>>(true, "Không có bài viết nào", new List<PostResponse>());
            }

            var postsResponse = await ConvertToPostsResponseAsync(posts);
            return new ApiResponse<List<PostResponse>>(true, "Lấy danh sách bài viết thành công", postsResponse);
        }


        public async Task<ApiResponse<List<PostResponse>>> GetAllAsync()
        {

            var posts = await _postRepository.GetAllActiveAsync();
            if (posts == null || !posts.Any())
            {
                return new ApiResponse<List<PostResponse>>(true, "Không có bài viết nào", new List<PostResponse>());
            }

            var postsResponse = await ConvertToPostsResponseAsync(posts);
            return new ApiResponse<List<PostResponse>>(true, "Lấy danh sách bài viết thành công", postsResponse);
        }


        public async Task<ApiResponse<List<Post>>> FindByConditionAsync(Expression<Func<Post, bool>> predicate)
        {
            var result = await _postRepository.FindByConditionAsync(predicate);
            return new ApiResponse<List<Post>>(true, "Lọc bài viết thành công", result);
        }

        public async Task<ApiResponse<PostResponse?>> GetByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out Guid guidId))
            {
                return new ApiResponse<PostResponse?>(false, "ID không hợp lệ");
            }

            var post = await _postRepository.GetByIdAsync(guidId);
            if (post == null)
                return new ApiResponse<PostResponse?>(false, "Không tìm thấy bài viết");

            var postResponse = (await ConvertToPostsResponseAsync(new List<Post> { post })).FirstOrDefault();
            

            return new ApiResponse<PostResponse?>(true, "Lấy bài viết thành công", postResponse);
        }


        public async Task<ApiResponse<PostResponse>> CreateAsync(CreatePostRequest request, List<IFormFile> files)
        {
            try
            {
                if (files == null || files.Count == 0)
                {
                    return new ApiResponse<PostResponse>(false, "Bài viết phải có ít nhất một ảnh", null);
                }
                var retryPipeline = _resiliencePipeline.GetPipeline("my-retry-pipeline");
                var authorResponse = await retryPipeline.ExecuteAsync(async _ => await GetCurrentUserAsync());

                if (authorResponse == null)
                {
                    return new ApiResponse<PostResponse>(false, "Không tìm thấy thông tin người dùng", null);
                }

                var imageResponses =  await UploadMultipleFiles(files);

                if (imageResponses == null || imageResponses.Count == 0)
                {
                    return new ApiResponse<PostResponse>(false, "Không thể tải ảnh lên", null);
                }

                var post = new Post
                {
                    Content = request.Content,
                    AuthorId = authorResponse.Id,
                    DateCreate = DateTime.UtcNow,
                    IsDeleted = false,
                   
                };

                post.Images = imageResponses.Select(img => new PostImage
                {
                    FileMetadataId = img.Id,
             
                }).ToList();

                await _postRepository.CreateAsync(post);

                var postResponse = PostConversion.FromEntityToPostRespone(
                    post,
                    authorResponse,
                    imageResponses.Select(img => img.FileUrl).ToList(),
                    new List<CommentResponse>(),
                    likeCount: 0,
                    commentCount: 0
                    );

                return new ApiResponse<PostResponse>(true, "Thêm bài viết thành công", postResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<PostResponse>(false, "Lỗi khi thêm bài viết", null, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<PostResponse>> UpdateAsync(UpdatePostRequest request, string id)
        {
            try
            {
                if(!Guid.TryParse(id, out Guid postId))
                {
                    return new ApiResponse<PostResponse>(false, "ID không hợp lệ", null);
                }
                var existingPost = await _postRepository.GetByIdAsync(postId);
                if (existingPost == null)
                    return new ApiResponse<PostResponse>(false, "Không tìm thấy bài viết", null);

                existingPost.Content = request.NewContent;
                existingPost.DateChange = DateTime.UtcNow;

                await _postRepository.UpdateAsync(existingPost);

                var postResponse = (await ConvertToPostsResponseAsync(new List<Post> { existingPost })).FirstOrDefault();

                return new ApiResponse<PostResponse>(true, "Cập nhật nội dung bài viết thành công", postResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<PostResponse>(false, "Lỗi khi cập nhật bài viết", null, new List<string> { ex.Message });
            }
        }


        public async Task<ApiResponse<bool>> DeleteAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out Guid guidPostId))
                {
                    return new ApiResponse<bool>(false, "ID không hợp lệ");
                }
                await _postRepository.DeleteAsync(guidPostId);
                return new ApiResponse<bool>(true, "Xóa bài viết thành công", true);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(false, "Lỗi khi xóa bài viết", false, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> SoftDeleteAsync(string id)
        {
            try
            {
                if (!Guid.TryParse(id, out Guid guidId))
                {
                    return new ApiResponse<bool>(false, "ID không hợp lệ");
                }
                await _postRepository.SoftDeleteAsync(guidId);
                return new ApiResponse<bool>(true, "Xóa mềm bài viết thành công", true);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(false, "Lỗi khi xóa mềm bài viết", false, new List<string> { ex.Message });
            }
        }


        private async Task<List<CommentResponse>?> GetCommentsByPostIds(PostIdsRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync($"/api/comments/commentsByPostIds", request);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var reponseContent = await response.Content.ReadAsStringAsync();
            var commentsResponse = JsonSerializer.Deserialize<ApiResponse<List<CommentResponse>>>(reponseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return commentsResponse?.Results;
        }
        private async Task<List<AuthorResponse>?> GetUsersByIdsAsync(List<string> userIds)
        {
            var userIdRequest = new UserIdsRequest() { UserIds = userIds };
            var response = await _httpClient.PostAsJsonAsync("/api/users/getUsersByIds", userIds);

            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = await response.Content.ReadAsStringAsync();
            var authorsResponse = JsonSerializer.Deserialize<ApiResponse<List<AuthorResponse>>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return authorsResponse?.Results;
        }

        private async Task<AuthorResponse?> GetCurrentUserAsync()
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
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

        public async Task<List<FileMetadataReponse>?> UploadMultipleFiles(List<IFormFile> files)
        {
            using var content = new MultipartFormDataContent();

            foreach (var file in files)
            {
                var stream = file.OpenReadStream(); // Không dùng await using
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                content.Add(fileContent, "files", file.FileName);
            }

            var response = await _httpClient.PostAsync("/api/filemetadata/upload-multiple", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<List<FileMetadataReponse>>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return apiResponse?.Results;
        }


        private async Task<List<FileMetadataReponse>?> GetFilesMetadataByPosts(List<Post> posts)
        {
            var fileIds = posts.SelectMany(p => p.Images)
                              .Select(img => img.FileMetadataId)
                              .Distinct()
                              .ToList();

            var request = new FileMetadataIdsRequest { FileIds = fileIds };

            var response = await _httpClient.PostAsJsonAsync("/api/filemetadata/getFilesMetadataByIds", request);

            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = await response.Content.ReadAsStringAsync();
            var fileMetadataReponse = JsonSerializer.Deserialize<ApiResponse<List<FileMetadataReponse>>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return fileMetadataReponse?.Results;
        }

        private async Task<List<PostResponse>> ConvertToPostsResponseAsync(List<Post> posts)
        {
            var postIds = posts.Select(post => post.Id.ToString()).ToList();
            var auhorIds = posts.Select(post => post.AuthorId).Distinct().ToList();
            if (postIds.Count == 0)
            {
                return new List<PostResponse>();
            }
            var portIdsRequest = new PostIdsRequest { PostIds = postIds };
            var retryPipeline = _resiliencePipeline.GetPipeline("my-retry-pipeline");

            var commentsResponse = await retryPipeline.ExecuteAsync(async _ => await GetCommentsByPostIds(portIdsRequest));
            var authorsRespone = await retryPipeline.ExecuteAsync(async _ => await GetUsersByIdsAsync(auhorIds));
            var fileMetadatas = await retryPipeline.ExecuteAsync(async _ => await GetFilesMetadataByPosts(posts));

            var authorsDictionary = authorsRespone?.ToDictionary(p => p.Id, p => p)
                ?? new Dictionary<string, AuthorResponse>();

            var commentsDictionary = commentsResponse?.GroupBy(p => p.PostId).ToDictionary(p => p.Key, p => p.ToList())
                 ?? new Dictionary<string, List<CommentResponse>>();

            var fileMetadatasDictionary = fileMetadatas?.ToDictionary(file => file.Id, file => file.FileUrl)
                ?? new Dictionary<string, string>();
           

            var postsResponse = posts.Select(post =>
            {
                authorsDictionary.TryGetValue(post.AuthorId, out var author);

                var imageUrls = post.Images
                     .Select(img => fileMetadatasDictionary.TryGetValue(img.FileMetadataId, out var fileUrl) ? fileUrl : "URL_MẶC_ĐỊNH")
                     .ToList();


            
                commentsDictionary.TryGetValue(post.Id.ToString(), out var comments);
             

                return PostConversion.FromEntityToPostRespone(
                    post,
                    author ?? new AuthorResponse { Id = post.AuthorId, FullName = "ẩn danh", Avatar = "URL" },
                    imageUrls,
                    comments ?? new List<CommentResponse>(),
                    likeCount: 0,
                    commentCount: comments?.Count ?? 0
                    );
            }).ToList();

            return postsResponse;
        }
    }
}
