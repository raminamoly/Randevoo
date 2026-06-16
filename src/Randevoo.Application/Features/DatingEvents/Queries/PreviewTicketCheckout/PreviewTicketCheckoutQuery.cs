using MediatR;
using Randevoo.Domain.Enums;

namespace Randevoo.Application.Features.DatingEvents.Queries.PreviewTicketCheckout;

public record PreviewTicketCheckoutQuery(
    long BuyerUserId,
    long EventId,
    string? DiscountCode = null,
    long? ParticipantUserId = null,
    string? ParticipantMobileNumber = null) : IRequest<TicketCheckoutPreviewDto>;

public record TicketCheckoutPreviewDto(
    long EventId,
    string EventTitle,
    long BuyerUserId,
    long ParticipantUserId,
    string ParticipantDisplayName,
    EventPaymentCollectionMethod PaymentCollectionMethod,
    decimal GrossAmount,
    decimal DiscountAmount,
    decimal NetAmount,
    string CurrencyCode,
    string? DiscountCode,
    bool RequiresManualReceipt,
    string PaymentInstruction);
