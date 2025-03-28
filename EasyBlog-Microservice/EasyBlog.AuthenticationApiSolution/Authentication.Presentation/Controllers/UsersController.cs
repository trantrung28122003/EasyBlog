using AuthenticationApi.Application.DTOs.Request;
using AuthenticationApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var response = await _userService.GetCurrentUserAsync();
            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

       
        [HttpPost("getUsersByIds")]
        public async Task<IActionResult> GetUsersByIds([FromBody] UserIdsRequest request)
        {
            var response = await _userService.GetUsersByIdsAsync(request);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

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
