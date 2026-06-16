using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Randevoo.Web.Services;

public sealed class EndUserTicketsApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly EndUserSessionService _session;

    public EndUserTicketsApiClient(IHttpClientFactory httpClientFactory, EndUserSessionService session)
    {
        _httpClientFactory = httpClientFactory;
        _session = session;
    }

    public async Task<TicketCheckoutPreviewViewModel> PreviewAsync(long eventId, TicketCheckoutRequestViewModel request, CancellationToken cancellationToken)
    {
        var client = CreateAuthorizedClient();
        var response = await client.PostAsJsonAsync($"/api/v1/platform/events/{eventId}/checkout/preview", new
        {
            DiscountCode = string.IsNullOrWhiteSpace(request.DiscountCode) ? null : request.DiscountCode.Trim(),
            ParticipantUserId = request.ParticipantUserId,
            ParticipantMobileNumber = string.IsNullOrWhiteSpace(request.ParticipantMobileNumber) ? null : request.ParticipantMobileNumber.Trim()
        }, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorAsync(response, cancellationToken));

        return await response.Content.ReadFromJsonAsync<TicketCheckoutPreviewViewModel>(cancellationToken)
            ?? throw new InvalidOperationException("پاسخ پیش‌نمایش خرید نامعتبر است.");
    }

    public async Task<TicketPurchaseResultViewModel> BuyAsync(long eventId, TicketCheckoutRequestViewModel request, CancellationToken cancellationToken)
    {
        var client = CreateAuthorizedClient();
        var response = await client.PostAsJsonAsync($"/api/v1/platform/events/{eventId}/tickets", new
        {
            DiscountCode = string.IsNullOrWhiteSpace(request.DiscountCode) ? null : request.DiscountCode.Trim(),
            ParticipantUserId = request.ParticipantUserId,
            ParticipantMobileNumber = string.IsNullOrWhiteSpace(request.ParticipantMobileNumber) ? null : request.ParticipantMobileNumber.Trim(),
            ManualReceiptFilePath = request.ManualReceiptFilePath,
            ManualReceiptTrackingNumber = string.IsNullOrWhiteSpace(request.ManualReceiptTrackingNumber) ? null : request.ManualReceiptTrackingNumber.Trim(),
            ManualReceiptNote = string.IsNullOrWhiteSpace(request.ManualReceiptNote) ? null : request.ManualReceiptNote.Trim()
        }, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorAsync(response, cancellationToken));

        return await response.Content.ReadFromJsonAsync<TicketPurchaseResultViewModel>(cancellationToken)
            ?? throw new InvalidOperationException("پاسخ خرید بلیت نامعتبر است.");
    }

    public async Task<IReadOnlyList<MyTicketViewModel>> ListMineAsync(CancellationToken cancellationToken)
    {
        var client = CreateAuthorizedClient();
        var response = await client.GetAsync("/api/v1/platform/tickets", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return Array.Empty<MyTicketViewModel>();
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorAsync(response, cancellationToken));

        return await response.Content.ReadFromJsonAsync<IReadOnlyList<MyTicketViewModel>>(cancellationToken)
            ?? Array.Empty<MyTicketViewModel>();
    }

    private HttpClient CreateAuthorizedClient()
    {
        var token = _session.GetAccessToken();
        if (string.IsNullOrWhiteSpace(token))
            throw new UnauthorizedAccessException("برای ادامه باید وارد شوید.");

        var client = _httpClientFactory.CreateClient("RandevooApi");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        return string.IsNullOrWhiteSpace(body) ? "درخواست بلیت انجام نشد." : body;
    }
}

public sealed record TicketCheckoutRequestViewModel(
    string? DiscountCode,
    long? ParticipantUserId,
    string? ParticipantMobileNumber,
    string? ManualReceiptFilePath = null,
    string? ManualReceiptTrackingNumber = null,
    string? ManualReceiptNote = null);

public sealed record TicketCheckoutPreviewViewModel(
    long EventId,
    string EventTitle,
    long BuyerUserId,
    long ParticipantUserId,
    string ParticipantDisplayName,
    int PaymentCollectionMethod,
    decimal GrossAmount,
    decimal DiscountAmount,
    decimal NetAmount,
    string CurrencyCode,
    string? DiscountCode,
    bool RequiresManualReceipt,
    string PaymentInstruction);

public sealed record TicketPurchaseResultViewModel(
    long OrderId,
    long TicketId,
    IReadOnlyList<long> TicketIds,
    int PaymentCollectionMethod,
    int PaymentStatus,
    int OrderStatus,
    long? ManualPaymentReceiptId,
    long? OnlinePaymentId,
    long? ParticipantUserId,
    decimal GrossAmount,
    decimal DiscountAmount,
    decimal NetAmount,
    string CurrencyCode);

public sealed record MyTicketViewModel(
    long OrderId,
    long EventId,
    string EventTitle,
    DateTime DateTimeStart,
    DateTime DateTimeEnd,
    int PaymentCollectionMethod,
    int PaymentStatus,
    int OrderStatus,
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
    int? ManualReceiptStatus);
