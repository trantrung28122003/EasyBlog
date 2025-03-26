﻿
using EasyBlog.SharedLibrary.Response;
using Microsoft.AspNetCore.Http;
using UploadApi.Application.DTOs.Responses;
using UploadApi.Domain.Entites;

namespace UploadApi.Application.Interfaces
{
    public interface IFileMetadataService
    {
        Task<ApiResponse<FileMetadataResponse>> UploadAndSaveFileMetadataAsync(IFormFile file);
        Task<ApiResponse<FileMetadataResponse?>> GetFileMetadataByIdAsync(string id);
        Task<ApiResponse<List<FileMetadataResponse>?>> GetFilesMetadataByIdsAsync(List<string> fileIds);

    }
}
