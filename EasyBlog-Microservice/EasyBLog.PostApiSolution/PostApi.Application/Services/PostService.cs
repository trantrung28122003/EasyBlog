
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EasyBlog.SharedLibrary.Response;
using PostApi.Application.DTOs.Conversions;
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

        public PostService(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<ApiResponse<List<PostResponse>>> GetAllPagedAsync(int offset, int limit)
        {
            var posts = await _postRepository.GetAllPagedAsync(offset, limit);
            var 
            var postResponses = posts.Select(post => PostConversion.ToResponse(post)).ToList();
            return new ApiResponse<List<PostResponse>>(true, "Lấy danh sách bài viết thành công", postResponses);
        }


        public async Task<ApiResponse<List<Post>>> GetAllAsync()
        {
            var result = await _postRepository.GetAllAsync();
            return new ApiResponse<List<Post>>(true, "Lấy danh sách bài viết thành công", result);
        }

        public async Task<ApiResponse<List<Post>>> GetAllActiveAsync()
        {
            var result = await _postRepository.GetAllActiveAsync();
            return new ApiResponse<List<Post>>(true, "Lấy danh sách bài viết chưa xóa thành công", result);
        }

        public async Task<ApiResponse<List<Post>>> FindByConditionAsync(Expression<Func<Post, bool>> predicate)
        {
            var result = await _postRepository.FindByConditionAsync(predicate);
            return new ApiResponse<List<Post>>(true, "Lọc bài viết thành công", result);
        }

        public async Task<ApiResponse<Post?>> GetByIdAsync(string id)
        {
            if (!Guid.TryParse(id, out Guid guidId))
            {
                return new ApiResponse<Post?>(false, "ID không hợp lệ");
            }
            var result = await _postRepository.GetByIdAsync(guidId);
            if (result == null)
                return new ApiResponse<Post?>(false, "Không tìm thấy bài viết");

            return new ApiResponse<Post?>(true, "Lấy bài viết thành công", result);
        }

        public async Task<ApiResponse<bool>> CreateAsync(Post post)
        {
            try
            {
                await _postRepository.CreateAsync(post);

                return new ApiResponse<bool>(true, "Thêm bài viết thành công", true);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(false, "Lỗi khi thêm bài viết", false, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> UpdateAsync(Post post)
        {
            try
            {
                var existingPost = await _postRepository.GetByIdAsync(post.Id);
                if (existingPost == null)
                    return new ApiResponse<bool>(false, "Không tìm thấy bài viết", false);

                await _postRepository.UpdateAsync(post);
                return new ApiResponse<bool>(true, "Cập nhật bài viết thành công", true);
            }
            catch (Exception ex)
            {
                return new ApiResponse<bool>(false, "Lỗi khi cập nhật bài viết", false, new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Post post)
        {
            try
            {
                await _postRepository.DeleteAsync(post);
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
    }

}
