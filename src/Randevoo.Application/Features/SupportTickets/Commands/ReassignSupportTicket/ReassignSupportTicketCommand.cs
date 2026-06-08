using MediatR;
using Randevoo.Application.Features.SupportTickets.Common;

namespace Randevoo.Application.Features.SupportTickets.Commands.ReassignSupportTicket;

public record ReassignSupportTicketCommand(long ActorUserId, long TicketId, long? AssigneeUserId, string? Note) : IRequest<SupportTicketDetailDto>;
