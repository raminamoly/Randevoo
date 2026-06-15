using MediatR;
using Randevoo.Application.Features.SupportTickets.Common;
using Randevoo.Application.Interfaces.Auditing;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.SupportTickets.Commands.ChangeSupportTicketStatus;

public class ChangeSupportTicketStatusHandler : IRequestHandler<ChangeSupportTicketStatusCommand, SupportTicketDetailDto>
{
    private readonly IUserRepository _users;
    private readonly ISupportTicketRepository _tickets;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;

    public ChangeSupportTicketStatusHandler(IUserRepository users, ISupportTicketRepository tickets, IUnitOfWork unitOfWork, IAuditLogger auditLogger)
    {
        _users = users;
        _tickets = tickets;
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
    }

    public async Task<SupportTicketDetailDto> Handle(ChangeSupportTicketStatusCommand request, CancellationToken cancellationToken)
    {
        var actor = await _users.GetByIdAsync(request.ActorUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.ActorUserId);
        var ticket = await _tickets.GetByIdWithDetailsAsync(request.TicketId, cancellationToken)
            ?? throw new NotFoundException("SupportTicket", request.TicketId);
        if (!await _tickets.IsTicketStatusActiveAsync(request.TicketStatusId, cancellationToken))
            throw new BusinessRuleViolationException("Invalid ticket status", "Ticket status is inactive or invalid");

        ticket.ChangeStatus(actor, request.TicketStatusId, request.Note);
        await _tickets.UpdateAsync(ticket, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _auditLogger.TryLogAsync(new AuditLogEntry(actor.Id, "StatusChanged", nameof(ticket), ticket.Id.ToString(), ActorRole: actor.Role.ToString(), LogType: "support", Module: "support", Status: "success"), cancellationToken);
        return SupportTicketDtoMapper.ToDetail(ticket);
    }
}
