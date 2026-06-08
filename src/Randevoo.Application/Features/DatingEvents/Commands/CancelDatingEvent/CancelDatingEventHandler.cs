using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using Randevoo.Application.Interfaces.Auditing;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.DatingEvents.Commands.CancelDatingEvent;

public class CancelDatingEventHandler : IRequestHandler<CancelDatingEventCommand>
{
    private readonly IUserRepository _users;
    private readonly IBalanceAccountRepository _balances;
    private readonly IDatingEventRepository _events;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;
    private readonly ILogger<CancelDatingEventHandler> _logger;

    public CancelDatingEventHandler(IUserRepository users, IBalanceAccountRepository balances, IDatingEventRepository events, IUnitOfWork unitOfWork, IAuditLogger auditLogger, ILogger<CancelDatingEventHandler> logger)
    {
        _users = users;
        _balances = balances;
        _events = events;
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
        _logger = logger;
    }

    public async Task Handle(CancelDatingEventCommand request, CancellationToken cancellationToken)
    {
        var actor = await _users.GetByIdAsync(request.ActorUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.ActorUserId);
        var datingEvent = await _events.GetByIdWithTicketsAsync(request.EventId, cancellationToken)
            ?? throw new NotFoundException("DatingEvent", request.EventId);

        if (datingEvent.EventPlannerUserId != actor.Id && actor.Role != UserRole.Admin)
            throw new BusinessRuleViolationException("Access denied", "Only owner or admin can cancel event");

        var beforeSnapshot = CreateSnapshot(datingEvent);
        var tickets = datingEvent.Cancel();
        var refundCount = 0;
        var refundTotal = 0m;
        var refundTotalIrr = 0m;
        foreach (var ticket in tickets)
        {
            var balance = await _balances.GetByUserIdAsync(ticket.UserId, cancellationToken);
            if (balance is null)
                continue;

            balance.Credit(
                ticket.Price,
                BalanceTransactionType.TicketRefund,
                $"Refund for {datingEvent.Title}",
                datingEvent.Id,
                nameof(EventTicket),
                ticket.Id,
                actor.Id,
                ticket.CurrencyCode,
                ticket.ReportingPriceIrr,
                ticket.ExchangeRateToIrr,
                ticket.ExchangeRateCapturedAtUtc,
                ticket.ExchangeRateId);
            refundCount++;
            refundTotal += ticket.Price;
            refundTotalIrr += ticket.ReportingPriceIrr;
        }

        await _auditLogger.LogAsync(new AuditLogEntry(
            actor.Id,
            "EventCancelled",
            "DatingEvent",
            datingEvent.Id.ToString(),
            JsonSerializer.Serialize(beforeSnapshot),
            JsonSerializer.Serialize(new { Event = CreateSnapshot(datingEvent), refundCount, refundTotal, refundTotalIrr }),
            "رویداد لغو شد و بلیت‌های معتبر برگشت خوردند."), cancellationToken);

        await _events.UpdateAsync(datingEvent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogWarning("Actor {ActorUserId} cancelled event {EventId}; refunded {RefundCount} tickets totaling {RefundTotal}", actor.Id, datingEvent.Id, refundCount, refundTotal);
    }

    private static object CreateSnapshot(Randevoo.Domain.Entities.DatingEvent datingEvent) => new
    {
        datingEvent.Id,
        datingEvent.Title,
        datingEvent.ReviewStatus,
        datingEvent.IsOpenForSell,
        datingEvent.IsCancelled,
        OperationalStatus = datingEvent.ResolveOperationalStatus(DateTime.UtcNow)
    };
}
