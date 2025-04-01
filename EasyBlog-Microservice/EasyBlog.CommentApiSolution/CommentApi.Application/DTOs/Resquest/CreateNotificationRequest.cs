using CommentApi.Application.DTOs.Resquest;

namespace CommentApi.Application.DTOs.Resquest
{
    public class CreateNotificationRequest
    {
        public List<string> UserIds { get; set; } = [];
        public required string TypeNotification { get; set; }

        public required string Message { get; set; }
    }
}
