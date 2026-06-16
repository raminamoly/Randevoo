using System.Net;
using System.Net.Http.Json;

namespace Randevoo.Web.Services;

public sealed class EndUserAuthApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public EndUserAuthApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task RequestCodeAsync(string mobileNumber, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("RandevooApi");
        var response = await client.PostAsJsonAsync("/api/v1/platform/auth/mobile/request-code", new { MobileNumber = mobileNumber }, cancellationToken);
        if (response.StatusCode != HttpStatusCode.Accepted)
            throw new InvalidOperationException(await ReadErrorAsync(response, cancellationToken));
    }

    public async Task<AuthResultViewModel> VerifyCodeAsync(string mobileNumber, string code, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient("RandevooApi");
        var response = await client.PostAsJsonAsync("/api/v1/platform/auth/mobile/verify", new { MobileNumber = mobileNumber, Code = code }, cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorAsync(response, cancellationToken));

        return await response.Content.ReadFromJsonAsync<AuthResultViewModel>(cancellationToken)
            ?? throw new InvalidOperationException("پاسخ ورود نامعتبر است.");
    }

    private static async Task<string> ReadErrorAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        return string.IsNullOrWhiteSpace(body) ? "درخواست ورود انجام نشد." : body;
    }
}

public sealed record AuthResultViewModel(
    long UserId,
    string MobileNumber,
    string Token,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc);
