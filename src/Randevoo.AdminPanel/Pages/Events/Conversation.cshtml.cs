using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.Domain.Entities;
using Randevoo.Infrastructure.Data;

namespace Randevoo.AdminPanel.Pages.Events;

[Authorize(Policy = Policies.AdminOnly)]
public class ConversationModel : PageModel
{
    private readonly RandevooDbContext _db;

    public ConversationModel(RandevooDbContext db)
    {
        _db = db;
    }

    public long EventId { get; private set; }
    public string EventTitle { get; private set; } = string.Empty;
    public EventConversationThread Conversation { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(long eventId, long conversationId)
    {
        var conversation = await _db.EventConversations
            .Include(item => item.DatingEvent)
            .Include(item => item.StarterUser)
            .ThenInclude(user => user.Profile)
            .Include(item => item.ParticipantUser)
            .ThenInclude(user => user.Profile)
            .Include(item => item.Messages)
            .ThenInclude(message => message.SenderUser)
            .ThenInclude(user => user.Profile)
            .Include(item => item.Blocks)
            .FirstOrDefaultAsync(item => item.DatingEventId == eventId && item.Id == conversationId);

        if (conversation is null)
            return NotFound();

        EventId = conversation.DatingEventId;
        EventTitle = conversation.DatingEvent.Title;

        Conversation = new EventConversationThread
        {
            Id = conversation.Id,
            StarterUserId = conversation.StarterUserId,
            ParticipantUserId = conversation.ParticipantUserId,
            StarterName = GetDisplayName(conversation.StarterUser),
            ParticipantName = GetDisplayName(conversation.ParticipantUser),
            IsDisabled = conversation.IsDisabled,
            DisabledReason = conversation.DisabledReason,
            DisabledAtUtc = conversation.DisabledAt.HasValue ? AsUtc(conversation.DisabledAt.Value) : null,
            CreatedAtUtc = AsUtc(conversation.CreatedAt),
            UpdatedAtUtc = AsUtc(conversation.UpdatedAt ?? conversation.CreatedAt),
            ActiveBlockCount = conversation.Blocks.Count(item => item.IsActive),
            Messages = conversation.Messages
                .OrderBy(message => message.CreatedAt)
                .Select(message => new EventConversationThreadMessage
                {
                    SenderName = GetDisplayName(message.SenderUser),
                    Body = message.Body,
                    SentAtUtc = AsUtc(message.CreatedAt),
                    IsStarterSender = message.SenderUserId == conversation.StarterUserId
                })
                .ToList()
        };

        return Page();
    }

    private static DateTimeOffset AsUtc(DateTime value)
        => DateTime.SpecifyKind(value, DateTimeKind.Utc);

    private static string GetDisplayName(User user)
        => user.Profile?.DisplayName ?? user.MobileNumber;

    public sealed class EventConversationThread
    {
        public long Id { get; set; }
        public long StarterUserId { get; set; }
        public long ParticipantUserId { get; set; }
        public string StarterName { get; set; } = string.Empty;
        public string ParticipantName { get; set; } = string.Empty;
        public bool IsDisabled { get; set; }
        public string? DisabledReason { get; set; }
        public DateTimeOffset? DisabledAtUtc { get; set; }
        public DateTimeOffset CreatedAtUtc { get; set; }
        public DateTimeOffset UpdatedAtUtc { get; set; }
        public int ActiveBlockCount { get; set; }
        public IReadOnlyList<EventConversationThreadMessage> Messages { get; set; } = Array.Empty<EventConversationThreadMessage>();
    }

    public sealed class EventConversationThreadMessage
    {
        public string SenderName { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTimeOffset SentAtUtc { get; set; }
        public bool IsStarterSender { get; set; }
    }
}
