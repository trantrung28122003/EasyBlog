using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace CommentApi.Application.Hubs
{
    public class CommentHub : Hub
    {
        public async Task JoinPostGroup(string postId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, postId);
            await Clients.Caller.SendAsync("GroupJoined", postId);

        }

        public async Task LeavePostGroup(string postId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, postId);
        }
    }
}
