
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using SignalRGateway.Application.DTOs.Responses;
using SignalRGateway.Presentation.Hubs;

namespace SignalRGateway.Application.Consumers
{
    public class CommentCreatedConsumer : IConsumer<CommentResponse>
    {
        private readonly IHubContext<CommentHub> _hubContext;
       

        public CommentCreatedConsumer(IHubContext<CommentHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<CommentResponse> context)
        {
            var comment = context.Message;
            await _hubContext.Clients.Group(comment.PostId).SendAsync("ReceiveComment", comment);
        }
    }
}
