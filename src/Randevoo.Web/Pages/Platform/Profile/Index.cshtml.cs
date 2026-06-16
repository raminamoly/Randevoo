using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using Randevoo.Web.Services;

namespace Randevoo.Web.Pages.Platform.Profile;

public class IndexModel : PageModel
{
    private readonly EndUserProfileApiClient _profileApiClient;
    private readonly EndUserSessionService _session;
    private readonly IWebHostEnvironment _environment;

    public IndexModel(
        EndUserProfileApiClient profileApiClient,
        EndUserSessionService session,
        IWebHostEnvironment environment)
    {
        _profileApiClient = profileApiClient;
        _session = session;
        _environment = environment;
    }

    [BindProperty]
    public ProfileFormModel Form { get; set; } = new();

    [BindProperty]
    public long? ProfileId { get; set; }

    [BindProperty]
    public int PersianBirthYear { get; set; } = 1374;

    [BindProperty]
    public int PersianBirthMonth { get; set; } = 1;

    [BindProperty]
    public int PersianBirthDay { get; set; } = 1;

    [BindProperty]
    public List<IFormFile> ProfileImageFiles { get; set; } = [];

    public DatingProfileViewModel? Profile { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string? SuccessMessage { get; private set; }
    public bool IsSignedIn => _session.IsSignedIn;
    public string? MobileNumber => _session.GetMobileNumber();
    public IReadOnlyList<int> PersianYears { get; } = Enumerable.Range(1320, 90).Reverse().ToList();
    public IReadOnlyList<SelectOption> PersianMonths { get; } =
    [
        new(1, "۱ - فروردین"),
        new(2, "۲ - اردیبهشت"),
        new(3, "۳ - خرداد"),
        new(4, "۴ - تیر"),
        new(5, "۵ - مرداد"),
        new(6, "۶ - شهریور"),
        new(7, "۷ - مهر"),
        new(8, "۸ - آبان"),
        new(9, "۹ - آذر"),
        new(10, "۱۰ - دی"),
        new(11, "۱۱ - بهمن"),
        new(12, "۱۲ - اسفند")
    ];
    public IReadOnlyList<int> PersianDays { get; } = Enumerable.Range(1, 31).ToList();
    public IReadOnlyList<SelectOption> CityOptions { get; } =
    [
        new(1, "تهران", "Tehran"),
        new(2, "مشهد", "Mashhad"),
        new(3, "شیراز", "Shiraz"),
        new(4, "اصفهان", "Isfahan"),
        new(5, "تبریز", "Tabriz")
    ];
    public IReadOnlyList<ChoiceOption> GenderOptions { get; } =
    [
        new("2", "آقا", "bi-gender-male"),
        new("3", "خانم", "bi-gender-female")
    ];
    public IReadOnlyList<ChoiceOption> ZodiacOptions { get; } =
    [
        new("1", "حمل", "bi-stars"),
        new("2", "ثور", "bi-flower1"),
        new("3", "جوزا", "bi-gem"),
        new("4", "سرطان", "bi-moon-stars"),
        new("5", "اسد", "bi-sun"),
        new("6", "سنبله", "bi-feather"),
        new("7", "میزان", "bi-yin-yang"),
        new("8", "عقرب", "bi-lightning"),
        new("9", "قوس", "bi-arrow-up-right-circle"),
        new("10", "جدی", "bi-mountain"),
        new("11", "دلو", "bi-water"),
        new("12", "حوت", "bi-droplet")
    ];
    public IReadOnlyList<InterestGroup> InterestGroups { get; } =
    [
        new("سبک زندگی", [
            new("کافه‌گردی", "bi-cup-hot"),
            new("پیاده‌روی", "bi-person-walking"),
            new("سفر", "bi-luggage"),
            new("آشپزی", "bi-egg-fried")
        ]),
        new("فرهنگ و هنر", [
            new("موسیقی", "bi-music-note-beamed"),
            new("سینما", "bi-camera-reels"),
            new("کتاب", "bi-book"),
            new("گالری", "bi-palette")
        ]),
        new("اجتماعی و رشد", [
            new("گفتگو", "bi-chat-heart"),
            new("روانشناسی", "bi-heart-pulse"),
            new("ورزش", "bi-activity"),
            new("بازی گروهی", "bi-dice-5")
        ])
    ];

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (!_session.IsSignedIn)
            return RedirectToPage("/Platform/Account/Login", new { returnUrl = "/platform/profile" });

        await LoadProfileAsync(cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostSaveAsync(CancellationToken cancellationToken)
    {
        if (!_session.IsSignedIn)
            return RedirectToPage("/Platform/Account/Login", new { returnUrl = "/platform/profile" });

        ValidateForm();
        if (!ModelState.IsValid)
            return Page();

        try
        {
            await SaveUploadedProfileImagesAsync(cancellationToken);

            if (ProfileId is null)
            {
                Profile = await _profileApiClient.CreateAsync(Form, cancellationToken);
                ProfileId = Profile.Id;
            }
            else
            {
                await _profileApiClient.UpdateAsync(ProfileId.Value, Form, cancellationToken);
                Profile = await _profileApiClient.GetMineAsync(cancellationToken);
            }

            if (Profile is not null)
                Form = ProfileFormModel.FromProfile(Profile);
                SyncPersianBirthDateFromForm();
            SuccessMessage = "پروفایل ذخیره شد.";
            return Page();
        }
        catch (Exception ex)
        {
            ErrorMessage = ToFriendlyMessage(ex);
            return Page();
        }
    }

    public IActionResult OnPostLogout()
    {
        _session.SignOut();
        return RedirectToPage("/Platform/Account/Login");
    }

    public string ProfileStatusText => Profile?.ProfileStatus switch
    {
        2 => "کامل",
        1 => "آماده خرید",
        0 => "ناقص",
        _ => "هنوز ساخته نشده"
    };

    private async Task LoadProfileAsync(CancellationToken cancellationToken)
    {
        try
        {
            Profile = await _profileApiClient.GetMineAsync(cancellationToken);
            if (Profile is not null)
            {
                ProfileId = Profile.Id;
                Form = ProfileFormModel.FromProfile(Profile);
                SyncPersianBirthDateFromForm();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ToFriendlyMessage(ex);
        }
    }

    private void ValidateForm()
    {
        if (!TrySetGregorianBirthDate())
            ModelState.AddModelError("PersianBirthDay", "تاریخ تولد شمسی معتبر نیست.");

        NormalizePhotoSelection();
        ValidateProfileImages();

        if (string.IsNullOrWhiteSpace(Form.DisplayName))
            ModelState.AddModelError("Form.DisplayName", "نام نمایشی را وارد کن.");
        if (Form.HeightCm is < 140 or > 210)
            ModelState.AddModelError("Form.HeightCm", "قد باید بین ۱۴۰ تا ۲۱۰ سانتی‌متر باشد.");
        if (Form.DateOfBirth > DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-18)))
            ModelState.AddModelError("Form.DateOfBirth", "سن باید حداقل ۱۸ سال باشد.");
        if (Form.EducationLevel == 0)
            ModelState.AddModelError("Form.EducationLevel", "مدرک تحصیلی را انتخاب کن.");
        if (Form.Gender is not 2 and not 3)
            ModelState.AddModelError("Form.Gender", "جنسیت را انتخاب کن.");
        if (Form.ZodiacSignId is null or < 1 or > 12)
            ModelState.AddModelError("Form.ZodiacSignId", "نشان زودیاک را انتخاب کن.");
        if (Form.SelectedInterestNames.Count > 4)
            ModelState.AddModelError("Form.SelectedInterestNames", "حداکثر ۴ علاقه‌مندی انتخاب کن.");
    }

    private bool TrySetGregorianBirthDate()
    {
        try
        {
            var calendar = new PersianCalendar();
            var dateTime = calendar.ToDateTime(PersianBirthYear, PersianBirthMonth, PersianBirthDay, 0, 0, 0, 0);
            Form.DateOfBirth = DateOnly.FromDateTime(dateTime);
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
    }

    private void ValidateProfileImages()
    {
        var files = ProfileImageFiles.Where(file => file.Length > 0).ToList();
        if (files.Count == 0)
            return;

        if (Form.PhotoUrls.Count + files.Count > 3)
            ModelState.AddModelError("ProfileImageFiles", "حداکثر ۳ عکس پروفایل می‌توانی داشته باشی.");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        foreach (var file in files)
        {
            if (file.Length > 3 * 1024 * 1024)
                ModelState.AddModelError("ProfileImageFiles", "حجم هر عکس باید حداکثر ۳ مگابایت باشد.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                ModelState.AddModelError("ProfileImageFiles", "فرمت عکس‌ها باید JPG، PNG یا WebP باشد.");
        }
    }

    private async Task SaveUploadedProfileImagesAsync(CancellationToken cancellationToken)
    {
        var files = ProfileImageFiles.Where(file => file.Length > 0).Take(3 - Form.PhotoUrls.Count).ToList();
        if (files.Count == 0)
            return;

        var absoluteDirectory = Path.Combine(
            Path.GetFullPath(Path.Combine(
                _environment.ContentRootPath,
                "..",
                "Randevoo.AdminPanel",
                "wwwroot",
                "uploads")),
            "profiles");
        Directory.CreateDirectory(absoluteDirectory);

        foreach (var file in files)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{_session.GetUserId() ?? 0}-{Guid.NewGuid():N}{extension}";
            var absolutePath = Path.Combine(absoluteDirectory, fileName);
            await using var stream = System.IO.File.Create(absolutePath);
            await file.CopyToAsync(stream, cancellationToken);

            Form.PhotoUrls.Add($"/uploads/profiles/{fileName}");
        }

        NormalizePhotoSelection();
    }

    private void SyncPersianBirthDateFromForm()
    {
        var calendar = new PersianCalendar();
        var dateTime = Form.DateOfBirth.ToDateTime(TimeOnly.MinValue);
        PersianBirthYear = calendar.GetYear(dateTime);
        PersianBirthMonth = calendar.GetMonth(dateTime);
        PersianBirthDay = calendar.GetDayOfMonth(dateTime);
    }

    private static string ToFriendlyMessage(Exception ex)
    {
        if (ex is UnauthorizedAccessException)
            return "برای ادامه باید وارد شوید.";
        if (ex.Message.Contains("Duplicate", StringComparison.OrdinalIgnoreCase))
            return "این نام نمایشی قبلاً استفاده شده است.";
        return "ذخیره پروفایل انجام نشد.";
    }

    private void NormalizePhotoSelection()
    {
        Form.Country = "Iran";
        Form.PhotoUrls = Form.PhotoUrls
            .Where(url => !string.IsNullOrWhiteSpace(url))
            .Select(url => url.Trim().Replace('\\', '/'))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(3)
            .ToList();

        if (Form.PhotoUrls.Count == 0)
        {
            Form.PrimaryImageUrl = null;
            return;
        }

        if (string.IsNullOrWhiteSpace(Form.PrimaryImageUrl) ||
            !Form.PhotoUrls.Any(url => url.Equals(Form.PrimaryImageUrl, StringComparison.OrdinalIgnoreCase)))
        {
            Form.PrimaryImageUrl = Form.PhotoUrls[0];
        }
    }

    public sealed record SelectOption(int Value, string Label, string? Code = null);
    public sealed record ChoiceOption(string Value, string Label, string Icon);
    public sealed record InterestOption(string Name, string Icon);
    public sealed record InterestGroup(string Title, IReadOnlyList<InterestOption> Items);
}
