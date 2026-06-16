using MediatR;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.DatingEvents.Queries.PreviewTicketCheckout;

public sealed class PreviewTicketCheckoutHandler : IRequestHandler<PreviewTicketCheckoutQuery, TicketCheckoutPreviewDto>
{
    private readonly IUserRepository _users;
    private readonly IUserProfileRepository _profiles;
    private readonly IDatingEventRepository _events;
    private readonly IEventDiscountCodeRepository _discountCodes;

    public PreviewTicketCheckoutHandler(
        IUserRepository users,
        IUserProfileRepository profiles,
        IDatingEventRepository events,
        IEventDiscountCodeRepository discountCodes)
    {
        _users = users;
        _profiles = profiles;
        _events = events;
        _discountCodes = discountCodes;
    }

    public async Task<TicketCheckoutPreviewDto> Handle(PreviewTicketCheckoutQuery request, CancellationToken cancellationToken)
    {
        var buyer = await _users.GetByIdAsync(request.BuyerUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.BuyerUserId);
        var participant = await ResolveParticipantAsync(request, buyer, cancellationToken);
        var profile = await _profiles.GetByUserIdAsync(participant.Id, cancellationToken)
            ?? throw new BusinessRuleViolationException("Profile required", "Participant must complete a profile before buying tickets");
        var datingEvent = await _events.GetByIdWithTicketsAsync(request.EventId, cancellationToken)
            ?? throw new NotFoundException("DatingEvent", request.EventId);

        // Reuse domain ticket invariants without persisting anything.
        var basePrice = datingEvent.GetTicketPriceForGender(profile.Gender);
        var discountCode = await ResolveDiscountCodeAsync(datingEvent.Id, request.DiscountCode, cancellationToken);
        decimal? discountedPrice = null;
        if (discountCode is not null)
        {
            discountCode.EnsureCanUse(datingEvent.Id, profile.Gender, DateTime.UtcNow, basePrice);
            discountedPrice = discountCode.CalculateDiscountedPrice(basePrice);
        }

        var finalPrice = discountedPrice ?? basePrice;
        var previewOrder = new TicketOrder(
            datingEvent,
            buyer,
            basePrice,
            basePrice - finalPrice,
            finalPrice,
            finalPrice * datingEvent.EventPlannerCommissionPercent / 100,
            datingEvent.PaymentCollectionMethod,
            datingEvent.GetTicketCurrencyForGender(profile.Gender),
            1m,
            DateTime.UtcNow,
            null,
            discountCode);
        datingEvent.SellTicket(previewOrder, participant, profile, finalPrice, discountCode);

        var paymentInstruction = datingEvent.PaymentCollectionMethod switch
        {
            EventPaymentCollectionMethod.PlatformGateway => "پرداخت آنلاین با درگاه پلتفرم انجام و بلیت بلافاصله صادر می‌شود.",
            EventPaymentCollectionMethod.PlatformManualTransfer => "پس از واریز به حساب پلتفرم، رسید را بارگذاری کن؛ بلیت بعد از تایید پشتیبانی صادر می‌شود.",
            EventPaymentCollectionMethod.OrganizerManualTransfer => datingEvent.OrganizerPaymentInstructions
                ?? "پس از واریز به حساب برگزارکننده، رسید را بارگذاری کن؛ بلیت بعد از تایید برگزارکننده صادر می‌شود.",
            _ => "روش پرداخت نامشخص است."
        };

        return new TicketCheckoutPreviewDto(
            datingEvent.Id,
            datingEvent.Title,
            buyer.Id,
            participant.Id,
            BuildDisplayName(participant),
            datingEvent.PaymentCollectionMethod,
            basePrice,
            basePrice - finalPrice,
            finalPrice,
            datingEvent.GetTicketCurrencyForGender(profile.Gender),
            discountCode?.Code,
            datingEvent.PaymentCollectionMethod != EventPaymentCollectionMethod.PlatformGateway,
            paymentInstruction);
    }

    private async Task<User> ResolveParticipantAsync(PreviewTicketCheckoutQuery request, User buyer, CancellationToken cancellationToken)
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
            return await _users.GetByMobileNumberAsync(request.ParticipantMobileNumber.Trim(), cancellationToken)
                ?? throw new BusinessRuleViolationException("Participant not found", "No participant user was found with this mobile number.");
        }

        return buyer;
    }

    private async Task<EventDiscountCode?> ResolveDiscountCodeAsync(long eventId, string? code, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        return await _discountCodes.GetApplicableByCodeAsync(eventId, code, cancellationToken)
            ?? throw new BusinessRuleViolationException("Discount code not found", "Discount code is not valid for this event.");
    }

    private static string BuildDisplayName(User user)
    {
        var profile = user.Profile;
        if (profile is null)
            return user.MobileNumber;

        return string.IsNullOrWhiteSpace(profile.DisplayName) ? user.MobileNumber : profile.DisplayName;
    }
}
