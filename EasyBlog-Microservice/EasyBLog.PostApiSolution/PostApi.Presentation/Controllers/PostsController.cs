using System.Runtime.InteropServices;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PostApi.Application.DTOs;
using PostApi.Application.DTOs.Conversions;
using PostApi.Application.Interfaces;
using PostApi.Domain.Entities;

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

        public async Task<IActionResult> GetAll()
        {
            var response = await _postService.GetAllAsync();
            return Ok(response);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetAllActive()
        {
            var response = await _postService.GetAllActiveAsync();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string Id)
        {
            var response = await _postService.GetByIdAsync(Id);
            if (!response.IsSuccess)
                return NotFound(response);
            return Ok(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] PostDTO postDTO)
        {
            var post = PostConversion.ToEntity(postDTO, postDTO.AuthorId);
            var response = await _postService.CreateAsync(post);
            if (!response.IsSuccess)
                return BadRequest(response);
            return CreatedAtAction(nameof(GetById), new { id = post.Id }, response);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] Post post)
        {
            var response = await _postService.UpdateAsync(post);
            if (!response.IsSuccess)
                return NotFound(response);
            return Ok(response);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromBody] Post post)
        {
            var response = await _postService.DeleteAsync(post);
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


    }
}
