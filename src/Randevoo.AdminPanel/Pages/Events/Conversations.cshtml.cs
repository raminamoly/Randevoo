using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.Infrastructure.Data;

namespace Randevoo.AdminPanel.Pages.Events;

[Authorize(Policy = Policies.AdminOnly)]
public class ConversationsModel : PageModel
{
    private readonly RandevooDbContext _db;

    public ConversationsModel(RandevooDbContext db)
    {
        _db = db;
    }

    public long EventId { get; private set; }
    public string EventTitle { get; private set; } = string.Empty;
    public IReadOnlyList<EventConversationItem> Conversations { get; private set; } = Array.Empty<EventConversationItem>();

    public async Task<IActionResult> OnGetAsync(long eventId)
    {
        var datingEvent = await _db.DatingEvents.FirstOrDefaultAsync(item => item.Id == eventId);
        if (datingEvent is null)
            return NotFound();

        EventId = datingEvent.Id;
        EventTitle = datingEvent.Title;

        var conversations = await _db.EventConversations
            .Include(item => item.StarterUser)
            .ThenInclude(user => user.Profile)
            .Include(item => item.ParticipantUser)
            .ThenInclude(user => user.Profile)
            .Include(item => item.Messages)
            .ThenInclude(message => message.SenderUser)
            .ThenInclude(user => user.Profile)
            .Where(item => item.DatingEventId == eventId)
            .OrderByDescending(item => item.UpdatedAt ?? item.CreatedAt)
            .ToListAsync();

        Conversations = conversations.Select(item =>
        {
            var lastMessage = item.Messages
                .OrderByDescending(message => message.CreatedAt)
                .FirstOrDefault();

            return new EventConversationItem
            {
                Id = item.Id,
                StarterName = item.StarterUser.Profile?.DisplayName ?? item.StarterUser.MobileNumber,
                ParticipantName = item.ParticipantUser.Profile?.DisplayName ?? item.ParticipantUser.MobileNumber,
                IsDisabled = item.IsDisabled,
                DisabledReason = item.DisabledReason,
                CreatedAtUtc = DateTime.SpecifyKind(item.CreatedAt, DateTimeKind.Utc),
                LastMessageAtUtc = lastMessage is null
                    ? null
                    : DateTime.SpecifyKind(lastMessage.CreatedAt, DateTimeKind.Utc),
                LastMessageSenderName = lastMessage?.SenderUser.Profile?.DisplayName ?? lastMessage?.SenderUser.MobileNumber,
                LastMessageBody = lastMessage?.Body,
                MessageCount = item.Messages.Count
            };
        }).ToList();

        return Page();
    }

    public sealed class EventConversationItem
    {
        public long Id { get; set; }
        public string StarterName { get; set; } = string.Empty;
        public string ParticipantName { get; set; } = string.Empty;
        public bool IsDisabled { get; set; }
        public string? DisabledReason { get; set; }
        public DateTimeOffset CreatedAtUtc { get; set; }
        public DateTimeOffset? LastMessageAtUtc { get; set; }
        public string? LastMessageSenderName { get; set; }
        public string? LastMessageBody { get; set; }
        public int MessageCount { get; set; }
    }
}
