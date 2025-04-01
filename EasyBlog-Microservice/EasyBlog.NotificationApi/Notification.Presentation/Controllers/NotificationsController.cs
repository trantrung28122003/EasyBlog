using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationApi.Application.Interfaces;
using NotificationApi.Application.DTOs.Requests;


namespace NotificationApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            var response = await _notificationService.GetNotificationsByUserIdAsync(userId);
            if (!response.IsSuccess)
                return NotFound(response);
            return Ok(response);
        }

        [Authorize]
        [HttpGet("current-user")]
        public async Task<IActionResult> GetNotificationByCurrentUser()
        {
            var response = await _notificationService.GetNotificationsByCurrentUserAsync();
            if (!response.IsSuccess)
                return Ok(response);
            return Ok(response);
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateNotificationRequest request)
        {
            var response = await _notificationService.CreateNotificationsAsync(request);
            if (!response.IsSuccess)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpPut("mark-as-read/{notificationId}")]
        public async Task<IActionResult> MarkAsRead(string notificationId)
        {
            var response = await _notificationService.MarkAsReadAsync(notificationId);
            if (!response.IsSuccess)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpPut("soft-delete/user/{userId}")]
        public async Task<IActionResult> SoftDeleteAllByUser(string userId)
        {
            var response = await _notificationService.SoftDeleteAllNotificationsByUserIdAsync(userId);
            if (!response.IsSuccess)
                return BadRequest(response);
            return Ok(response);
        }
    }
}
