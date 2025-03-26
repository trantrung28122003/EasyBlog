using EasyBlog.SharedLibrary.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UploadApi.Application.DTOs.Responses;
using UploadApi.Application.Interfaces;
using UploadApi.Domain.Entites;

namespace UploadApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class FileMetadataController : ControllerBase
    {
        private readonly IFileMetadataService _fileMetadataService;

        public FileMetadataController(IFileMetadataService fileMetadataService)
        {
            _fileMetadataService = fileMetadataService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new ApiResponse<FileMetadataResponse>(false, "File không hợp lệ", null));

            var response = await _fileMetadataService.UploadAndSaveFileMetadataAsync(file);

            if (!response.IsSuccess)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("getFilesMetadataByIds")]
        public async Task<IActionResult> GetAvatarsByIds([FromBody] List<string> fileIds)
        {
            if (fileIds == null || !fileIds.Any())
                return BadRequest(new ApiResponse<List<FileMetadataResponse>>(false, "Danh sách ID không hợp lệ", null));

            var response = await _fileMetadataService.GetFilesMetadataByIdsAsync(fileIds);

            if (!response.IsSuccess)
                return BadRequest(response);

            return Ok(response);
        }

    }
}
