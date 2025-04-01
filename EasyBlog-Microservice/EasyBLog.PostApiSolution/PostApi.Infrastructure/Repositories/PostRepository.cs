
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
    public class PostRepository : IPostRepository
    {
        private readonly PostDBContext _context;

        public PostRepository (PostDBContext context)
        {
            _context = context;
        }
        public async Task<List<Post>> GetPostsByPageAsync(int offset, int limit)
        {
            return await _context.Posts
                .Where(p => !p.IsDeleted)
                .OrderByDescending(p => p.DateCreate)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }
        public async Task<List<Post>> GetAllAsync()
        {
            return await _context.Posts
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Post>> GetAllActiveAsync()
        {
            return await _context.Posts
                .Include(p=>p.Images)
                .Where(p => !p.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Post>> FindByConditionAsync(Expression<Func<Post, bool>> predicate)
        {
            return await _context.Posts
                .Where(predicate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Post?> GetByIdAsync(Guid id)
        {
            return await _context.Posts
                .Where(p => p.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Post post)
        {
            var existingPost = await _context.Posts.FindAsync(post.Id);
            if (existingPost is null) throw new Exception("Post not found");

            existingPost.Title = post.Title;
            existingPost.Content = post.Content;
            existingPost.Images = post.Images;
            existingPost.DateChange = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var existingPost = await _context.Posts.FindAsync(id);
            if (existingPost is null) throw new Exception("Post not found");

            _context.Posts.Remove(existingPost);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Guid id)
        {
            var existingPost = await _context.Posts.FindAsync(id);
            if (existingPost is null) throw new Exception("Post not found");

            existingPost.IsDeleted = true;
            existingPost.DateChange = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task<List<string>> GetTop3AuthorsWithMostPosts()
        {
            return await _context.Posts
                 .Where(p => !p.IsDeleted)
                 .GroupBy(p => p.AuthorId)
                 .OrderByDescending(g => g.Count())
                 .Take(3)
                 .Select(g => g.Key)
                 .ToListAsync();
        }


        public async Task<int> CountPostsByAuthor(string authorId)
        {
            return await _context.Posts
                .Where(p => p.AuthorId == authorId && !p.IsDeleted)  
                .CountAsync();  
        }

    }
}
