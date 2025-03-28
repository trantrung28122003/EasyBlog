using EasyBlog.SharedLibrary.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UploadApi.Application.DTOs.Requests;
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



        [HttpGet("{id}")]
        public async Task<IActionResult> GetFileById(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) 
                return BadRequest(new ApiResponse<List<FileMetadataResponse>>(false, "ID không hợp lệ", null));

            var response = await _fileMetadataService.GetFileMetadataByIdAsync(id);

            if (!response.IsSuccess)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("getFilesMetadataByIds")]
        public async Task<IActionResult> GetFilesByIds([FromBody] FileMetadataIdsRequest request)
        {
            if (request.FileIds == null || !request.FileIds.Any())
                return BadRequest(new ApiResponse<List<FileMetadataResponse>>(false, "Danh sách ID không hợp lệ", null));

            var response = await _fileMetadataService.GetFilesMetadataByIdsAsync(request.FileIds);

            if (!response.IsSuccess)
                return BadRequest(response);

            return Ok(response);
        }



        [HttpPost("upload")]
        public async Task<IActionResult> UploadSingleFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new ApiResponse<FileMetadataResponse>(false, "File không hợp lệ", null));

            var response = await _fileMetadataService.UploadSingleFileAsync(file);

            if (!response.IsSuccess)
                return BadRequest(response);

            return Ok(response);
        }

        [Authorize]
        [HttpPost("upload-multiple")]
        public async Task<IActionResult> UploadMutipleFile([FromForm]List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return BadRequest(new ApiResponse<List<FileMetadataResponse>>(false, "File không hợp lệ", null));

            var response = await _fileMetadataService.UploadMultipleFilesAsync(files);

            if (!response.IsSuccess)
                return BadRequest(response);

            return Ok(response);
        }



        

    }
}
