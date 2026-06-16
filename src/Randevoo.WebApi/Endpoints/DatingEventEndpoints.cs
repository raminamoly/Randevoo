using System.Security.Claims;
using MediatR;
using Randevoo.Application.Features.DatingEvents.Commands.BuyDatingEventTicket;
using Randevoo.Application.Features.DatingEvents.Queries.PreviewTicketCheckout;
using Randevoo.Domain.Exceptions;

namespace Randevoo.WebApi.Endpoints;

public static class DatingEventEndpoints
{
    public static RouteGroupBuilder MapDatingEventEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/platform/events")
            .RequireAuthorization("EndUserOnly")
            .WithTags("Platform Tickets");

        group.MapPost("/{eventId:long}/tickets", BuyTicketAsync).RequireAuthorization("EndUserOnly").WithName("BuyDatingEventTicket");
        group.MapPost("/{eventId:long}/checkout/preview", PreviewTicketAsync).RequireAuthorization("EndUserOnly").WithName("PreviewDatingEventTicketCheckout");
        return group;
    }

    private static async Task<IResult> BuyTicketAsync(long eventId, BuyDatingEventTicketRequest? request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            var result = await sender.Send(new BuyDatingEventTicketCommand(
                EndpointHelpers.GetUserId(principal),
                eventId,
                request?.DiscountCode,
                request?.ParticipantUserId,
                request?.ParticipantMobileNumber,
                request?.ManualReceiptFilePath,
                request?.ManualReceiptTrackingNumber,
                request?.ManualReceiptNote), cancellationToken);

            return Results.Created($"/api/v1/platform/events/{eventId}/orders/{result.TicketOrderId}", new
            {
                orderId = result.TicketOrderId,
                ticketId = result.TicketId,
                ticketIds = result.TicketIds,
                paymentCollectionMethod = result.PaymentCollectionMethod,
                paymentStatus = result.PaymentStatus,
                orderStatus = result.OrderStatus,
                manualPaymentReceiptId = result.ManualPaymentReceiptId,
                onlinePaymentId = result.OnlinePaymentId,
                participantUserId = result.ParticipantUserId,
                grossAmount = result.GrossAmount,
                discountAmount = result.DiscountAmount,
                netAmount = result.NetAmount,
                currencyCode = result.CurrencyCode
            });
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    private static async Task<IResult> PreviewTicketAsync(long eventId, TicketCheckoutPreviewRequest? request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        try
        {
            return Results.Ok(await sender.Send(new PreviewTicketCheckoutQuery(
                EndpointHelpers.GetUserId(principal),
                eventId,
                request?.DiscountCode,
                request?.ParticipantUserId,
                request?.ParticipantMobileNumber), cancellationToken));
        }
        catch (Exception ex) when (ex is DomainException or UnauthorizedAccessException)
        {
            return EndpointHelpers.ToProblem(ex);
        }
    }

    public record BuyDatingEventTicketRequest(
        string? DiscountCode,
        long? ParticipantUserId,
        string? ParticipantMobileNumber,
        string? ManualReceiptFilePath,
        string? ManualReceiptTrackingNumber,
        string? ManualReceiptNote);

    public record TicketCheckoutPreviewRequest(
        string? DiscountCode,
        long? ParticipantUserId,
        string? ParticipantMobileNumber);
}
