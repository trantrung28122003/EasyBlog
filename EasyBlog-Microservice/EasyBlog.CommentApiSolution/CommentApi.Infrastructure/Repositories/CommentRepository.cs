using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using CommentApi.Application.Interfaces;
using CommentApi.Domain.Entities;
using CommentApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace CommentApi.Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly CommentDbContext _context;

        public CommentRepository(CommentDbContext context)
        {
            _context = context;
        }
        public async Task<List<Comment>> GetAllAsync()
        {
            return await _context.Comments
            .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Comment>> GetAllActiveAsync()
        {
            return await _context.Comments
                .Where(p => !p.IsDeleted)
            .AsNoTracking()
            .ToListAsync();
        }

        public async Task<List<Comment>> FindByConditionAsync(Expression<Func<Comment, bool>> predicate)
        {
            return await _context.Comments
                .Where(predicate)
                .AsNoTracking()
            .ToListAsync();
        }

        public async Task<Comment?> GetByIdAsync(Guid id)
        {
            return await _context.Comments
                .Where(p => p.Id == id)
                .AsNoTracking()
            .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Comment comment)
        {
            var existingPost = await _context.Comments.FindAsync(comment.Id);
            if (existingPost is null) throw new Exception("Post not found");

            existingPost.Content = comment.Content;
            existingPost.DateChange = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Comment comment)
        {
            var existingPost = await _context.Comments.FindAsync(comment.Id);
            if (existingPost is null) throw new Exception("Post not found");

            _context.Comments.Remove(existingPost);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Guid id)
        {
            var existingPost = await _context.Comments.FindAsync(id);
            if (existingPost is null) throw new Exception("Post not found");

            existingPost.IsDeleted = true;
            existingPost.DateChange = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

    }
}
