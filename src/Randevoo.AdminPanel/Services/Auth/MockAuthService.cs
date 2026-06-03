using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Services.MockData;

namespace Randevoo.AdminPanel.Services.Auth;

public sealed class MockAuthService
{
    private readonly AdminPanelStore _store;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MockAuthService(AdminPanelStore store, IHttpContextAccessor httpContextAccessor)
    {
        _store = store;
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<MockAuthResult> VerifyLoginAsync(string mobile, string verificationCode, AdminRole requestedRole)
    {
        var user = _store.FindUserByMobile(mobile);
        if (user is null)
        {
            return Task.FromResult(MockAuthResult.Fail("این شماره موبایل هنوز ثبت نشده است."));
        }

        if (user.Role != requestedRole)
        {
            return Task.FromResult(MockAuthResult.Fail("نقش انتخاب شده با این حساب هماهنگ نیست."));
        }

        if (!user.IsActive)
        {
            return Task.FromResult(MockAuthResult.Fail("این حساب غیرفعال شده است."));
        }

        if (string.IsNullOrWhiteSpace(verificationCode))
        {
            return Task.FromResult(MockAuthResult.Fail("کد تایید را وارد کنید."));
        }

        return Task.FromResult(MockAuthResult.Ok(user));
    }

    public async Task SignInAsync(MockUser user)
    {
        var context = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("درخواست فعال HTTP پیدا نشد.");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.FullName),
            new(ClaimTypes.Role, user.Role.ToString()),
            new("mobile", user.Mobile)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await context.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                AllowRefresh = true
            });
    }

    public async Task SignOutAsync()
    {
        var context = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("درخواست فعال HTTP پیدا نشد.");
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
