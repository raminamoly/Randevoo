using MediatR;
using Randevoo.Domain.Enums;

namespace Randevoo.Application.Features.EventParticipants.Queries.ListMyTicketOrders;

public record ListMyTicketOrdersQuery(long UserId) : IRequest<IReadOnlyList<MyTicketOrderDto>>;

public record MyTicketOrderDto(
    long OrderId,
    long EventId,
    string EventTitle,
    DateTime DateTimeStart,
    DateTime DateTimeEnd,
    EventPaymentCollectionMethod PaymentCollectionMethod,
    TicketOrderPaymentStatus PaymentStatus,
    TicketOrderStatus OrderStatus,
    decimal GrossAmount,
    decimal DiscountAmount,
    decimal NetAmount,
    string CurrencyCode,
    long BuyerUserId,
    string BuyerDisplayName,
    long ParticipantUserId,
    string ParticipantDisplayName,
    long? TicketId,
    bool HasValidTicket,
    bool IsRefunded,
    bool IsRemoved,
    string? RemovalReason,
    long? ManualReceiptId,
    ManualPaymentReceiptStatus? ManualReceiptStatus);
