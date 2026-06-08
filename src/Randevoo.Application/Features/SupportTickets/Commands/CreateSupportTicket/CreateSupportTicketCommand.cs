using MediatR;
using Randevoo.Application.Features.SupportTickets.Common;
using Randevoo.Domain.Enums;

namespace Randevoo.Application.Features.SupportTickets.Commands.CreateSupportTicket;

public record CreateSupportTicketCommand(
    long SubmitterUserId,
    string Title,
    SupportTicketCategory Category,
    string Body,
    IReadOnlyList<SupportTicketAttachmentInput> Attachments) : IRequest<SupportTicketDetailDto>;
