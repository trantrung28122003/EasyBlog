
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using Azure;
using EasyBlog.SharedLibrary.Logs;
using EasyBlog.SharedLibrary.Response;
using Microsoft.EntityFrameworkCore;
using PostApi.Application.Interfaces;
using PostApi.Domain.Entities;
using PostApi.Infrastructure.Data;

namespace PostApi.Infrastructure.Repositories
{
    public class PostRepository : IPost
    {
        private readonly PostDBContext _context;

        public PostRepository (PostDBContext context)
        {
            _context = context;
        }
        public async Task<ApiResponse> CreateAsync(Post post)
        {
            try
            {
                var addedPost = _context.Posts.Add(post).Entity;
                await _context.SaveChangesAsync();
                if (addedPost is null)
                {
                    return new ApiResponse(false, "Failed to add post to database");
                }
                return new ApiResponse(true, $"{post} added to database successfully");

            }
            catch (Exception ex) 
            { 
                LogException.LogExceptions(ex);
                return new ApiResponse(false, "Error occured adding new product");
            }
        }


        public async Task<ApiResponse> UpdateAsync(Post post)
        {
            try
            {
                var existingPost = await _context.Posts.FindAsync(post.Id);
                if (existingPost is null)
                {
                    return new ApiResponse(false, "Post not found");
                }

                existingPost.Title = post.Title;
                existingPost.Content = post.Content;
                existingPost.ImageUrls = post.ImageUrls;
                existingPost.DateChange = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return new ApiResponse(true, "Post updated successfully");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new ApiResponse(false, "Error occurred while updating the post");
            }
        }



        public async Task<ApiResponse> DeleteAsync(Post post)
        {
            try
            {
                var existingPost = await _context.Posts.FindAsync(post.Id);
                if (existingPost is null)
                {
                    return new ApiResponse(false, "Post not found");
                }

                existingPost.IsDeleted = true;
                existingPost.DateChange = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return new ApiResponse(true, "Post deleted successfully");
            }
            catch (Exception ex)
            {
                LogException.LogExceptions(ex);
                return new ApiResponse(false, "Error occurred while deleting the post");
            }
        }

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            return await _context.Posts
                .Where(p => !p.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Post?> GetByAsync(Expression<Func<Post, bool>> predicate)
        {
            return await _context.Posts
                .Where(p => !p.IsDeleted)
                .AsNoTracking()
                .FirstOrDefaultAsync(predicate);
        }

        public async Task<Post?> GetByIdAsync(string id)
        {
            return await _context.Posts
                .Where(p => !p.IsDeleted && p.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        
    }
}
