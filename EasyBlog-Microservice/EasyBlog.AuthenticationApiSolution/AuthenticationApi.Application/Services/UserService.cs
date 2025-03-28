using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AuthenticationApi.Application.DTOs.Conversions;
using AuthenticationApi.Application.DTOs.Reponses;
using AuthenticationApi.Application.DTOs.Request;
using AuthenticationApi.Application.DTOs.Requests;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Domain.Enums;
using Azure;
using EasyBlog.SharedLibrary.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.SqlServer.Server;
using Polly;
using Polly.Registry;

namespace AuthenticationApi.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly ResiliencePipelineProvider<string> _resiliencePipeline;

        public UserService(IUserRepository userRepository, IConfiguration config, 
            IHttpContextAccessor httpContextAccessor, HttpClient httpClient,
            ResiliencePipelineProvider<string> resiliencePipeline
            )
        {
            _userRepository = userRepository;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
            _resiliencePipeline = resiliencePipeline;
        }

        public async Task<ApiResponse<UserResponse>> GetUserByIdAsync(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return new ApiResponse<UserResponse>(false, "Không tìm thấy tài khoản trong hệ thống", null);
            }
            var retryPipeline = _resiliencePipeline.GetPipeline("my-retry-pipeline");
            
           
            var fileMetadata = user.Avatar != null ? await retryPipeline.ExecuteAsync(async _ => await GetFileMetadataById(user.Avatar)) : null;
          

            var userReponse = UserConversion.FormEntityToUserResponse(user, fileMetadata?.FileUrl);

            return new ApiResponse<UserResponse>(true, "Tìm tài khoản thành công", userReponse);
        }

        public async Task<ApiResponse<string>> Register(UserRegisterRequest request, IFormFile? avatar)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return new ApiResponse<string>(false, "Email đã được sử dụng", null);
            }
            var retryPipeline = _resiliencePipeline.GetPipeline("my-retry-pipeline");

            string avatarId = "4278876e-1458-4f05-9941-f9dda054f640"; 

            if (avatar != null)
            {
                var fileMetadata = await retryPipeline.ExecuteAsync(async _ => await UploadFileAvatar(avatar));
                if (fileMetadata != null)
                {
                    avatarId = fileMetadata.Id; 
                }
            }

            var newUser = new ApplicationUser
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                Role = UserRole.User,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Avatar = avatarId,
                DateChange = DateTime.UtcNow
            };

            await _userRepository.CreateUserAsync(newUser);
            return new ApiResponse<string>(true, "Đăng ký thành công", newUser.FullName);
        }

        public async Task<ApiResponse<string>> Login(UserLoginRequest request)
        {
            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return new ApiResponse<string>(false, "Email hoặc mật khẩu không đúng", null);
            }

            string token = GenerateJwtToken(user);

            return new ApiResponse<string>(true, "Đăng nhập thành công", token);
        }
        public async Task<ApiResponse<UserResponse>> GetCurrentUserAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
           
            if (string.IsNullOrEmpty(userId))
            {
                return new ApiResponse<UserResponse>(false, "Không tìm thấy thông tin user từ token", null);
            }

            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return new ApiResponse<UserResponse>(false, "User không tồn tại", null);
            }
            var retryPipeline = _resiliencePipeline.GetPipeline("my-retry-pipeline");
            var fileMetadata = user.Avatar != null ? await retryPipeline.ExecuteAsync(async _ => await GetFileMetadataById(user.Avatar)) : null;
         
            var userResponse = UserConversion.FormEntityToUserResponse(user, fileMetadata?.FileUrl);

            return new ApiResponse<UserResponse>(true, "Lấy thông tin user thành công", userResponse);
        }


        private string GenerateJwtToken(ApplicationUser user)
        {
            var key = Encoding.UTF8.GetBytes(_config.GetSection("Authentication:Key").Value!);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.FullName!),
                new(ClaimTypes.Email, user.Email!)
            };

            if (!string.IsNullOrEmpty(user.Role.ToString()))
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role.ToString()));
            }

            var token = new JwtSecurityToken(
                issuer: _config["Authentication:Issuer"],
                audience: _config["Authentication:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<FileMetadataReponse?> UploadFileAvatar(IFormFile file)
        {

            using var content = new MultipartFormDataContent();
            await using var stream = file.OpenReadStream();
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            content.Add(fileContent, "file", file.FileName);

            
            Console.WriteLine($"Content-Type: {content.Headers.ContentType}");
            var response = await _httpClient.PostAsync("/api/filemetadata/upload", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<FileMetadataReponse>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return apiResponse?.Results;
        }

        public async Task<FileMetadataReponse?> GetFileMetadataById(string fileId)
        {
            var response = await _httpClient.GetAsync($"/api/filemetadata/{fileId}");

            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = await response.Content.ReadAsStringAsync();
            var fileMetadataReponse = JsonSerializer.Deserialize<ApiResponse<FileMetadataReponse>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return fileMetadataReponse?.Results;
        }

        private async Task<List<FileMetadataReponse>?> GetFilesMetadataByUsers(List<ApplicationUser> users)
        {

            var fileIds = users
                .Where(user => !string.IsNullOrEmpty(user.Avatar))
                .Select(user => user.Avatar!)
                .Distinct()
                .ToList();

            var request = new FileMetadataIdsRequest { FileIds = fileIds };

            var response = await _httpClient.PostAsJsonAsync("/api/filemetadata/getFilesMetadataByIds", request);

            if (!response.IsSuccessStatusCode)
                return null;

            var responseContent = await response.Content.ReadAsStringAsync();
            var fileMetadataReponse = JsonSerializer.Deserialize<ApiResponse<List<FileMetadataReponse>>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return fileMetadataReponse?.Results;
        }


        public async Task<ApiResponse<List<UserResponse>>> GetUsersByIdsAsync(UserIdsRequest request)
        {
            if (request.UserIds == null || !request.UserIds.Any())
            {
                return new ApiResponse<List<UserResponse>>(false, "Danh sách userId không hợp lệ", null);
            }
            var guidUserIds = request.UserIds.Where(id => Guid.TryParse(id, out _))
                            .Select(Guid.Parse)
                            .ToList();
            var users = await _userRepository.GetUsersByIdsAsync(guidUserIds);
            if (users == null || !users.Any())
            {
                return new ApiResponse<List<UserResponse>>(false, "Không tìm thấy user nào", new List<UserResponse>());
            }

            var retryPipeline = _resiliencePipeline.GetPipeline("my-retry-pipeline");
            var fileMetadataList =  await retryPipeline.ExecuteAsync(async _ => await GetFilesMetadataByUsers(users));

            var avatarDictionary = fileMetadataList?.ToDictionary(file => file.Id, file => file.FileUrl) ?? new Dictionary<string, string>();

            var userDTOs = users.Select(user =>
                   UserConversion.FormEntityToUserResponse(user, avatarDictionary!.GetValueOrDefault(user.Avatar))
               ).ToList();

            return new ApiResponse<List<UserResponse>>(true, "Lấy danh sách user thành công", userDTOs);

        }
    }
}
