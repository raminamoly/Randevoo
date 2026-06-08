using MediatR;
using Randevoo.Application.Features.SupportTickets.Common;
using Randevoo.Domain.Enums;

namespace Randevoo.Application.Features.SupportTickets.Commands.ChangeSupportTicketStatus;

public record ChangeSupportTicketStatusCommand(long ActorUserId, long TicketId, SupportTicketStatus Status, string? Note) : IRequest<SupportTicketDetailDto>;
