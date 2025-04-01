
using EasyBlog.SharedLibrary.Response;
using Microsoft.AspNetCore.Http;
using NotificationApi.Application.DTOs.Requests;
using NotificationApi.Application.DTOs.Responses;

namespace NotificationApi.Application.Interfaces
{
    public interface INotificationService
    {
        Task<ApiResponse<string>> CreateNotificationsAsync(CreateNotificationRequest request );
        Task<ApiResponse<List<NotificationResponse>>> GetNotificationsByUserIdAsync(string userId);
        Task<ApiResponse<List<NotificationResponse>>> GetNotificationsByCurrentUserAsync();
        Task<ApiResponse<string>> MarkAsReadAsync(string id);
        Task<ApiResponse<string>> SoftDeleteAllNotificationsByUserIdAsync(string id);
    }

}
