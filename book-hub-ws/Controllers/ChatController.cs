using book_hub_ws.Chat;
using book_hub_ws.DAL;
using book_hub_ws.Models;
using book_hub_ws.Models.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace book_hub_ws.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ChatController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartConversation([FromBody] StartConversationDto dto)
        {
            var receiver = await _context.Users.FirstOrDefaultAsync(u => u.Nickname == dto.ReceiverNickname);
            if (receiver == null)
            {
                return NotFound("Receiver not found");
            }

            var existingConversation = await _context.Conversations
                .FirstOrDefaultAsync(c => (c.InitiatorUserId == dto.InitiatorUserId && c.ReceiverUserId == receiver.UserId) ||
                                          (c.InitiatorUserId == receiver.UserId && c.ReceiverUserId == dto.InitiatorUserId));

            if (existingConversation != null)
            {
                return Ok(existingConversation);
            }

            var conversation = new Conversation
            {
                InitiatorUserId = dto.InitiatorUserId,
                ReceiverUserId = receiver.UserId
            };

            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();

            return Ok(conversation);
        }


        [HttpGet("conversations")]
        public async Task<ActionResult<IEnumerable<ConversationDto>>> GetConversations()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("User ID not found in token");
            }
            var conversations = await _context.Conversations
                .Where(c => c.InitiatorUserId.ToString() == userId || c.ReceiverUserId.ToString() == userId)
                .Select(c => new ConversationDto
                {
                    Id = c.Id,
                    InitiatorUserNickname = c.InitiatorUser.Nickname,
                    ReceiverUserNickname = c.ReceiverUser.Nickname,
                    LastMessage = c.Messages.OrderByDescending(m => m.Timestamp).FirstOrDefault().Text,
                    UnreadMessagesCount = c.Messages.Count(m => !m.IsRead && m.SenderUserId.ToString() != userId)
                })
                .ToListAsync();

            return Ok(conversations);
        }

        [HttpGet("conversations/{id}/messages")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessages(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User ID not found in token");
            }

            var messages = await _context.Messages
                .Where(m => m.ConversationId == id)
                .Include(m => m.SenderUser)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            foreach (var message in messages)
            {
                if (message.SenderUserId != int.Parse(userId))
                {
                    message.IsRead = true;
                }
            }

            await _context.SaveChangesAsync();

            var messageDtos = messages.Select(m => new MessageDto
            {
                Id = m.Id,
                SenderUserId = m.SenderUserId,
                SenderNickname = m.SenderUser?.Nickname,
                Text = m.Text,
                Timestamp = m.Timestamp,
                IsRead = m.IsRead
            }).ToList();

            return Ok(messageDtos);
        }

   

        [HttpGet("unread-count")]
        public async Task<ActionResult<int>> GetUnreadMessageCount()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("User ID not found in token");
            }

            var unreadMessageCount = await _context.Messages
                .Where(m => m.Conversation.ReceiverUserId == int.Parse(userId)
                            || m.Conversation.InitiatorUserId == int.Parse(userId))
                .Where(m => !m.IsRead && m.SenderUserId != int.Parse(userId)) 
                .CountAsync();

            return Ok(unreadMessageCount);
        }


    }
}
