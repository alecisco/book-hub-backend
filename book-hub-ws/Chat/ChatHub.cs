using book_hub_ws.DAL;
using book_hub_ws.Models.EF;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace book_hub_ws.Chat
{
    public class ChatHub : Hub
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public ChatHub(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task SendMessage(string user, string message, int conversationId)
        {
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                throw new HubException("User not authenticated.");
            }

            var messageEntity = new Message
            {
                ConversationId = conversationId,
                SenderUserId = int.Parse(userId),
                Text = message,
                Timestamp = DateTime.UtcNow
            };

            _context.Messages.Add(messageEntity);
            await _context.SaveChangesAsync();

            await Clients.Group(conversationId.ToString()).SendAsync("ReceiveMessage", user, message, conversationId);
            await Clients.All.SendAsync("NewMessageNotification", user, message, conversationId);
        }

        public async Task JoinConversation(int conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
        }

        
    }
}
