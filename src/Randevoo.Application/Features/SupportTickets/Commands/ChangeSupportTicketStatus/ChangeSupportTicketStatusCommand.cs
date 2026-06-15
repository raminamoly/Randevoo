using MediatR;
using Randevoo.Application.Features.SupportTickets.Common;

namespace Randevoo.Application.Features.SupportTickets.Commands.ChangeSupportTicketStatus;

public record ChangeSupportTicketStatusCommand(long ActorUserId, long TicketId, long TicketStatusId, string? Note) : IRequest<SupportTicketDetailDto>;
