using MediatR;
using Microsoft.Extensions.Logging;
using Randevoo.Application.Interfaces.Auditing;
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
    private readonly IEventDiscountCodeRepository _discountCodes;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;
    private readonly ILogger<BuyDatingEventTicketHandler> _logger;

    public BuyDatingEventTicketHandler(
        IUserRepository users,
        IUserProfileRepository profiles,
        IBalanceAccountRepository balances,
        IDatingEventRepository events,
        IEventDiscountCodeRepository discountCodes,
        IUnitOfWork unitOfWork,
        IAuditLogger auditLogger,
        ILogger<BuyDatingEventTicketHandler> logger)
    {
        _users = users;
        _profiles = profiles;
        _balances = balances;
        _events = events;
        _discountCodes = discountCodes;
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
        _logger = logger;
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

        var basePrice = datingEvent.GetTicketPriceForGender(profile.Gender);
        var discountCode = await ResolveDiscountCodeAsync(datingEvent.Id, request.DiscountCode, cancellationToken);
        decimal? discountedPrice = null;
        if (discountCode is not null)
        {
            discountCode.EnsureCanUse(datingEvent.Id, profile.Gender, DateTime.UtcNow, basePrice);
            discountedPrice = discountCode.CalculateDiscountedPrice(basePrice);
        }

        var ticket = datingEvent.SellTicket(buyer, profile, discountedPrice, discountCode);
        if (discountCode is not null)
        {
            discountCode.RegisterUsage(DateTime.UtcNow);
            await _discountCodes.UpdateAsync(discountCode, cancellationToken);
        }

        buyerBalance.Debit(ticket.Price, BalanceTransactionType.TicketPurchase, $"Ticket purchase for {datingEvent.Title}", datingEvent.Id);
        var plannerIncome = ticket.Price * (100 - datingEvent.EventPlannerCommissionPercent) / 100;
        plannerBalance.Credit(plannerIncome, BalanceTransactionType.EventPlannerIncome, $"Ticket income for {datingEvent.Title}", datingEvent.Id);

        await _events.UpdateAsync(datingEvent, cancellationToken);
        await _balances.UpdateAsync(buyerBalance, cancellationToken);
        if (!isNewPlannerBalance)
            await _balances.UpdateAsync(plannerBalance, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _auditLogger.TryLogAsync(new AuditLogEntry(
            ActorUserId: buyer.Id,
            Action: "DatingEventTicketPurchased",
            TargetType: "DatingEvent",
            TargetId: datingEvent.Id.ToString(),
            LogType: "purchase",
            Module: "events",
            Description: $"User purchased a ticket for {datingEvent.Title}.",
            Status: "success",
            MetadataJson: $$"""{"ticketId":{{ticket.Id}},"amount":{{ticket.Price}},"originalPrice":{{basePrice}},"discountCode":"{{discountCode?.Code}}","discountAmount":{{(basePrice - ticket.Price)}}}"""), cancellationToken);
        _logger.LogInformation("User {BuyerUserId} bought ticket {TicketId} for event {EventId} at price {TicketPrice}", buyer.Id, ticket.Id, datingEvent.Id, ticket.Price);
        return ticket.Id;
    }

    private async Task<EventDiscountCode?> ResolveDiscountCodeAsync(long eventId, string? code, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        return await _discountCodes.GetApplicableByCodeAsync(eventId, code, cancellationToken)
            ?? throw new BusinessRuleViolationException("Discount code not found", "Discount code is not valid for this event.");
    }
}
