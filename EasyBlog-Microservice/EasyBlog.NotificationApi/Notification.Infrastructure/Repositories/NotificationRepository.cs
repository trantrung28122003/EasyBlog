using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NotificationApi.Application.Interfaces;
using NotificationApi.Domain.Entities;
using NotificationApi.Infrastructure.Data;

namespace NotificationApi.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDbContext _context;

        public NotificationRepository(NotificationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Notification>> GetAllAsync()
        {
            return await _context.Notifications
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task AddNotificationsAsync(List<Notification> notifications)
        {
            await _context.Notifications.AddRangeAsync(notifications);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Notification>> GetAllActiveAsync()
        {
            return await _context.Notifications
                .Where(n => !n.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Notification>> FindByConditionAsync(Expression<Func<Notification, bool>> predicate)
        {
            return await _context.Notifications
                .Where(predicate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Notification?> GetByIdAsync(Guid id)
        {
            return await _context.Notifications
                .Where(n => n.Id == id)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Notification notification)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Notification notification)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            var existingNotification = await _context.Notifications.FindAsync(notification.Id);
            if (existingNotification == null)
                throw new KeyNotFoundException("Notification not found");

            existingNotification.Message = notification.Message;
            existingNotification.IsRead = notification.IsRead;
            existingNotification.Type = notification.Type;
            existingNotification.DateCreate = notification.DateCreate;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var existingNotification = await _context.Notifications.FindAsync(id);
            if (existingNotification == null)
                throw new KeyNotFoundException("Notification not found");

            _context.Notifications.Remove(existingNotification);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(Guid id)
        {
            var existingNotification = await _context.Notifications.FindAsync(id);
            if (existingNotification == null)
                throw new KeyNotFoundException("Notification not found");

            existingNotification.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetNotificationsByUserIdAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsDeleted)
                .OrderByDescending(n => n.DateCreate)
                .ToListAsync();
        }

        public async Task CreateNotificationsAsync(List<Notification> notifications)
        {
            await _context.Notifications.AddRangeAsync(notifications);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateNotificationsAsync(List<Notification> notifications)
        {
            _context.Notifications.UpdateRange(notifications);
            await _context.SaveChangesAsync();
        }


    }
}
