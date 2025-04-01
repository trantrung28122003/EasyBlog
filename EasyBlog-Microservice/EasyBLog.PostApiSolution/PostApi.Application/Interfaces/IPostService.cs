using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using EasyBlog.SharedLibrary.Response;
using Microsoft.AspNetCore.Http;
using PostApi.Application.DTOs.Requests;
using PostApi.Application.DTOs.Responses;
using PostApi.Domain.Entities;

namespace PostApi.Application.Interfaces
{
    public interface IPostService
    {
        Task<ApiResponse<List<PostResponse>>> GetAllAsync();
        Task<ApiResponse<List<PostResponse>>> GetPostsByPageAsync(int offset, int limit);
        Task<ApiResponse<List<AuthorResponse>?>> GetTop3AuthorsWithMostPosts();
        Task<ApiResponse<List<Post>>> FindByConditionAsync(Expression<Func<Post, bool>> predicate);
        Task<ApiResponse<PostResponse?>> GetPostByIdAsync(string id);
        Task<ApiResponse<PostResponse>> CreateAsync(CreatePostRequest request, List<IFormFile> files);
        Task<ApiResponse<PostResponse>> UpdateAsync(UpdatePostRequest request, string id);
        Task<ApiResponse<bool>> DeleteAsync(string id);
        Task<ApiResponse<bool>> SoftDeleteAsync(string id);
    }
}
