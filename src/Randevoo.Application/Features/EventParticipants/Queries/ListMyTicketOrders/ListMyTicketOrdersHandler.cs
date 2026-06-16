using MediatR;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.EventParticipants.Queries.ListMyTicketOrders;

public sealed class ListMyTicketOrdersHandler : IRequestHandler<ListMyTicketOrdersQuery, IReadOnlyList<MyTicketOrderDto>>
{
    private readonly ITicketOrderRepository _orders;
    private readonly IManualPaymentReceiptRepository _manualReceipts;

    public ListMyTicketOrdersHandler(ITicketOrderRepository orders, IManualPaymentReceiptRepository manualReceipts)
    {
        _orders = orders;
        _manualReceipts = manualReceipts;
    }

    public async Task<IReadOnlyList<MyTicketOrderDto>> Handle(ListMyTicketOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orders.ListForBuyerOrParticipantAsync(request.UserId, cancellationToken);
        var receipts = await _manualReceipts.ListByOrderIdsAsync(orders.Select(order => order.Id).ToArray(), cancellationToken);
        var receiptByOrderId = receipts
            .Where(receipt => receipt.TicketOrderId.HasValue)
            .GroupBy(receipt => receipt.TicketOrderId!.Value)
            .ToDictionary(group => group.Key, group => group.OrderByDescending(item => item.SubmittedAtUtc).First());

        return orders
            .Select(order => Map(order, receiptByOrderId.GetValueOrDefault(order.Id)))
            .ToList();
    }

    private static MyTicketOrderDto Map(TicketOrder order, ManualPaymentReceipt? receipt)
    {
        var ticket = order.Tickets.FirstOrDefault();
        var participant = ticket?.User ?? receipt?.ParticipantUser ?? order.BuyerUser;
        return new MyTicketOrderDto(
            order.Id,
            order.DatingEventId,
            order.DatingEvent.Title,
            order.DatingEvent.DateTimeStart,
            order.DatingEvent.DateTimeEnd,
            order.PaymentCollectionMethod,
            order.PaymentStatus,
            order.OrderStatus,
            order.GrossAmount,
            order.DiscountAmount,
            order.NetAmount,
            order.CurrencyCode,
            order.BuyerUserId,
            BuildDisplayName(order.BuyerUser),
            participant.Id,
            BuildDisplayName(participant),
            ticket?.Id,
            ticket?.IsValidForEventAccess ?? false,
            ticket?.IsRefunded ?? false,
            ticket?.IsRemoved ?? false,
            ticket?.RemovalReason,
            receipt?.Id,
            receipt?.Status);
    }

    private static string BuildDisplayName(User user)
    {
        var profile = user.Profile;
        if (profile is null)
            return user.MobileNumber;

        return string.IsNullOrWhiteSpace(profile.DisplayName) ? user.MobileNumber : profile.DisplayName;
    }
}
