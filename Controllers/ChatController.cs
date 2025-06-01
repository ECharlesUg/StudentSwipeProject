using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSwipe.Models;

[Authorize]
public class ChatController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ChatController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> StartChat(string receiverId, string context)
    {
        var sender = await _userManager.GetUserAsync(User);
        var receiver = await _context.Users.FirstOrDefaultAsync(u => u.Id == receiverId);

        if (sender == null || receiver == null) return NotFound();

        // Prevent cross-type chats
        var senderProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == sender.Id);
        var receiverProfile = await _context.Profiles.FirstOrDefaultAsync(p => p.UserId == receiver.Id);

        if (context == "Roommate" && (senderProfile.UserType != "University" || receiverProfile.UserType != "University"))
            return Forbid();

        ViewBag.Receiver = receiver;
        ViewBag.Context = context;

        var messages = await _context.ChatMessages
            .Where(m =>
                ((m.SenderId == sender.Id && m.ReceiverId == receiverId) ||
                (m.SenderId == receiverId && m.ReceiverId == sender.Id)) &&
                m.Context == context)
            .OrderBy(m => m.Timestamp)
            .ToListAsync();

        return View("Chat", messages);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessage(string receiverId, string message, string context)
    {
        var sender = await _userManager.GetUserAsync(User);
        var chatMessage = new ChatMessage
        {
            SenderId = sender.Id,
            ReceiverId = receiverId,
            Message = message,
            Context = context
        };
        _context.ChatMessages.Add(chatMessage);
        await _context.SaveChangesAsync();

        return Ok();
    }
}
