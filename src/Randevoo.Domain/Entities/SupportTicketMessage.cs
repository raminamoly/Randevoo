using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class SupportTicketMessage : BaseEntity
{
    private readonly List<SupportTicketAttachment> _attachments = new();

    public long SupportTicketId { get; private set; }
    public SupportTicket SupportTicket { get; private set; } = null!;
    public long SenderUserId { get; private set; }
    public User SenderUser { get; private set; } = null!;
    public UserRole SenderRole { get; private set; }
    public long? RepresentedUserId { get; private set; }
    public User? RepresentedUser { get; private set; }
    public string Body { get; private set; } = null!;
    public IReadOnlyCollection<SupportTicketAttachment> Attachments => _attachments.AsReadOnly();

    private SupportTicketMessage() { }

    public SupportTicketMessage(User sender, string body, IEnumerable<SupportTicketAttachment>? attachments = null, User? representedUser = null)
    {
        SenderUser = sender ?? throw new BusinessRuleViolationException("Invalid sender", "Sender is required");
        SenderUserId = sender.Id;
        SenderRole = sender.Role;
        RepresentedUser = representedUser;
        RepresentedUserId = representedUser?.Id;
        Body = NormalizeBody(body);

        if (attachments is not null)
            _attachments.AddRange(attachments);
    }

    private static string NormalizeBody(string body)
    {
        var normalized = body?.Trim() ?? string.Empty;
        if (normalized.Length < 2 || normalized.Length > 4000)
            throw new BusinessRuleViolationException("Invalid ticket message", "Message must be between 2 and 4000 characters");

        return normalized;
    }
}
