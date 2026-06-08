using MediatR;
using Randevoo.Application.Features.SupportTickets.Common;

namespace Randevoo.Application.Features.SupportTickets.Commands.ReplyToSupportTicket;

public record ReplyToSupportTicketCommand(
    long ActorUserId,
    long TicketId,
    string Body,
    IReadOnlyList<SupportTicketAttachmentInput> Attachments,
    long? RepresentedUserId = null) : IRequest<SupportTicketDetailDto>;
