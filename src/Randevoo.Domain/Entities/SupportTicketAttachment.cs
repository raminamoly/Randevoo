using Randevoo.Domain.Common;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class SupportTicketAttachment : BaseEntity
{
    public long SupportTicketMessageId { get; private set; }
    public SupportTicketMessage Message { get; private set; } = null!;
    public string FileName { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public long SizeBytes { get; private set; }
    public string Url { get; private set; } = null!;

    private SupportTicketAttachment() { }

    public SupportTicketAttachment(string fileName, string contentType, long sizeBytes, string url)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new BusinessRuleViolationException("Invalid attachment", "File name is required");
        if (string.IsNullOrWhiteSpace(contentType) || !contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            throw new BusinessRuleViolationException("Invalid attachment", "Only image attachments are supported");
        if (sizeBytes <= 0 || sizeBytes > 5 * 1024 * 1024)
            throw new BusinessRuleViolationException("Invalid attachment", "Image size must be between 1 byte and 5 MB");
        if (string.IsNullOrWhiteSpace(url))
            throw new BusinessRuleViolationException("Invalid attachment", "Attachment URL is required");

        FileName = fileName.Trim();
        ContentType = contentType.Trim();
        SizeBytes = sizeBytes;
        Url = url.Trim();
    }
}
