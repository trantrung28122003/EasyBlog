using CommentApi.Application.DTOs;
using CommentApi.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommentApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;
        public CommentsController(ICommentService commentService) 
        { 
            _commentService = commentService;   
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetById(string Id)
        {
            var response = await _commentService.GetByIdAsync(Id);
            if (!response.IsSuccess)
                return NotFound(response);
            return Ok(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CommentDTO commentDTO)
        {
            var response = await _commentService.CreateAsync(commentDTO);
            if (!response.IsSuccess)
                return BadRequest(response);
            return CreatedAtAction(nameof(GetById), response);
        }

        [HttpGet("commentsByPostId/{postId}")]
        public async Task<IActionResult> GetCommentsByPostId(string postId)
        {
            var response = await _commentService.GetCommentsByPostIdAsync(postId);
            if (!response.IsSuccess)
                return NotFound(response);
            return Ok(response);
        }
    }
}
