namespace Randevoo.Application.Features.SupportTickets.Common;

public record SupportTicketAttachmentInput(string FileName, string ContentType, long SizeBytes, string Url);
