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
            return Task.FromResult(MockAuthResult.Fail("This mobile number is not registered yet."));
        }

        if (user.Role != requestedRole)
        {
            return Task.FromResult(MockAuthResult.Fail("The selected role does not match this account."));
        }

        if (!user.IsActive)
        {
            return Task.FromResult(MockAuthResult.Fail("This account is disabled."));
        }

        if (string.IsNullOrWhiteSpace(verificationCode))
        {
            return Task.FromResult(MockAuthResult.Fail("Please enter the verification code."));
        }

        return Task.FromResult(MockAuthResult.Ok(user));
    }

    public async Task SignInAsync(MockUser user)
    {
        var context = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("No active HTTP context.");

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
        var context = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("No active HTTP context.");
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}

