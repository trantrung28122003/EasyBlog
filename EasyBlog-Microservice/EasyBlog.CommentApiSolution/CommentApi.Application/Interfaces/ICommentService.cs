using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CommentApi.Application.DTOs.Responses;
using CommentApi.Application.DTOs.Resquest;
using CommentApi.Domain.Entities;
using EasyBlog.SharedLibrary.Response;
using Microsoft.Extensions.Hosting;

namespace CommentApi.Application.Interfaces
{
    public interface ICommentService
    {
        //Task<ApiResponse<List<Comment>>> GetAllAsync();
        //Task<ApiResponse<List<Comment>>> GetAllActiveAsync();
        //Task<ApiResponse<List<Comment>>> FindByConditionAsync(Expression<Func<Comment, bool>> predicate);
        Task<ApiResponse<CommentResponse?>> GetByIdAsync(string id);
        Task<ApiResponse<CommentResponse>> CreateAsync(CreateCommentRequest request);
        Task<ApiResponse<UpdateCommentResponse>> UpdateAsync(UpdateCommentRequest request, string id);
        Task<ApiResponse<string>> DeleteAsync(string id);
        Task<ApiResponse<string>> SoftDeleteAsync(string id);
        Task<ApiResponse<List<CommentResponse>>> GetCommentsByPostIdAsync(string postId);
        Task<ApiResponse<List<CommentResponse>>> GetCommentsByPostIdsAsync(PostIdsRequest request);

    }
}
