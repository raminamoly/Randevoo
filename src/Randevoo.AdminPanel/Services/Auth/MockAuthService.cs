using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Randevoo.Application.Interfaces.Auditing;
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
    private readonly IAuditLogger _auditLogger;

    public MockAuthService(RandevooDbContext db, IHttpContextAccessor httpContextAccessor, IAuditLogger auditLogger)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
        _auditLogger = auditLogger;
    }

    public async Task<MockAuthResult> RequestCodeAsync(string mobile, AdminRole requestedRole)
    {
        var normalizedMobile = (mobile ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(normalizedMobile))
        {
            await LogAsync(null, "AdminLoginCodeRequestFailed", "login_code_request", "account", normalizedMobile, "شماره موبایل خالی بود.", "failed");
            return MockAuthResult.Fail("شماره موبایل را وارد کنید.");
        }

        if (requestedRole == AdminRole.SupportTeam)
        {
            await LogAsync(null, "AdminLoginCodeRequestFailed", "login_code_request", "account", normalizedMobile, "نقش انتخاب شده پشتیبانی بود.", "failed");
            return MockAuthResult.Fail("نقش تیم پشتیبانی در نسخه واقعی پنل فعال نشده است.");
        }

        var user = await _db.Users
            .Include(item => item.Profile)
            .FirstOrDefaultAsync(item => item.MobileNumber == normalizedMobile);

        if (user is null)
        {
            await LogAsync(null, "AdminLoginCodeRequestFailed", "login_code_request", "account", normalizedMobile, "کاربر پیدا نشد.", "failed");
            return MockAuthResult.Fail("این شماره موبایل هنوز در پایگاه داده ثبت نشده است.");
        }

        if (user.Role == UserRole.EndUser)
        {
            await LogAsync(user.Id, "AdminLoginCodeRequestFailed", "login_code_request", "account", normalizedMobile, "کاربر نقش پنل مدیریت ندارد.", "failed");
            return MockAuthResult.Fail("این حساب برای پنل مدیریت دسترسی ندارد.");
        }

        if (DatabaseModelMapper.ToAdminRole(user.Role) != requestedRole)
        {
            await LogAsync(user.Id, "AdminLoginCodeRequestFailed", "login_code_request", "account", normalizedMobile, "نقش انتخاب شده با حساب هماهنگ نیست.", "failed");
            return MockAuthResult.Fail("نقش انتخاب شده با این حساب هماهنگ نیست.");
        }

        if (!user.IsActive)
        {
            await LogAsync(user.Id, "AdminLoginCodeRequestFailed", "login_code_request", "account", normalizedMobile, "حساب غیرفعال است.", "failed");
            return MockAuthResult.Fail("این حساب غیرفعال شده است.");
        }

        await LogAsync(user.Id, "AdminLoginCodeRequested", "login_code_request", "account", normalizedMobile, "درخواست کد ورود پنل مدیریت ثبت شد.", "success");
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
            await LogAsync(result.User.Id, "AdminLoginFailed", "failed_login", "account", mobile, "کد تایید وارد نشد.", "failed");
            return MockAuthResult.Fail("کد تایید را وارد کنید.");
        }

        if (!string.Equals(verificationCode.Trim(), DemoVerificationCode, StringComparison.Ordinal))
        {
            await LogAsync(result.User.Id, "AdminLoginFailed", "failed_login", "account", mobile, "کد تایید نامعتبر بود.", "failed");
            return MockAuthResult.Fail("کد تایید آزمایشی معتبر نیست.");
        }

        await LogAsync(result.User.Id, "AdminLoginSucceeded", "login", "account", mobile, "ورود به پنل مدیریت موفق بود.", "success");
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
        var userId = context.User.Identity?.IsAuthenticated == true
            && long.TryParse(context.User.FindFirstValue(ClaimTypes.NameIdentifier), out var parsedUserId)
            ? parsedUserId
            : (long?)null;

        await LogAsync(userId, "AdminLogout", "logout", "account", context.Request.Path.Value ?? "/Account/Logout", "خروج از پنل مدیریت انجام شد.", "success");
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    private Task LogAsync(long? actorUserId, string action, string logType, string module, string targetId, string description, string status)
    {
        return _auditLogger.TryLogAsync(new AuditLogEntry(
            ActorUserId: actorUserId,
            Action: action,
            TargetType: "AdminAccount",
            TargetId: string.IsNullOrWhiteSpace(targetId) ? "/" : targetId,
            LogType: logType,
            Module: module,
            Description: description,
            Status: status));
    }
}
