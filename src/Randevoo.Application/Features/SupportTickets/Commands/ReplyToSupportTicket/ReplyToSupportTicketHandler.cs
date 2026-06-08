using MediatR;
using Randevoo.Application.Features.SupportTickets.Common;
using Randevoo.Application.Interfaces.Auditing;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.SupportTickets.Commands.ReplyToSupportTicket;

public class ReplyToSupportTicketHandler : IRequestHandler<ReplyToSupportTicketCommand, SupportTicketDetailDto>
{
    private readonly IUserRepository _users;
    private readonly ISupportTicketRepository _tickets;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;

    public ReplyToSupportTicketHandler(IUserRepository users, ISupportTicketRepository tickets, IUnitOfWork unitOfWork, IAuditLogger auditLogger)
    {
        _users = users;
        _tickets = tickets;
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
    }

    public async Task<SupportTicketDetailDto> Handle(ReplyToSupportTicketCommand request, CancellationToken cancellationToken)
    {
        var actor = await _users.GetByIdAsync(request.ActorUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.ActorUserId);
        var ticket = await _tickets.GetByIdWithDetailsAsync(request.TicketId, cancellationToken)
            ?? throw new NotFoundException("SupportTicket", request.TicketId);
        User? represented = null;
        if (request.RepresentedUserId is long representedUserId)
        {
            if (actor.Role != UserRole.Admin)
                throw new BusinessRuleViolationException("Access denied", "Only admin users can reply on behalf of another user");

            represented = await _users.GetByIdAsync(representedUserId, cancellationToken)
                ?? throw new NotFoundException("User", representedUserId);
        }

        var attachments = request.Attachments.Select(item => new SupportTicketAttachment(item.FileName, item.ContentType, item.SizeBytes, item.Url)).ToList();
        ticket.AddReply(actor, request.Body, attachments, represented);
        await _tickets.UpdateAsync(ticket, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _auditLogger.TryLogAsync(new AuditLogEntry(actor.Id, represented is null ? "ReplyAdded" : "AdminReplyOnBehalf", nameof(SupportTicket), ticket.Id.ToString(), ActorRole: actor.Role.ToString(), LogType: "support", Module: "support", Status: "success"), cancellationToken);
        return SupportTicketDtoMapper.ToDetail(ticket);
    }
}
