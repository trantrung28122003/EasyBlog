

using NotificationApi.Application.DTOs.Resquest;

namespace NotificationApi.Application.DTOs.Requests
{
    public class CreateNotificationRequest
    {
        public List<string> UserIds { get; set; } = [];
        public required string TypeNotification { get; set; }
        public required string Message { get; set; }
    }
}
