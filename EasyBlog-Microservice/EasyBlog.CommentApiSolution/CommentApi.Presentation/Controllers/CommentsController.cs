using CommentApi.Application.DTOs.Responses;
using CommentApi.Application.DTOs.Resquest;
using CommentApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommentApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;
        public CommentsController(ICommentService commentService) 
        { 
            _commentService = commentService;   
        }

        [Authorize]
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById(string Id)
        {
            var response = await _commentService.GetByIdAsync(Id);
            if (!response.IsSuccess)
                return NotFound(response);
            return Ok(response);
        }

       
        [HttpGet("commentsByPostId/{postId}")]
        public async Task<IActionResult> GetCommentsByPostId(string postId)
        {
            var response = await _commentService.GetCommentsByPostIdAsync(postId);
            if (!response.IsSuccess)
                return NotFound(response);
            return Ok(response);
        }

        [HttpPost("commentsByPostIds")]
        public async Task<IActionResult> GetCommentsByPostIds([FromBody] PostIdsRequest request)
        {
            var response = await _commentService.GetCommentsByPostIdsAsync(request);
            if (!response.IsSuccess)
                return NotFound(response);
            return Ok(response);
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateCommentRequest request)
        {
            var response = await _commentService.CreateAsync(request);
            if (!response.IsSuccess)
                return BadRequest(response);
          
            return Ok(response);
        }


        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateCommentRequest request)
        {
            var response = await _commentService.UpdateAsync(request, id);
            if (!response.IsSuccess)
                return BadRequest(response);
            return Ok(response);
        }

 
        [HttpPut("soft/{id}")]
        public async Task<IActionResult> SoftDelete(string id)
        {
            var response = await _commentService.SoftDeleteAsync(id);
            if (!response.IsSuccess)
                return BadRequest(response);
            return Ok(response);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _commentService.DeleteAsync(id);
            if (!response.IsSuccess)
                return BadRequest(response);
            return Ok(response);
        }
    }
}
