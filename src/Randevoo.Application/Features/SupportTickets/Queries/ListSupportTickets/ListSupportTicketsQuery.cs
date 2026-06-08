using MediatR;
using Randevoo.Application.Features.SupportTickets.Common;
using Randevoo.Domain.Enums;

namespace Randevoo.Application.Features.SupportTickets.Queries.ListSupportTickets;

public record ListSupportTicketsQuery(
    long RequesterUserId,
    SupportTicketStatus? Status,
    SupportTicketCategory? Category,
    UserRole? SubmitterRole,
    long? AssigneeUserId,
    DateTime? CreatedFromUtc,
    DateTime? CreatedToUtc,
    int Limit) : IRequest<IReadOnlyList<SupportTicketListItemDto>>;
