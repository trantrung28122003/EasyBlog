using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Data;
using EasyBlog.SharedLibrary.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationApi.Infrastructure.Respositories
{
    internal class UserRepository : IUserRepository
    {
        private readonly AuthenticationDbContext _context;

        public UserRepository(AuthenticationDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(p => p.Id.ToString() == userId);
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(p => p.Email == email);
        }

        public async Task<ApplicationUser> CreateUserAsync(ApplicationUser user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
