
using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Domain.Entities;
using EasyBlog.SharedLibrary.Response;


namespace AuthenticationApi.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<ApplicationUser?> CreateUserAsync(ApplicationUser user);
        Task<List<ApplicationUser>?> GetUsersByIdsAsync(List<Guid> id);
    }

}
