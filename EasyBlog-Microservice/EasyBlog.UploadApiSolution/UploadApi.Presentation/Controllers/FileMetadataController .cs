using EasyBlog.SharedLibrary.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UploadApi.Application.DTOs;
using UploadApi.Application.Interfaces;
using UploadApi.Domain.Entites;

namespace UploadApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class FileMetadataController : ControllerBase
    {
        private readonly IFileMetadataService _fileMetadataService;

        public FileMetadataController(IFileMetadataService fileMetadataService)
        {
            _fileMetadataService = fileMetadataService;
        }

        //[Authorize]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new ApiResponse<FileMetadataDTO>(false, "File không hợp lệ", null));

            var response = await _fileMetadataService.UploadAndSaveFileMetadataAsync(file);

            if (!response.IsSuccess)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFileMetadataById(string id)
        {
            var response = await _fileMetadataService.GetFileMetadataByIdAsync(id);
            if (response == null)
                return NotFound(response);
            return Ok(response);
        }

    }
}
