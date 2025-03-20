using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using EasyBlog.SharedLibrary.Response;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationApi.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;

        public UserService(IUserRepository userRepository, IConfiguration config)
        {
            _userRepository = userRepository;
            _config = config;
        }

        public async Task<ApiResponse<GetUserDTO>> GetUserByIdAsync(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return new ApiResponse<GetUserDTO>(false, "User not found", null);
            }

            var userDto = new GetUserDTO
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                Role = user.Role
            };

            return new ApiResponse<GetUserDTO>(true, "User retrieved successfully", userDto);
        }

        public async Task<ApiResponse<string>> Register(ApplicationUserDTO applicationUserDTO)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(applicationUserDTO.Email);
            if (existingUser != null)
            {
                return new ApiResponse<string>(false, "Email đã được sử dụng", null);
            }

            var newUser = new ApplicationUser
            {
                FullName = applicationUserDTO.FullName,
                Email = applicationUserDTO.Email,
                PhoneNumber = applicationUserDTO.PhoneNumber,
                Address = applicationUserDTO.Address,
                Role = applicationUserDTO.Role,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(applicationUserDTO.Password)
            };

            await _userRepository.CreateUserAsync(newUser);

            return new ApiResponse<string>(true, "Đăng ký thành công", newUser.Id.ToString());
        }

        public async Task<ApiResponse<string>> Login(LoginDTO loginDTO)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDTO.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.PasswordHash))
            {
                return new ApiResponse<string>(false, "Email hoặc mật khẩu không đúng", null);
            }

            string token = GenerateJwtToken(user);

            return new ApiResponse<string>(true, "Đăng nhập thành công", token);
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var key = Encoding.UTF8.GetBytes(_config.GetSection("Authentication:Key").Value!);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.FullName!),
                new(ClaimTypes.Email, user.Email!)
            };

            if (!string.IsNullOrEmpty(user.Role))
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role));
            }

            var token = new JwtSecurityToken(
                issuer: _config["Authentication:Issuer"],
                audience: _config["Authentication:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Set token expiration
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
