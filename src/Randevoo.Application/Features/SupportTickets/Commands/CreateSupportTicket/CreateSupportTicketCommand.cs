using MediatR;
using Randevoo.Application.Features.SupportTickets.Common;

namespace Randevoo.Application.Features.SupportTickets.Commands.CreateSupportTicket;

public record CreateSupportTicketCommand(
    long SubmitterUserId,
    string Title,
    long TicketTypeId,
    long TicketRecipientTypeId,
    long? EventId,
    string Body,
    IReadOnlyList<SupportTicketAttachmentInput> Attachments) : IRequest<SupportTicketDetailDto>;
