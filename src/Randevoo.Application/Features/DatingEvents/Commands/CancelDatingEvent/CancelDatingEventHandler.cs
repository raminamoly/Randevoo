using MediatR;
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

    public CancelDatingEventHandler(IUserRepository users, IBalanceAccountRepository balances, IDatingEventRepository events, IUnitOfWork unitOfWork)
    {
        _users = users;
        _balances = balances;
        _events = events;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CancelDatingEventCommand request, CancellationToken cancellationToken)
    {
        var actor = await _users.GetByIdAsync(request.ActorUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.ActorUserId);
        var datingEvent = await _events.GetByIdWithTicketsAsync(request.EventId, cancellationToken)
            ?? throw new NotFoundException("DatingEvent", request.EventId);

        if (datingEvent.EventPlannerUserId != actor.Id && actor.Role != UserRole.Admin)
            throw new BusinessRuleViolationException("Access denied", "Only owner or admin can cancel event");

        var tickets = datingEvent.Cancel();
        foreach (var ticket in tickets)
        {
            var balance = await _balances.GetByUserIdAsync(ticket.UserId, cancellationToken);
            balance?.Credit(ticket.Price, BalanceTransactionType.TicketRefund, $"Refund for {datingEvent.Title}", datingEvent.Id);
        }

        await _events.UpdateAsync(datingEvent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
