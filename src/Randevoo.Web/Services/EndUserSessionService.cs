namespace Randevoo.Web.Services;

public sealed class EndUserSessionService
{
    private const string AccessTokenCookie = "rv_access_token";
    private const string RefreshTokenCookie = "rv_refresh_token";
    private const string UserIdCookie = "rv_user_id";
    private const string MobileCookie = "rv_mobile";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EndUserSessionService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsSignedIn => GetUserId() is not null && !string.IsNullOrWhiteSpace(GetAccessToken());

    public string? GetAccessToken() => Request?.Cookies[AccessTokenCookie];
    public string? GetRefreshToken() => Request?.Cookies[RefreshTokenCookie];
    public string? GetMobileNumber() => Request?.Cookies[MobileCookie];

    public long? GetUserId()
    {
        var value = Request?.Cookies[UserIdCookie];
        return long.TryParse(value, out var userId) ? userId : null;
    }

    public void SignIn(AuthResultViewModel auth)
    {
        var response = Response ?? throw new InvalidOperationException("HTTP response is not available.");
        var secure = Request?.IsHttps == true;
        var accessCookie = BuildCookieOptions(auth.AccessTokenExpiresAtUtc, secure);
        var refreshCookie = BuildCookieOptions(auth.RefreshTokenExpiresAtUtc, secure);

        response.Cookies.Append(AccessTokenCookie, auth.Token, accessCookie);
        response.Cookies.Append(RefreshTokenCookie, auth.RefreshToken, refreshCookie);
        response.Cookies.Append(UserIdCookie, auth.UserId.ToString(), refreshCookie);
        response.Cookies.Append(MobileCookie, auth.MobileNumber, refreshCookie);
    }

    public void SignOut()
    {
        var response = Response;
        if (response is null)
            return;

        response.Cookies.Delete(AccessTokenCookie);
        response.Cookies.Delete(RefreshTokenCookie);
        response.Cookies.Delete(UserIdCookie);
        response.Cookies.Delete(MobileCookie);
    }

    private HttpRequest? Request => _httpContextAccessor.HttpContext?.Request;
    private HttpResponse? Response => _httpContextAccessor.HttpContext?.Response;

    private static CookieOptions BuildCookieOptions(DateTime expiresAtUtc, bool secure) => new()
    {
        HttpOnly = true,
        Secure = secure,
        SameSite = SameSiteMode.Lax,
        Expires = new DateTimeOffset(expiresAtUtc)
    };
}
