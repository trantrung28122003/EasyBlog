using System;
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
        public async Task<ApiResponse<FileMetadataResponse>> UploadAndSaveFileMetadataAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return new ApiResponse<FileMetadataResponse>(false, "File không hợp lệ", null);
            var fileUrl = await _uploadFileService.UploadFileAsync(file);
            //var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //if (string.IsNullOrEmpty(userId))
            //{
            //    return new ApiResponse<FileMetadataDTO>(false, "Không tìm thấy thông tin người dùng", null);
            //}
            var fileMetadata = new FileMetadata
            {
                FileName = file.FileName,
                FileUrl = fileUrl,
                FileSize = file.Length,
                FileType = GetFileType(file.FileName),
                ChangeBy = "Hệ Thống"
            };

            await _fileMetadataRepository.CreateAsync(fileMetadata);
            var fileMetadataDTO = FileMetadataConversion.FromEntity(fileMetadata);

            if (fileMetadataDTO == null)
                return new ApiResponse<FileMetadataResponse>(false, "Lỗi khi chuyển đổi FileMetadata", null);
            return new ApiResponse<FileMetadataResponse>(true, "Tải file lên cloudinary và database thành công", fileMetadataDTO);
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
    }
}
