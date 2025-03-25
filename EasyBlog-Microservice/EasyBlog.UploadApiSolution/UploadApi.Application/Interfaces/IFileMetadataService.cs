
using EasyBlog.SharedLibrary.Response;
using Microsoft.AspNetCore.Http;
using UploadApi.Application.DTOs;
using UploadApi.Domain.Entites;

namespace UploadApi.Application.Interfaces
{
    public interface IFileMetadataService
    {
        Task<ApiResponse<FileMetadataDTO>> UploadAndSaveFileMetadataAsync(IFormFile file);
        Task<ApiResponse<FileMetadataDTO?>> GetFileMetadataByIdAsync(string id);
    }
}
