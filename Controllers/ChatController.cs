using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenAI_API;
using StudentSwipe.Models;
using System.Security.Claims;

namespace StudentSwipe.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly OpenAIAPI _openAI;

        public ChatController(ApplicationDbContext context, OpenAIAPI openAI)
        {
            _context = context;
            _openAI = openAI;
        }

        [HttpGet]
        public async Task<IActionResult> StartChat(string receiverId, string context)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId == null || receiverId == null) return Unauthorized();

            var messages = await _context.ChatMessages
                .Where(m =>
                    (m.SenderId == currentUserId && m.ReceiverId == receiverId) ||
                    (m.SenderId == receiverId && m.ReceiverId == currentUserId))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            ViewBag.Receiver = await _context.Users.FindAsync(receiverId);
            ViewBag.Context = context;
            return View("Chat", messages);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string receiverId, string message, string context)
        {
            var senderId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(senderId) || string.IsNullOrWhiteSpace(receiverId)) return BadRequest();

            // OpenAI moderation
            
            var result = await _openAI.Moderation.CallModerationAsync(message);
            var isFlagged = result.Results.FirstOrDefault()?.Flagged ?? false;

            if (isFlagged)
            {
                return BadRequest("Message was flagged by OpenAI moderation.");
            }


            var msg = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message,
                Context = context,
                Timestamp = DateTime.UtcNow
            };

            _context.ChatMessages.Add(msg);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
