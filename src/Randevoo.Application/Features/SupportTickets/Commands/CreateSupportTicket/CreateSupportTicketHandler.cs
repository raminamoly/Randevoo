using MediatR;
using Randevoo.Application.Features.SupportTickets.Common;
using Randevoo.Application.Interfaces.Auditing;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.SupportTickets.Commands.CreateSupportTicket;

public class CreateSupportTicketHandler : IRequestHandler<CreateSupportTicketCommand, SupportTicketDetailDto>
{
    private readonly IUserRepository _users;
    private readonly ISupportTicketRepository _tickets;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;

    public CreateSupportTicketHandler(IUserRepository users, ISupportTicketRepository tickets, IUnitOfWork unitOfWork, IAuditLogger auditLogger)
    {
        _users = users;
        _tickets = tickets;
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
    }

    public async Task<SupportTicketDetailDto> Handle(CreateSupportTicketCommand request, CancellationToken cancellationToken)
    {
        var submitter = await _users.GetByIdAsync(request.SubmitterUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.SubmitterUserId);
        var assignee = await _tickets.GetNextRoundRobinAssigneeAsync(cancellationToken);
        var attachments = request.Attachments.Select(item => new SupportTicketAttachment(item.FileName, item.ContentType, item.SizeBytes, item.Url)).ToList();
        var firstMessage = new SupportTicketMessage(submitter, request.Body, attachments);
        var ticket = new SupportTicket(submitter, request.Title, request.Category, firstMessage, assignee);

        await _tickets.AddAsync(ticket, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _auditLogger.TryLogAsync(new AuditLogEntry(submitter.Id, "TicketCreated", nameof(SupportTicket), ticket.Id.ToString(), ActorRole: submitter.Role.ToString(), LogType: "support", Module: "support", Status: "success"), cancellationToken);
        if (assignee is not null)
            await _auditLogger.TryLogAsync(new AuditLogEntry(submitter.Id, "TicketAssigned", nameof(SupportTicket), ticket.Id.ToString(), ActorRole: submitter.Role.ToString(), LogType: "support", Module: "support", Status: "success", MetadataJson: $"{{\"assigneeUserId\":{assignee.Id}}}"), cancellationToken);

        var saved = await _tickets.GetByIdWithDetailsAsync(ticket.Id, cancellationToken) ?? ticket;
        return SupportTicketDtoMapper.ToDetail(saved);
    }
}
