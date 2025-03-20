using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CommentApi.Application.DTOs;
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
        Task<ApiResponse<CommentDTO?>> GetByIdAsync(string id);
        Task<ApiResponse<bool>> CreateAsync(CommentDTO commentDTO);
        //Task<ApiResponse<bool>> UpdateAsync(Comment comment);
        //Task<ApiResponse<bool>> DeleteAsync(Comment comment);
        //Task<ApiResponse<bool>> SoftDeleteAsync(string id);
        Task<ApiResponse<List<CommentDTO>>> GetCommentsByPostIdAsync(string postId);
    }
}
