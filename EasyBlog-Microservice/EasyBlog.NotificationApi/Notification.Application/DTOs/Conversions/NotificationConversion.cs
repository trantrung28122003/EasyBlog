
using NotificationApi.Application.DTOs.Responses;
using NotificationApi.Domain.Entities;

namespace NotificationApi.Application.DTOs.Conversions
{
    public static class NotificationConversion
    {
        public static NotificationResponse FormEntityToNotificationResponse(Notification notification)
        {
            return new NotificationResponse
            {
                Id = notification.Id.ToString(),
                Message = notification.Message,
                IsRead = notification.IsRead,
                Type = notification.Type.ToString(),
                DateCreate = notification.DateCreate,
            };
        }
    }
}
