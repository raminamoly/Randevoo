using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Randevoo.Web.Services;

namespace Randevoo.Web.Pages.Platform.Account;

public class LoginModel : PageModel
{
    private readonly EndUserAuthApiClient _authApiClient;
    private readonly EndUserSessionService _session;
    private readonly IWebHostEnvironment _environment;

    public LoginModel(EndUserAuthApiClient authApiClient, EndUserSessionService session, IWebHostEnvironment environment)
    {
        _authApiClient = authApiClient;
        _session = session;
        _environment = environment;
    }

    [BindProperty]
    [Required(ErrorMessage = "شماره موبایل را وارد کن.")]
    [MinLength(8, ErrorMessage = "شماره موبایل کوتاه است.")]
    [MaxLength(20, ErrorMessage = "شماره موبایل طولانی است.")]
    public string MobileNumber { get; set; } = string.Empty;

    [BindProperty]
    [MaxLength(6, ErrorMessage = "کد باید ۶ رقم باشد.")]
    public string Code { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public bool CodeRequested { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string? SuccessMessage { get; private set; }
    public bool ShowDevelopmentHint => _environment.IsDevelopment();

    public IActionResult OnGet()
    {
        if (_session.IsSignedIn)
            return RedirectAfterSignIn();

        if (_environment.IsDevelopment())
        {
            MobileNumber = "09120000000";
            Code = "123456";
        }

        return Page();
    }

    public async Task<IActionResult> OnPostRequestCodeAsync(CancellationToken cancellationToken)
    {
        CodeRequested = false;
        ModelState.Remove(nameof(Code));
        if (!ModelState.IsValid)
            return Page();

        try
        {
            await _authApiClient.RequestCodeAsync(MobileNumber.Trim(), cancellationToken);
            CodeRequested = true;
            if (_environment.IsDevelopment())
                Code = "123456";
            SuccessMessage = _environment.IsDevelopment()
                ? "کد توسعه آماده است: 123456"
                : "کد ورود ارسال شد.";
            return Page();
        }
        catch (Exception ex)
        {
            ErrorMessage = ToFriendlyMessage(ex);
            return Page();
        }
    }

    public async Task<IActionResult> OnPostVerifyCodeAsync(CancellationToken cancellationToken)
    {
        CodeRequested = true;
        if (string.IsNullOrWhiteSpace(MobileNumber))
            ModelState.AddModelError(nameof(MobileNumber), "شماره موبایل را وارد کن.");
        if (string.IsNullOrWhiteSpace(Code) || Code.Trim().Length != 6)
            ModelState.AddModelError(nameof(Code), "کد ۶ رقمی را وارد کن.");
        if (!ModelState.IsValid)
            return Page();

        try
        {
            var auth = await _authApiClient.VerifyCodeAsync(MobileNumber.Trim(), Code.Trim(), cancellationToken);
            _session.SignIn(auth);
            return RedirectAfterSignIn();
        }
        catch (Exception ex)
        {
            ErrorMessage = ToFriendlyMessage(ex);
            return Page();
        }
    }

    private string ToFriendlyMessage(Exception ex)
    {
        if (_environment.IsDevelopment())
            return ex.Message;

        var message = ex.Message;
        if (message.Contains("Too many", StringComparison.OrdinalIgnoreCase))
            return "تعداد درخواست‌ها زیاد شده است. چند دقیقه بعد دوباره تلاش کن.";
        if (message.Contains("Invalid", StringComparison.OrdinalIgnoreCase))
            return "کد یا شماره موبایل درست نیست.";
        return "ورود انجام نشد. دوباره تلاش کن.";
    }

    private IActionResult RedirectAfterSignIn()
    {
        if (!string.IsNullOrWhiteSpace(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            return LocalRedirect(ReturnUrl);

        return RedirectToPage("/Platform/Events/Index");
    }
}
