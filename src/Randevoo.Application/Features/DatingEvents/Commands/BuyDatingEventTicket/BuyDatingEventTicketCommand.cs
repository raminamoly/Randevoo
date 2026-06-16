using MediatR;
using Randevoo.Domain.Enums;

namespace Randevoo.Application.Features.DatingEvents.Commands.BuyDatingEventTicket;

public record BuyDatingEventTicketCommand(
    long BuyerUserId,
    long EventId,
    string? DiscountCode = null,
    long? ParticipantUserId = null,
    string? ParticipantMobileNumber = null,
    string? ManualReceiptFilePath = null,
    string? ManualReceiptTrackingNumber = null,
    string? ManualReceiptNote = null) : IRequest<TicketOrderPurchaseResult>;

public record TicketOrderPurchaseResult(
    long TicketOrderId,
    IReadOnlyList<long> TicketIds,
    EventPaymentCollectionMethod PaymentCollectionMethod,
    TicketOrderPaymentStatus PaymentStatus,
    TicketOrderStatus OrderStatus,
    long? ManualPaymentReceiptId = null,
    long? OnlinePaymentId = null,
    long? ParticipantUserId = null,
    decimal GrossAmount = 0,
    decimal DiscountAmount = 0,
    decimal NetAmount = 0,
    string CurrencyCode = "IRR")
{
    public long TicketId => TicketIds.FirstOrDefault();
}
