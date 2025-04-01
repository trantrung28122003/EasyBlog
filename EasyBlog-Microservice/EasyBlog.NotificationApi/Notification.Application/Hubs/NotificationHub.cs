    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;
    using NotificationApi.Application.DTOs.Responses;

namespace NotificationApi.Application.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task RegisterUser(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
            Console.WriteLine($"✅ [RegisterUser] User {userId} đã được thêm vào group!");
        }
        public async Task SendNotification(string userId, NotificationResponse notificationResponse)
        {
           
            await Clients.Group(userId).SendAsync("ReceiveNotification", notificationResponse);
        }
    }
}
