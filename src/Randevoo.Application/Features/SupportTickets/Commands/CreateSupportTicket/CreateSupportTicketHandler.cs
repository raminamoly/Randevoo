using MediatR;
using Randevoo.Application.Features.SupportTickets.Common;
using Randevoo.Application.Interfaces.Auditing;
using Randevoo.Domain.Constants;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.SupportTickets.Commands.CreateSupportTicket;

public class CreateSupportTicketHandler : IRequestHandler<CreateSupportTicketCommand, SupportTicketDetailDto>
{
    private readonly IUserRepository _users;
    private readonly ISupportTicketRepository _tickets;
    private readonly IDatingEventRepository _events;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;

    public CreateSupportTicketHandler(IUserRepository users, ISupportTicketRepository tickets, IDatingEventRepository events, IUnitOfWork unitOfWork, IAuditLogger auditLogger)
    {
        _users = users;
        _tickets = tickets;
        _events = events;
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
    }

    public async Task<SupportTicketDetailDto> Handle(CreateSupportTicketCommand request, CancellationToken cancellationToken)
    {
        var submitter = await _users.GetByIdAsync(request.SubmitterUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.SubmitterUserId);
        if (!await _tickets.IsTicketTypeActiveAsync(request.TicketTypeId, cancellationToken))
            throw new BusinessRuleViolationException("Invalid ticket type", "Ticket type is inactive or invalid");
        if (!await _tickets.IsTicketRecipientTypeActiveAsync(request.TicketRecipientTypeId, cancellationToken))
            throw new BusinessRuleViolationException("Invalid ticket recipient", "Ticket recipient is inactive or invalid");

        User? assignee = null;
        DatingEvent? datingEvent = null;
        User? recipientPlanner = null;
        if (request.TicketRecipientTypeId == SupportTicketLookupIds.RecipientPlatformSupport)
        {
            assignee = await _tickets.GetNextRoundRobinAssigneeAsync(cancellationToken);
            if (request.EventId is long platformEventId)
                datingEvent = await _events.GetByIdAsync(platformEventId, cancellationToken);
        }
        else if (request.TicketRecipientTypeId == SupportTicketLookupIds.RecipientEventPlanner)
        {
            if (request.EventId is null)
                throw new BusinessRuleViolationException("Event required", "Organizer tickets must be linked to an event");

            datingEvent = await _events.GetByIdAsync(request.EventId.Value, cancellationToken)
                ?? throw new NotFoundException("DatingEvent", request.EventId.Value);
            recipientPlanner = datingEvent.EventPlannerUser;
            if (submitter.Role == UserRole.EventPlanner && submitter.Id == datingEvent.EventPlannerUserId)
                throw new BusinessRuleViolationException("Invalid recipient", "Planner cannot submit an organizer ticket to their own event");
        }

        var attachments = request.Attachments.Select(item => new SupportTicketAttachment(item.FileName, item.ContentType, item.SizeBytes, item.Url)).ToList();
        var firstMessage = new SupportTicketMessage(submitter, request.Body, attachments);
        var ticket = new SupportTicket(submitter, request.Title, request.TicketTypeId, request.TicketRecipientTypeId, firstMessage, assignee, datingEvent, recipientPlanner);

        await _tickets.AddAsync(ticket, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _auditLogger.TryLogAsync(new AuditLogEntry(submitter.Id, "TicketCreated", nameof(SupportTicket), ticket.Id.ToString(), ActorRole: submitter.Role.ToString(), LogType: "support", Module: "support", Status: "success", MetadataJson: $"{{\"ticketTypeId\":{ticket.TicketTypeId},\"recipientTypeId\":{ticket.TicketRecipientTypeId}}}"), cancellationToken);
        if (assignee is not null)
            await _auditLogger.TryLogAsync(new AuditLogEntry(submitter.Id, "TicketAssigned", nameof(SupportTicket), ticket.Id.ToString(), ActorRole: submitter.Role.ToString(), LogType: "support", Module: "support", Status: "success", MetadataJson: $"{{\"assigneeUserId\":{assignee.Id}}}"), cancellationToken);
        if (recipientPlanner is not null)
            await _auditLogger.TryLogAsync(new AuditLogEntry(submitter.Id, "TicketSentToPlanner", nameof(SupportTicket), ticket.Id.ToString(), ActorRole: submitter.Role.ToString(), LogType: "support", Module: "support", Status: "success", MetadataJson: $"{{\"plannerUserId\":{recipientPlanner.Id},\"eventId\":{datingEvent?.Id ?? 0}}}"), cancellationToken);

        var saved = await _tickets.GetByIdWithDetailsAsync(ticket.Id, cancellationToken) ?? ticket;
        return SupportTicketDtoMapper.ToDetail(saved);
    }
}
