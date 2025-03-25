using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using EasyBlog.SharedLibrary.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthenticationController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] ApplicationUserDTO applicationUserDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(false, "Invalid model", null));
            }

            var response = await _userService.Register(applicationUserDTO);

            if (response.IsSuccess)
            {
                return Ok(response); 
            }

            return BadRequest(response); 
        }

        // Đăng nhập người dùng
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(false, "Invalid model", null));
            }

            var response = await _userService.Login(loginDTO);

            if (response.IsSuccess)
            {
                return Ok(response); 
            }

            return Unauthorized(response); 
        }
        [Authorize]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserAsync(string userId)
        {
            var response = await _userService.GetUserByIdAsync(userId);

            if (response.IsSuccess)
            {
                return Ok(response); 
            }

            return NotFound(response);  
        }
    }
}
