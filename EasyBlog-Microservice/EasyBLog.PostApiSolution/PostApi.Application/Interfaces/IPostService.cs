using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EasyBlog.SharedLibrary.Response;
using PostApi.Application.DTOs.Responses;
using PostApi.Domain.Entities;

namespace PostApi.Application.Interfaces
{
    public interface IPostService
    {
        Task<ApiResponse<List<Post>>> GetAllAsync();
        Task<ApiResponse<List<Post>>> GetAllActiveAsync();
        Task<ApiResponse<List<PostResponse>>> GetAllPagedAsync(int offset, int limit);
        Task<ApiResponse<List<Post>>> FindByConditionAsync(Expression<Func<Post, bool>> predicate);
        Task<ApiResponse<Post?>> GetByIdAsync(string id);
        Task<ApiResponse<bool>> CreateAsync(Post post);
        Task<ApiResponse<bool>> UpdateAsync(Post post);
        Task<ApiResponse<bool>> DeleteAsync(Post post);
        Task<ApiResponse<bool>> SoftDeleteAsync(string id);
    }
}
