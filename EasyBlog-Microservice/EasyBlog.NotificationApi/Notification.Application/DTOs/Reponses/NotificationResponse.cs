
using NotificationApi.Domain.Enums;

namespace NotificationApi.Application.DTOs.Responses
{
    public class NotificationResponse
    {
        public required string Id { get; set; }
        public required string Message { get; set; }
        public bool IsRead { get; set; }
        public string Type { get; set; }
        public DateTime DateCreate { get; set; }
    }
}

