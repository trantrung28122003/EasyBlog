using System.Runtime.InteropServices;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PostApi.Application.DTOs;
using PostApi.Application.DTOs.Conversions;
using PostApi.Application.DTOs.Requests;
using PostApi.Application.Interfaces;
using PostApi.Domain.Entities;
using PostApi.Infrastructure.Repositories;

namespace PostApi.Presentation.Controllers
{
    [Route("api/[controller]")] 
    [ApiController]
    [AllowAnonymous]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllPosts()
        {
            var response = await _postService.GetAllAsync();
            return Ok(response);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPostspaged([FromQuery] int offset = 0, [FromQuery] int limit = 5)
        {
            var response = await _postService.GetPostsByPageAsync(offset, limit);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPostById(string Id)
        {
            var response = await _postService.GetPostByIdAsync(Id);
            if (!response.IsSuccess)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] CreatePostRequest request, List<IFormFile> files)
        {
           
            var response = await _postService.CreateAsync(request, files);
            if (!response.IsSuccess)
                return BadRequest(response);
            return Ok(response);
        }



        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdatePostRequest request)
        {
            var response = await _postService.UpdateAsync(request, id);
            if (!response.IsSuccess)
                return NotFound(response);
            return Ok(response);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _postService.DeleteAsync(id);
            if (!response.IsSuccess)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpPut("soft/{id}")]
        public async Task<IActionResult> SoftDelete(string id)
        {
            var response = await _postService.SoftDeleteAsync(id);
            if (!response.IsSuccess)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("top-authors-post")]
        public async Task<IActionResult> GetTopAuthors()
        {
            var response = await _postService.GetTop3AuthorsWithMostPosts();

            if (!response.IsSuccess)
                return Ok(response);

            return Ok(response);
        }


    }
}
