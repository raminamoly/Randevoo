using MediatR;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.DatingEvents.Commands.BuyDatingEventTicket;

public class BuyDatingEventTicketHandler : IRequestHandler<BuyDatingEventTicketCommand, long>
{
    private readonly IUserRepository _users;
    private readonly IUserProfileRepository _profiles;
    private readonly IBalanceAccountRepository _balances;
    private readonly IDatingEventRepository _events;
    private readonly IUnitOfWork _unitOfWork;

    public BuyDatingEventTicketHandler(IUserRepository users, IUserProfileRepository profiles, IBalanceAccountRepository balances, IDatingEventRepository events, IUnitOfWork unitOfWork)
    {
        _users = users;
        _profiles = profiles;
        _balances = balances;
        _events = events;
        _unitOfWork = unitOfWork;
    }

    public async Task<long> Handle(BuyDatingEventTicketCommand request, CancellationToken cancellationToken)
    {
        var buyer = await _users.GetByIdAsync(request.BuyerUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.BuyerUserId);
        var profile = await _profiles.GetByUserIdAsync(request.BuyerUserId, cancellationToken)
            ?? throw new BusinessRuleViolationException("Profile required", "End user must complete a profile before buying tickets");
        var datingEvent = await _events.GetByIdWithTicketsAsync(request.EventId, cancellationToken)
            ?? throw new NotFoundException("DatingEvent", request.EventId);
        var buyerBalance = await _balances.GetByUserIdAsync(request.BuyerUserId, cancellationToken)
            ?? throw new BusinessRuleViolationException("Balance required", "User does not have a balance account");
        var plannerBalance = await _balances.GetByUserIdAsync(datingEvent.EventPlannerUserId, cancellationToken);
        var isNewPlannerBalance = plannerBalance is null;
        if (plannerBalance is null)
        {
            plannerBalance = new BalanceAccount(datingEvent.EventPlannerUser);
            await _balances.AddAsync(plannerBalance, cancellationToken);
        }

        var ticket = datingEvent.SellTicket(buyer, profile);
        buyerBalance.Debit(ticket.Price, BalanceTransactionType.TicketPurchase, $"Ticket purchase for {datingEvent.Title}", datingEvent.Id);
        var plannerIncome = ticket.Price * (100 - datingEvent.EventPlannerCommissionPercent) / 100;
        plannerBalance.Credit(plannerIncome, BalanceTransactionType.EventPlannerIncome, $"Ticket income for {datingEvent.Title}", datingEvent.Id);

        await _events.UpdateAsync(datingEvent, cancellationToken);
        await _balances.UpdateAsync(buyerBalance, cancellationToken);
        if (!isNewPlannerBalance)
            await _balances.UpdateAsync(plannerBalance, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ticket.Id;
    }
}
