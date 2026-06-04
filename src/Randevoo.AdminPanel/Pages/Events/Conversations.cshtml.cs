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

        Conversations = conversations.Select(item => new EventConversationItem
        {
            Id = item.Id,
            StarterName = item.StarterUser.Profile?.DisplayName ?? item.StarterUser.MobileNumber,
            ParticipantName = item.ParticipantUser.Profile?.DisplayName ?? item.ParticipantUser.MobileNumber,
            IsDisabled = item.IsDisabled,
            DisabledReason = item.DisabledReason,
            CreatedAtUtc = DateTime.SpecifyKind(item.CreatedAt, DateTimeKind.Utc),
            MessageCount = item.Messages.Count,
            Messages = item.Messages
                .OrderBy(message => message.CreatedAt)
                .Select(message => new EventConversationMessageItem
                {
                    SenderName = message.SenderUser.Profile?.DisplayName ?? message.SenderUser.MobileNumber,
                    Body = message.Body,
                    SentAtUtc = DateTime.SpecifyKind(message.CreatedAt, DateTimeKind.Utc)
                })
                .ToList()
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
        public int MessageCount { get; set; }
        public IReadOnlyList<EventConversationMessageItem> Messages { get; set; } = Array.Empty<EventConversationMessageItem>();
    }

    public sealed class EventConversationMessageItem
    {
        public string SenderName { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTimeOffset SentAtUtc { get; set; }
    }
}
