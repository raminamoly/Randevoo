using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Services.ApiClients;
using Randevoo.Domain.Enums;
using Randevoo.Infrastructure.Data;

namespace Randevoo.AdminPanel.Services.Auth;

public sealed class MockAuthService
{
    private const string DemoVerificationCode = "123456";
    private readonly RandevooDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MockAuthService(RandevooDbContext db, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<MockAuthResult> RequestCodeAsync(string mobile, AdminRole requestedRole)
    {
        var normalizedMobile = (mobile ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(normalizedMobile))
        {
            return MockAuthResult.Fail("شماره موبایل را وارد کنید.");
        }

        if (requestedRole == AdminRole.SupportTeam)
        {
            return MockAuthResult.Fail("نقش تیم پشتیبانی در نسخه واقعی پنل فعال نشده است.");
        }

        var user = await _db.Users
            .Include(item => item.Profile)
            .FirstOrDefaultAsync(item => item.MobileNumber == normalizedMobile);

        if (user is null)
        {
            return MockAuthResult.Fail("این شماره موبایل هنوز در پایگاه داده ثبت نشده است.");
        }

        if (user.Role == UserRole.EndUser)
        {
            return MockAuthResult.Fail("این حساب برای پنل مدیریت دسترسی ندارد.");
        }

        if (DatabaseModelMapper.ToAdminRole(user.Role) != requestedRole)
        {
            return MockAuthResult.Fail("نقش انتخاب شده با این حساب هماهنگ نیست.");
        }

        if (!user.IsActive)
        {
            return MockAuthResult.Fail("این حساب غیرفعال شده است.");
        }

        return MockAuthResult.Ok(DatabaseModelMapper.ToAdminUser(user));
    }

    public async Task<MockAuthResult> VerifyLoginAsync(string mobile, string verificationCode, AdminRole requestedRole)
    {
        var result = await RequestCodeAsync(mobile, requestedRole);
        if (!result.Success || result.User is null)
        {
            return result;
        }

        if (string.IsNullOrWhiteSpace(verificationCode))
        {
            return MockAuthResult.Fail("کد تایید را وارد کنید.");
        }

        if (!string.Equals(verificationCode.Trim(), DemoVerificationCode, StringComparison.Ordinal))
        {
            return MockAuthResult.Fail("کد تایید آزمایشی معتبر نیست.");
        }

        return result;
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
