using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthenticationApi.Application.DTOs;
using EasyBlog.SharedLibrary.Response;

namespace AuthenticationApi.Application.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<GetUserDTO>> GetUserByIdAsync(string userId);
        Task<ApiResponse<string>> Register(ApplicationUserDTO applicationUserDTO);
        Task<ApiResponse<string>> Login(LoginDTO loginDTO);
    }

}
