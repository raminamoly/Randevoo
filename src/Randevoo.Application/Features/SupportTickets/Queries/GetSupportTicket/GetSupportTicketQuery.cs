using MediatR;
using Randevoo.Application.Features.SupportTickets.Common;

namespace Randevoo.Application.Features.SupportTickets.Queries.GetSupportTicket;

public record GetSupportTicketQuery(long RequesterUserId, long TicketId) : IRequest<SupportTicketDetailDto>;
