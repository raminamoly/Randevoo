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
    private readonly IDatingEventRepository _events;
    private readonly IEventDiscountCodeRepository _discountCodes;
    private readonly IUserRestrictionRepository _restrictions;
    private readonly ITicketOrderRepository _ticketOrders;
    private readonly IManualPaymentReceiptRepository _manualReceipts;
    private readonly IOnlinePaymentRepository _onlinePayments;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogger _auditLogger;
    private readonly ICurrencyExchangeRateProvider _exchangeRates;
    private readonly ILogger<BuyDatingEventTicketHandler> _logger;

    public BuyDatingEventTicketHandler(
        IUserRepository users,
        IUserProfileRepository profiles,
        IDatingEventRepository events,
        IEventDiscountCodeRepository discountCodes,
        IUserRestrictionRepository restrictions,
        ITicketOrderRepository ticketOrders,
        IManualPaymentReceiptRepository manualReceipts,
        IOnlinePaymentRepository onlinePayments,
        IUnitOfWork unitOfWork,
        IAuditLogger auditLogger,
        ICurrencyExchangeRateProvider exchangeRates,
        ILogger<BuyDatingEventTicketHandler> logger)
    {
        _users = users;
        _profiles = profiles;
        _events = events;
        _discountCodes = discountCodes;
        _restrictions = restrictions;
        _ticketOrders = ticketOrders;
        _manualReceipts = manualReceipts;
        _onlinePayments = onlinePayments;
        _unitOfWork = unitOfWork;
        _auditLogger = auditLogger;
        _exchangeRates = exchangeRates;
        _logger = logger;
    }

    public async Task<TicketOrderPurchaseResult> Handle(BuyDatingEventTicketCommand request, CancellationToken cancellationToken)
    {
        var buyer = await _users.GetByIdAsync(request.BuyerUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.BuyerUserId);
        var participant = await ResolveParticipantAsync(request, buyer, cancellationToken);

        await EnsureTicketPurchaseAllowedAsync(buyer.Id, "Buyer", cancellationToken);
        if (participant.Id != buyer.Id)
            await EnsureTicketPurchaseAllowedAsync(participant.Id, "Participant", cancellationToken);

        var profile = await _profiles.GetByUserIdAsync(participant.Id, cancellationToken)
            ?? throw new BusinessRuleViolationException("Profile required", "Participant must complete a profile before buying tickets");
        var datingEvent = await _events.GetByIdWithTicketsAsync(request.EventId, cancellationToken)
            ?? throw new NotFoundException("DatingEvent", request.EventId);

        if (await _manualReceipts.HasSubmittedReceiptAsync(datingEvent.Id, participant.Id, cancellationToken))
            throw new BusinessRuleViolationException("Pending receipt exists", "There is already a pending manual receipt for this participant and event.");

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
            discountCode);

        await _ticketOrders.AddAsync(order, cancellationToken);

        EventTicket? ticket = null;
        ManualPaymentReceipt? receipt = null;
        OnlinePayment? onlinePayment = null;

        if (datingEvent.PaymentCollectionMethod == EventPaymentCollectionMethod.PlatformGateway)
        {
            order.MarkPaid();
            ticket = datingEvent.SellTicket(order, participant, profile, finalPrice, discountCode);
            ticket.CaptureExchangeRate(exchangeRate.Rate, exchangeRate.CapturedAtUtc, exchangeRate.ExchangeRateId);
            RegisterDiscountUsage(discountCode);

            onlinePayment = new OnlinePayment(
                buyer,
                finalPrice,
                "DevGateway",
                $"DEV-{Guid.NewGuid():N}"[..24],
                OnlinePaymentStatus.Succeeded,
                datingEvent,
                ticket,
                currencyCode: currencyCode,
                reportingAmountIrr: ConvertToIrr(finalPrice, exchangeRate.Rate),
                exchangeRateToIrr: exchangeRate.Rate,
                exchangeRateCapturedAtUtc: exchangeRate.CapturedAtUtc,
                exchangeRateId: exchangeRate.ExchangeRateId,
                ticketOrder: order);
            await _onlinePayments.AddAsync(onlinePayment, cancellationToken);
        }
        else
        {
            if (string.IsNullOrWhiteSpace(request.ManualReceiptFilePath))
                throw new BusinessRuleViolationException("Manual receipt required", "A receipt image or file is required for manual payment.");

            receipt = new ManualPaymentReceipt(
                datingEvent,
                participant,
                basePrice,
                finalPrice,
                currencyCode,
                datingEvent.PaymentCollectionMethod,
                request.ManualReceiptFilePath,
                request.ManualReceiptTrackingNumber,
                request.ManualReceiptNote,
                exchangeRate.Rate,
                exchangeRate.CapturedAtUtc,
                exchangeRate.ExchangeRateId,
                discountCode);
            receipt.LinkTicketOrder(order);
            await _manualReceipts.AddAsync(receipt, cancellationToken);
        }

        await _events.UpdateAsync(datingEvent, cancellationToken);
        if (discountCode is not null && ticket is not null)
            await _discountCodes.UpdateAsync(discountCode, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _auditLogger.TryLogAsync(new AuditLogEntry(
            ActorUserId: buyer.Id,
            Action: "DatingEventTicketCheckoutCreated",
            TargetType: "DatingEvent",
            TargetId: datingEvent.Id.ToString(),
            LogType: "purchase",
            Module: "events",
            Description: $"User created a ticket checkout for {datingEvent.Title}.",
            Status: "success",
            MetadataJson: $$"""{"orderId":{{order.Id}},"ticketId":{{ticket?.Id ?? 0}},"manualReceiptId":{{receipt?.Id ?? 0}},"onlinePaymentId":{{onlinePayment?.Id ?? 0}},"buyerUserId":{{buyer.Id}},"participantUserId":{{participant.Id}},"amount":{{finalPrice}},"currencyCode":"{{currencyCode}}","discountCode":"{{discountCode?.Code}}","discountAmount":{{(basePrice - finalPrice)}},"paymentCollectionMethod":"{{datingEvent.PaymentCollectionMethod}}","paymentStatus":"{{order.PaymentStatus}}","orderStatus":"{{order.OrderStatus}}"}"""), cancellationToken);

        _logger.LogInformation("User {BuyerUserId} created order {OrderId} for participant {ParticipantUserId} and event {EventId} with payment method {PaymentMethod}", buyer.Id, order.Id, participant.Id, datingEvent.Id, datingEvent.PaymentCollectionMethod);
        return new TicketOrderPurchaseResult(
            order.Id,
            ticket is null ? Array.Empty<long>() : new[] { ticket.Id },
            order.PaymentCollectionMethod,
            order.PaymentStatus,
            order.OrderStatus,
            receipt?.Id,
            onlinePayment?.Id,
            participant.Id,
            order.GrossAmount,
            order.DiscountAmount,
            order.NetAmount,
            order.CurrencyCode);
    }

    private async Task<User> ResolveParticipantAsync(BuyDatingEventTicketCommand request, User buyer, CancellationToken cancellationToken)
    {
        if (request.ParticipantUserId.HasValue)
        {
            return request.ParticipantUserId.Value == buyer.Id
                ? buyer
                : await _users.GetByIdAsync(request.ParticipantUserId.Value, cancellationToken)
                    ?? throw new NotFoundException("User", request.ParticipantUserId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.ParticipantMobileNumber))
        {
            var participant = await _users.GetByMobileNumberAsync(request.ParticipantMobileNumber.Trim(), cancellationToken);
            if (participant is null)
                throw new BusinessRuleViolationException("Participant not found", "No participant user was found with this mobile number.");

            return participant;
        }

        return buyer;
    }

    private static void RegisterDiscountUsage(EventDiscountCode? discountCode)
    {
        discountCode?.RegisterUsage(DateTime.UtcNow);
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

    private async Task EnsureTicketPurchaseAllowedAsync(long userId, string role, CancellationToken cancellationToken)
    {
        if (await _restrictions.HasActiveRestrictionAsync(userId, UserRestrictionType.TicketPurchase, DateTime.UtcNow, cancellationToken))
            throw new BusinessRuleViolationException(
                "Ticket purchase restricted",
                $"{role} is temporarily restricted from buying tickets.");
    }
}
