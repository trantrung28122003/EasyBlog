using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NotificationApi.Domain.Entities;
using NotificationApi.Domain.Enums;
using NotificationApi.Application.Interfaces;
using EasyBlog.SharedLibrary.Response;
using NotificationApi.Application.DTOs.Requests;
using NotificationApi.Application.DTOs.Responses;

using Microsoft.AspNetCore.Http;
using Polly.Registry;
using System.Net.Http.Json;
using System.Text.Json;
using NotificationApi.Application.DTOs.Resquest;
using NotificationApi.Application.DTOs.Conversions;
using Microsoft.AspNetCore.SignalR;
using NotificationApi.Application.Hubs;

namespace NotificationApi.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ResiliencePipelineProvider<string> _resiliencePipeline;
        private readonly IHubContext<NotificationHub> _hubContext;
        public NotificationService(INotificationRepository notificationRepository, 
            INotificationRepository userRepository, HttpClient httpClient, IHttpContextAccessor httpContextAccessor,
            ResiliencePipelineProvider<string> resiliencePipeline,IHubContext<NotificationHub> hubContext)
        {
            _notificationRepository = notificationRepository;
            _httpClient = httpClient;
            _resiliencePipeline = resiliencePipeline;
            _httpContextAccessor = httpContextAccessor;
            _hubContext = hubContext;
        }

        // Create a new notification
        public async Task<ApiResponse<string>> CreateNotificationsAsync(CreateNotificationRequest request)
        {
            try
            {
                if (request.UserIds == null || !request.UserIds.Any())
                    return new ApiResponse<string>(false, "Danh sách UserId bị lỗi", null);
                if (!Enum.TryParse<NotificationType>(request.TypeNotification, true, out var notificationType))
                    return new ApiResponse<string>(false, "Loại thông báo không hợp lệ", null);

                var notifications = request.UserIds.Select(userId => new Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Message = request.Message,
                    Type = notificationType,
                    IsRead = false,
                    IsDeleted = false,
                    DateCreate = DateTime.UtcNow
                }).ToList();

                await _notificationRepository.CreateNotificationsAsync(notifications);


                foreach (var notification in notifications)
                {
                    if (!string.IsNullOrEmpty(notification.UserId))
                    {
                        await _hubContext.Clients.Group(notification.UserId)
                            .SendAsync("ReceiveNotification", notification);
                    }
                }

                return new ApiResponse<string>(true, "Tạo thông báo thành công cho nhiều người dùng", "Success");
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(false, "Lỗi khi tạo thông báo", ex.Message);
            }
        }

        // Get all notifications for a user

        public async Task<ApiResponse<List<NotificationResponse>>> GetNotificationsByUserIdAsync(string userId)
        {
            try
            {
                var notifications = await _notificationRepository.GetNotificationsByUserIdAsync(userId);

                if (notifications == null || !notifications.Any())
                    return new ApiResponse<List<NotificationResponse>>(false, "Không có thông báo nào", new List<NotificationResponse>());

                var notificationResponses = notifications.Select(
                    NotificationConversion.FormEntityToNotificationResponse).ToList();

                return new ApiResponse<List<NotificationResponse>>(true, "Lấy danh sách thông báo thành công", notificationResponses);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<NotificationResponse>>(false, "Lỗi khi lấy thông báo", new List<NotificationResponse>());
            }
        }

        public async Task<ApiResponse<List<NotificationResponse>>> GetNotificationsByCurrentUserAsync()
        {
            try
            {
                var retryPipeline = _resiliencePipeline.GetPipeline("my-retry-pipeline");
                var authorResponse = await retryPipeline.ExecuteAsync(async _ => await GetCurrentUserAsync());

                if (authorResponse is null)
                {
                    return new ApiResponse<List<NotificationResponse>> (false, "Không tìm thấy thông tin người dùng đang đăng nhập", null);
                }
                var notifications = await _notificationRepository.GetNotificationsByUserIdAsync(authorResponse.Id);

                if (notifications == null || !notifications.Any())
                    return new ApiResponse<List<NotificationResponse>>(false, "Không có thông báo nào", new List<NotificationResponse>());

                var notificationResponses = notifications.Select(
                    NotificationConversion.FormEntityToNotificationResponse).ToList();

                return new ApiResponse<List<NotificationResponse>>(true, "Lấy danh sách thông báo thành công", notificationResponses);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<NotificationResponse>>(false, "Lỗi khi lấy thông báo", new List<NotificationResponse>());
            }
        }
        // Mark a notification as read
        public async Task<ApiResponse<string>> MarkAsReadAsync(string notificationId)
        {
            try
            {
                if(!Guid.TryParse(notificationId, out var notificationGuid))
                    return new ApiResponse<string>(false, "Id thông báo không hợp lệ", notificationId);
                var notification = await _notificationRepository.GetByIdAsync(notificationGuid);
                if (notification == null)
                    return new ApiResponse<string>(false, "Không tìm thấy thông báo", null);

                if (notification.IsRead)
                    return new ApiResponse<string>(false, "Thông báo đã được đọc trước đó", notificationId.ToString());

                notification.IsRead = true;
                await _notificationRepository.UpdateAsync(notification);

                return new ApiResponse<string>(true, "Đánh dấu thông báo là đã đọc thành công", notificationId.ToString());
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(false, "Lỗi khi đánh dấu thông báo là đã đọc", ex.Message);
            }
        }


        // Soft delete a notification
        public async Task<ApiResponse<string>> SoftDeleteAllNotificationsByUserIdAsync(string userId)
        {
            try
            {
                var notifications = await _notificationRepository.GetNotificationsByUserIdAsync(userId);
                if (notifications == null || !notifications.Any())
                    return new ApiResponse<string>(false, "Không có thông báo để xóa", null);

                foreach (var notification in notifications)
                {
                    notification.IsDeleted = true;
                }

                await _notificationRepository.UpdateNotificationsAsync(notifications);

                return new ApiResponse<string>(true, "Xóa tất cả thông báo thành công", userId);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>(false, "Lỗi khi xóa tất cả thông báo", ex.Message);
            }
        }

        private async Task<AuthorResponse?> GetCurrentUserAsync()
        {

            var response = await _httpClient.GetAsync($"/api/users/profile");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var responseContent = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<AuthorResponse>>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return apiResponse?.Results;
        }
    }
}
