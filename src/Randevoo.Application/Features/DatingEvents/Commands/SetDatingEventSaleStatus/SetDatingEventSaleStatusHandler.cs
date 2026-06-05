using System.Text.Json;
using MediatR;
using Randevoo.Application.Interfaces.Auditing;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.DatingEvents.Commands.SetDatingEventSaleStatus;

public class SetDatingEventSaleStatusHandler : IRequestHandler<SetDatingEventSaleStatusCommand>
{
    private readonly IUserRepository _users;
    private readonly IDatingEventRepository _events;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;

    public SetDatingEventSaleStatusHandler(IUserRepository users, IDatingEventRepository events, IUnitOfWork unitOfWork, IAuditLogger auditLogger)
    {
        _users = users;
        _events = events;
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
    }

    public async Task Handle(SetDatingEventSaleStatusCommand request, CancellationToken cancellationToken)
    {
        var actor = await _users.GetByIdAsync(request.ActorUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.ActorUserId);
        var datingEvent = await _events.GetByIdAsync(request.EventId, cancellationToken)
            ?? throw new NotFoundException("DatingEvent", request.EventId);

        if (datingEvent.EventPlannerUserId != actor.Id && actor.Role != UserRole.Admin)
            throw new BusinessRuleViolationException("Access denied", "Only owner or admin can change event sale status");

        var beforeSnapshot = CreateSnapshot(datingEvent);
        if (request.Open) datingEvent.OpenForSell(); else datingEvent.CloseForSell();

        await _auditLogger.LogAsync(new AuditLogEntry(
            actor.Id,
            request.Open ? "EventSaleOpened" : "EventSaleClosed",
            "DatingEvent",
            datingEvent.Id.ToString(),
            JsonSerializer.Serialize(beforeSnapshot),
            JsonSerializer.Serialize(CreateSnapshot(datingEvent)),
            request.Open ? "فروش رویداد باز شد." : "فروش رویداد بسته شد."), cancellationToken);

        await _events.UpdateAsync(datingEvent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
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
