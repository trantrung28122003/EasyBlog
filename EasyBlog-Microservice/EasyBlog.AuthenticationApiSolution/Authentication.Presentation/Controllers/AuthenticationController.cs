using AuthenticationApi.Application.DTOs.Requests;
using AuthenticationApi.Application.Interfaces;
using EasyBlog.SharedLibrary.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
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
        public async Task<IActionResult> RegisterAsync([FromForm] UserRegisterRequest request, IFormFile? avatar)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(false, "Invalid model", null));
            }

            var response = await _userService.Register(request, avatar!);

            if (response.IsSuccess)
            {
                return Ok(response); 
            }

            return BadRequest(response); 
        }


        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] UserLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string>(false, "Invalid model", null));
            }

            var response = await _userService.Login(request);

            if (response.IsSuccess)
            {
                return Ok(response); 
            }

            return Unauthorized(response); 
        }

        

        
    }
}
