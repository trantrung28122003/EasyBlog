﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Azure;
using EasyBlog.SharedLibrary.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using UploadApi.Application.DTOs.Conversions;
using UploadApi.Application.DTOs.Responses;
using UploadApi.Application.Interfaces;
using UploadApi.Domain.Entites;
using UploadApi.Domain.Enums;

namespace UploadApi.Application.Services
{
    public class FileMetadataService :IFileMetadataService
    {
        private readonly IFileMetadataRepository _fileMetadataRepository;
        private readonly IUploadFileService _uploadFileService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FileMetadataService(IFileMetadataRepository fileMetadataRepository, 
            IUploadFileService uploadFileService, IHttpContextAccessor httpContextAccessor) {
            _fileMetadataRepository = fileMetadataRepository;
            _uploadFileService = uploadFileService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<FileMetadataResponse>> UploadSingleFileAsync(IFormFile file)
        {
            var fileMetadataDTO = await UploadAndStoreFileMetadataAsync(file);

            if (fileMetadataDTO == null)
                return new ApiResponse<FileMetadataResponse>(false, "Tải file thất bại", null);

            return new ApiResponse<FileMetadataResponse>(true, "Tải file lên cloudinary và database thành công", fileMetadataDTO);
        }

        public async Task<ApiResponse<List<FileMetadataResponse>>> UploadMultipleFilesAsync(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return new ApiResponse<List<FileMetadataResponse>>(false, "Danh sách file không hợp lệ", null);

            var uploadedFiles = new List<FileMetadataResponse>();

            foreach (var file in files)
            {
                var fileMetadataDTO = await UploadAndStoreFileMetadataAsync(file);
                if (fileMetadataDTO != null)
                {
                    uploadedFiles.Add(fileMetadataDTO);
                }
            }

            return new ApiResponse<List<FileMetadataResponse>>(true, "Tải lên thành công", uploadedFiles);
        }
        public async Task<ApiResponse<FileMetadataResponse?>> GetFileMetadataByIdAsync(string id)
        {
            if(!Guid.TryParse(id, out Guid guidId))
            {
                return new ApiResponse<FileMetadataResponse?>(false, "ID không hợp lệ", null);
            }    
            var fileMetadata = await _fileMetadataRepository.GetByIdAsync(guidId);
            if(fileMetadata == null)
            {
                return new ApiResponse<FileMetadataResponse?>(false, "Không tìm thấy thông tin tệp", null);
            }

            var fileMetadataDTO = FileMetadataConversion.FromEntity(fileMetadata);

            return new ApiResponse<FileMetadataResponse?>(true, "Thông tin về tệp", fileMetadataDTO);
        }
        public async Task<ApiResponse<List<FileMetadataResponse>?>> GetFilesMetadataByIdsAsync(List<string> fileIds)
        {

            var guidFileIds = fileIds.Where(id => Guid.TryParse(id, out  _))
                            .Select(Guid.Parse)
                            .ToList();

            if (!guidFileIds.Any())
                return new ApiResponse<List<FileMetadataResponse>?>(false, "Không có ID nào hợp lệ", null);

            var fileMetadataList = await _fileMetadataRepository.GetFilesMetadataByIdsAsync(guidFileIds);
            if( fileMetadataList == null)
                 return new ApiResponse<List<FileMetadataResponse>?>(false, "Không thể lấy dữ liệu file", null);
            var fileMetadataDtos = fileMetadataList
                 .Select(FileMetadataConversion.FromEntity)
                 .ToList();

            return new ApiResponse<List<FileMetadataResponse>?>(true, "Lấy danh sách file thành công", fileMetadataDtos!);
        }

        private FileType GetFileType(string fileName)
        {
            var extension = System.IO.Path.GetExtension(fileName).ToLower();
            return extension switch
            {
                ".jpg" or ".jpeg" or ".png" or ".gif" => FileType.Image,
                ".mp4" or ".avi" or ".mkv" => FileType.Video,
                _ => FileType.Other
            };
        }
        private async Task<FileMetadataResponse?> UploadAndStoreFileMetadataAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;
            var fileUrl = await _uploadFileService.UploadFileAsync(file);
            //var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //if (string.IsNullOrEmpty(userId))
            //{
            //    return null;
            //}
            var fileMetadata = new FileMetadata
            {
                FileName = file.FileName,
                FileUrl = fileUrl,
                FileSize = file.Length,
                FileType = GetFileType(file.FileName),
                ChangeBy = "System"
            };

            await _fileMetadataRepository.CreateAsync(fileMetadata);
            var fileMetadataDTO = FileMetadataConversion.FromEntity(fileMetadata);
            return fileMetadataDTO;
        }
    }
}
