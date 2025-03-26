using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthenticationApi.Application.DTOs.Reponses;
using AuthenticationApi.Application.DTOs.Requests;
using EasyBlog.SharedLibrary.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;

namespace AuthenticationApi.Application.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<UserResponse>> GetUserByIdAsync(string userId);
        Task<ApiResponse<UserResponse>> GetCurrentUserAsync();
        Task<ApiResponse<string>> Register(UserRegisterRequest request, IFormFile avatar);
        Task<ApiResponse<string>> Login(UserLoginRequest request);
        Task<ApiResponse<List<UserResponse>>> GetUsersByIdsAsync(List<string> userIds);
    }

}
