using MediatR;
using Microsoft.Extensions.Logging;
using Randevoo.Application.Interfaces.Auditing;
using Randevoo.Application.Interfaces.Currencies;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.DatingEvents.Commands.BuyDatingEventTicket;

public class BuyDatingEventTicketHandler : IRequestHandler<BuyDatingEventTicketCommand, TicketOrderPurchaseResult>
{
    private readonly IUserRepository _users;
    private readonly IUserProfileRepository _profiles;
    private readonly IBalanceAccountRepository _balances;
    private readonly IDatingEventRepository _events;
    private readonly IEventDiscountCodeRepository _discountCodes;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;
    private readonly ICurrencyExchangeRateProvider _exchangeRates;
    private readonly ILogger<BuyDatingEventTicketHandler> _logger;

    public BuyDatingEventTicketHandler(
        IUserRepository users,
        IUserProfileRepository profiles,
        IBalanceAccountRepository balances,
        IDatingEventRepository events,
        IEventDiscountCodeRepository discountCodes,
        IUnitOfWork unitOfWork,
        IAuditLogger auditLogger,
        ICurrencyExchangeRateProvider exchangeRates,
        ILogger<BuyDatingEventTicketHandler> logger)
    {
        _users = users;
        _profiles = profiles;
        _balances = balances;
        _events = events;
        _discountCodes = discountCodes;
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
        _exchangeRates = exchangeRates;
        _logger = logger;
    }

    public async Task<TicketOrderPurchaseResult> Handle(BuyDatingEventTicketCommand request, CancellationToken cancellationToken)
    {
        var buyer = await _users.GetByIdAsync(request.BuyerUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.BuyerUserId);
        var participantUserId = request.ParticipantUserId ?? request.BuyerUserId;
        var participant = participantUserId == buyer.Id
            ? buyer
            : await _users.GetByIdAsync(participantUserId, cancellationToken) ?? throw new NotFoundException("User", participantUserId);
        var profile = await _profiles.GetByUserIdAsync(participantUserId, cancellationToken)
            ?? throw new BusinessRuleViolationException("Profile required", "Participant must complete a profile before buying tickets");
        var datingEvent = await _events.GetByIdWithTicketsAsync(request.EventId, cancellationToken)
            ?? throw new NotFoundException("DatingEvent", request.EventId);
        var buyerBalance = await _balances.GetByUserIdAsync(request.BuyerUserId, cancellationToken);

        var basePrice = datingEvent.GetTicketPriceForGender(profile.Gender);
        var discountCode = await ResolveDiscountCodeAsync(datingEvent.Id, request.DiscountCode, cancellationToken);
        decimal? discountedPrice = null;
        if (discountCode is not null)
        {
            discountCode.EnsureCanUse(datingEvent.Id, profile.Gender, DateTime.UtcNow, basePrice);
            discountedPrice = discountCode.CalculateDiscountedPrice(basePrice);
        }

        var finalPrice = discountedPrice ?? basePrice;
        var currencyCode = datingEvent.GetTicketCurrencyForGender(profile.Gender);
        var exchangeRate = await _exchangeRates.GetActiveRateToIrrAsync(currencyCode, DateTime.UtcNow, cancellationToken);
        var platformCommission = finalPrice * datingEvent.EventPlannerCommissionPercent / 100;
        var order = new TicketOrder(
            datingEvent,
            buyer,
            basePrice,
            basePrice - finalPrice,
            finalPrice,
            platformCommission,
            datingEvent.PaymentCollectionMethod,
            currencyCode,
            exchangeRate.Rate,
            exchangeRate.CapturedAtUtc,
            exchangeRate.ExchangeRateId,
            discountCode,
            TicketOrderPaymentStatus.Paid,
            TicketOrderStatus.Confirmed);
        var ticket = datingEvent.SellTicket(order, participant, profile, finalPrice, discountCode);
        ticket.CaptureExchangeRate(exchangeRate.Rate, exchangeRate.CapturedAtUtc, exchangeRate.ExchangeRateId);
        if (discountCode is not null)
        {
            discountCode.RegisterUsage(DateTime.UtcNow);
            await _discountCodes.UpdateAsync(discountCode, cancellationToken);
        }

        var plannerIncome = ticket.Price - platformCommission;
        var platformCommissionIrr = ConvertToIrr(platformCommission, exchangeRate.Rate);
        var plannerIncomeIrr = ConvertToIrr(plannerIncome, exchangeRate.Rate);
        var paidAmountIrr = ticket.ReportingPriceIrr;

        if (datingEvent.PaymentCollectionMethod != EventPaymentCollectionMethod.OrganizerManualTransfer)
        {
            if (buyerBalance is null)
                throw new BusinessRuleViolationException("Balance required", "User does not have a balance account");

            buyerBalance.Debit(
                ticket.Price,
                BalanceTransactionType.TicketPurchase,
                $"Ticket purchase for {datingEvent.Title}",
                datingEvent.Id,
                nameof(EventTicket),
                ticket.Id,
                buyer.Id,
                ticket.CurrencyCode,
                paidAmountIrr,
                exchangeRate.Rate,
                exchangeRate.CapturedAtUtc,
                exchangeRate.ExchangeRateId,
                order);
        }

        await _events.UpdateAsync(datingEvent, cancellationToken);
        if (buyerBalance is not null)
            await _balances.UpdateAsync(buyerBalance, cancellationToken);
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
            MetadataJson: $$"""{"orderId":{{order.Id}},"ticketId":{{ticket.Id}},"buyerUserId":{{buyer.Id}},"participantUserId":{{participant.Id}},"amount":{{ticket.Price}},"currencyCode":"{{ticket.CurrencyCode}}","reportingAmountIrr":{{ticket.ReportingPriceIrr}},"exchangeRateToIrr":{{exchangeRate.Rate}},"exchangeRateId":{{exchangeRate.ExchangeRateId}},"originalPrice":{{basePrice}},"discountCode":"{{discountCode?.Code}}","discountAmount":{{(basePrice - ticket.Price)}},"paymentCollectionMethod":"{{datingEvent.PaymentCollectionMethod}}","platformCommission":{{platformCommission}},"platformCommissionIrr":{{platformCommissionIrr}},"plannerIncome":{{plannerIncome}},"plannerIncomeIrr":{{plannerIncomeIrr}}}"""), cancellationToken);
        _logger.LogInformation("User {BuyerUserId} bought ticket {TicketId} for participant {ParticipantUserId} and event {EventId} at price {TicketPrice}", buyer.Id, ticket.Id, participant.Id, datingEvent.Id, ticket.Price);
        return new TicketOrderPurchaseResult(order.Id, new[] { ticket.Id });
    }

    private static decimal ConvertToIrr(decimal amount, decimal rate)
        => Math.Round(amount * rate, 0, MidpointRounding.AwayFromZero);

    private async Task<EventDiscountCode?> ResolveDiscountCodeAsync(long eventId, string? code, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        return await _discountCodes.GetApplicableByCodeAsync(eventId, code, cancellationToken)
            ?? throw new BusinessRuleViolationException("Discount code not found", "Discount code is not valid for this event.");
    }
}
