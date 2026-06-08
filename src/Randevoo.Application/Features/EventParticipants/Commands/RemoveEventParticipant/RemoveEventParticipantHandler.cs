using MediatR;
using Microsoft.Extensions.Logging;
using Randevoo.Application.Interfaces.Auditing;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.EventParticipants.Commands.RemoveEventParticipant;

public class RemoveEventParticipantHandler : IRequestHandler<RemoveEventParticipantCommand>
{
    private readonly IUserRepository _users;
    private readonly IDatingEventRepository _events;
    private readonly IEventTicketRepository _tickets;
    private readonly IBalanceAccountRepository _balances;
    private readonly IEventConversationRepository _conversations;
    private readonly IModerationReportRepository _reports;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;
    private readonly ILogger<RemoveEventParticipantHandler> _logger;

    public RemoveEventParticipantHandler(
        IUserRepository users,
        IDatingEventRepository events,
        IEventTicketRepository tickets,
        IBalanceAccountRepository balances,
        IEventConversationRepository conversations,
        IModerationReportRepository reports,
        IUnitOfWork unitOfWork,
        IAuditLogger auditLogger,
        ILogger<RemoveEventParticipantHandler> logger)
    {
        _users = users;
        _events = events;
        _tickets = tickets;
        _balances = balances;
        _conversations = conversations;
        _reports = reports;
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
        _logger = logger;
    }

    public async Task Handle(RemoveEventParticipantCommand request, CancellationToken cancellationToken)
    {
        var actor = await _users.GetByIdAsync(request.ActorUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.ActorUserId);
        var datingEvent = await _events.GetByIdAsync(request.EventId, cancellationToken)
            ?? throw new NotFoundException("DatingEvent", request.EventId);

        if (datingEvent.EventPlannerUserId != actor.Id && actor.Role != UserRole.Admin)
            throw new BusinessRuleViolationException("Access denied", "Only owner or admin can remove participants");

        var ticket = await _tickets.GetByEventAndUserAsync(request.EventId, request.ParticipantUserId, cancellationToken)
            ?? throw new NotFoundException("EventTicket", request.ParticipantUserId);

        ticket.RemoveWithRefund(actor.Id, request.Reason);

        var balance = await _balances.GetByUserIdAsync(ticket.UserId, cancellationToken);
        if (balance is null)
        {
            balance = new BalanceAccount(ticket.User);
            await _balances.AddAsync(balance, cancellationToken);
        }

        balance.Credit(
            ticket.Price,
            BalanceTransactionType.EmergencyRemovalRefund,
            $"Emergency participant removal refund for {datingEvent.Title}: {request.Reason}",
            datingEvent.Id,
            nameof(EventTicket),
            ticket.Id,
            actor.Id,
            ticket.CurrencyCode,
            ticket.ReportingPriceIrr,
            ticket.ExchangeRateToIrr,
            ticket.ExchangeRateCapturedAtUtc,
            ticket.ExchangeRateId);

        var conversations = await _conversations.ListForEventUserAsync(request.EventId, request.ParticipantUserId, cancellationToken);
        foreach (var conversation in conversations)
        {
            conversation.Disable(actor.Id, request.Reason);
            await _conversations.UpdateAsync(conversation, cancellationToken);
        }

        var report = new ModerationReport(actor, ticket.User, ModerationReportReason.Other, request.Reason, request.EventId);
        report.Review(ModerationReportStatus.ActionTaken, actor.Id, "Emergency participant removal created by planner/admin.");
        await _reports.AddAsync(report, cancellationToken);

        await _auditLogger.LogAsync(new AuditLogEntry(
            actor.Id,
            "EventParticipantEmergencyRemoved",
            "DatingEvent",
            datingEvent.Id.ToString(),
            null,
            $"{{\"participantUserId\":{ticket.UserId},\"refundAmount\":{ticket.Price},\"currencyCode\":\"{ticket.CurrencyCode}\",\"reportingRefundAmountIrr\":{ticket.ReportingPriceIrr}}}",
            request.Reason), cancellationToken);

        await _tickets.UpdateAsync(ticket, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogWarning("Actor {ActorUserId} removed participant {ParticipantUserId} from event {EventId} with refund {RefundAmount}", actor.Id, ticket.UserId, datingEvent.Id, ticket.Price);
    }
}
