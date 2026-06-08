using MediatR;
using Randevoo.Application.Features.SupportTickets.Common;
using Randevoo.Application.Interfaces.Auditing;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.SupportTickets.Commands.ReassignSupportTicket;

public class ReassignSupportTicketHandler : IRequestHandler<ReassignSupportTicketCommand, SupportTicketDetailDto>
{
    private readonly IUserRepository _users;
    private readonly ISupportTicketRepository _tickets;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;

    public ReassignSupportTicketHandler(IUserRepository users, ISupportTicketRepository tickets, IUnitOfWork unitOfWork, IAuditLogger auditLogger)
    {
        _users = users;
        _tickets = tickets;
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
    }

    public async Task<SupportTicketDetailDto> Handle(ReassignSupportTicketCommand request, CancellationToken cancellationToken)
    {
        var actor = await _users.GetByIdAsync(request.ActorUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.ActorUserId);
        var ticket = await _tickets.GetByIdWithDetailsAsync(request.TicketId, cancellationToken)
            ?? throw new NotFoundException("SupportTicket", request.TicketId);
        var assignee = request.AssigneeUserId is null ? null : await _users.GetByIdAsync(request.AssigneeUserId.Value, cancellationToken)
            ?? throw new NotFoundException("User", request.AssigneeUserId.Value);

        ticket.Reassign(actor, assignee, request.Note);
        await _tickets.UpdateAsync(ticket, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _auditLogger.TryLogAsync(new AuditLogEntry(actor.Id, "TicketReassigned", nameof(ticket), ticket.Id.ToString(), ActorRole: actor.Role.ToString(), LogType: "support", Module: "support", Status: "success"), cancellationToken);
        return SupportTicketDtoMapper.ToDetail(ticket);
    }
}
