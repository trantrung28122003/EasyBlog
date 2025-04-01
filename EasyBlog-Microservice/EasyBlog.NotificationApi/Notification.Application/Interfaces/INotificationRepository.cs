
using EasyBlog.SharedLibrary.Interface;
using EasyBlog.SharedLibrary.Response;
using NotificationApi.Domain.Entities;


namespace NotificationApi.Application.Interfaces
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<List<Notification>> GetNotificationsByUserIdAsync(string userId);
        Task CreateNotificationsAsync(List<Notification> notifications);
        Task UpdateNotificationsAsync(List<Notification> notifications);
    }

}
