using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.ValueObjects;

namespace Randevoo.Infrastructure.Data;

public static class RandevooDatabaseInitializer
{
    private const string AdminMobileNumber = "09125177721";
    private const string AdminDisplayName = "مدیر راندوو";

    public static async Task InitializeDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RandevooDbContext>();

        if (db.Database.IsRelational())
        {
            await db.Database.MigrateAsync(cancellationToken);
        }
        else
        {
            await db.Database.EnsureCreatedAsync(cancellationToken);
        }

        await EnsureAdminUserAsync(db, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        var environment = scope.ServiceProvider.GetService<IHostEnvironment>();
        if (environment?.IsDevelopment() == true)
        {
            await EnsureSampleModerationReportsAsync(db, cancellationToken);
            await EnsureSampleCheckoutEventsAsync(db, cancellationToken);
            await db.SaveChangesAsync(cancellationToken);
        }
    }

    private static async Task EnsureAdminUserAsync(RandevooDbContext db, CancellationToken cancellationToken)
    {
        var admin = await db.Users
            .Include(item => item.Profile)
            .SingleOrDefaultAsync(item => item.MobileNumber == AdminMobileNumber, cancellationToken);

        if (admin is null)
        {
            admin = new User(AdminMobileNumber);
            db.Users.Add(admin);
        }

        if (admin.Role != UserRole.Admin)
            admin.ChangeUserRole(UserRole.Admin);

        if (admin.Profile is null)
        {
            admin.CreateProfile(
                AdminDisplayName,
                new DateOnly(1988, 4, 12),
                Gender.Male,
                new Location("ایران", "تهران", new Coordinates(35.7219m, 51.3347m), "ونک"),
                new Height(172));
        }
        else
        {
            admin.Profile.UpdateDisplayName(AdminDisplayName);
        }
    }

    private static async Task EnsureSampleModerationReportsAsync(RandevooDbContext db, CancellationToken cancellationToken)
    {
        var reported = await GetOrCreateSampleUserAsync(
            db,
            "09120001001",
            "کاوه نمونه",
            Gender.Male,
            new DateOnly(1992, 3, 14),
            "https://i.pravatar.cc/160?img=12",
            cancellationToken);
        var reporter1 = await GetOrCreateSampleUserAsync(
            db,
            "09120001002",
            "نیلوفر گزارشگر",
            Gender.Female,
            new DateOnly(1994, 7, 21),
            "https://i.pravatar.cc/160?img=47",
            cancellationToken);
        var reporter2 = await GetOrCreateSampleUserAsync(
            db,
            "09120001003",
            "سامان گزارشگر",
            Gender.Male,
            new DateOnly(1990, 11, 6),
            "https://i.pravatar.cc/160?img=33",
            cancellationToken);
        var reported2 = await GetOrCreateSampleUserAsync(
            db,
            "09120001004",
            "رها نمونه",
            Gender.Female,
            new DateOnly(1995, 2, 2),
            "https://i.pravatar.cc/160?img=49",
            cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        if (!await db.ModerationReports.AnyAsync(report => report.ReportedUserId == reported.Id, cancellationToken))
        {
            db.ModerationReports.Add(new ModerationReport(
                reporter1,
                reported,
                ModerationReportReason.Harassment,
                "نمونه تستی: کاربر بعد از رویداد پیام‌های نامناسب ارسال کرده است."));
            db.ModerationReports.Add(new ModerationReport(
                reporter2,
                reported,
                ModerationReportReason.UnsafeBehavior,
                "نمونه تستی: رفتار کاربر در رویداد باعث نگرانی چند نفر شده است."));
        }

        if (!await db.ModerationReports.AnyAsync(report => report.ReportedUserId == reported2.Id, cancellationToken))
        {
            db.ModerationReports.Add(new ModerationReport(
                reporter1,
                reported2,
                ModerationReportReason.FakeProfile,
                "نمونه تستی: تصویر و اطلاعات پروفایل با حضور کاربر در رویداد تطابق نداشت."));
        }
    }

    private static async Task<User> GetOrCreateSampleUserAsync(
        RandevooDbContext db,
        string mobileNumber,
        string displayName,
        Gender gender,
        DateOnly dateOfBirth,
        string imageUrl,
        CancellationToken cancellationToken)
    {
        var user = await db.Users
            .Include(item => item.Profile)
            .ThenInclude(profile => profile!.Images)
            .SingleOrDefaultAsync(item => item.MobileNumber == mobileNumber, cancellationToken);

        if (user is null)
        {
            user = new User(mobileNumber);
            db.Users.Add(user);
        }

        if (user.Profile is null)
        {
            user.CreateProfile(
                displayName,
                dateOfBirth,
                gender,
                new Location("ایران", "تهران", new Coordinates(35.7219m, 51.3347m), "نمونه"),
                new Height(gender == Gender.Male ? 178 : 165));
            user.Profile!.AddImage(imageUrl, 1, true);
        }
        else
        {
            user.Profile.UpdateDisplayName(displayName);
            if (user.Profile.Images.Count == 0)
                user.Profile.AddImage(imageUrl, 1, true);
        }

        user.Profile!.UpdateEducationLevel(EducationLevel.Undergraduate);
        return user;
    }

    private static async Task EnsureSampleCheckoutEventsAsync(RandevooDbContext db, CancellationToken cancellationToken)
    {
        await GetOrCreateSampleUserAsync(db, "09120002001", "آرمان خریدار", Gender.Male, new DateOnly(1994, 5, 16), "https://i.pravatar.cc/160?img=11", cancellationToken);
        await GetOrCreateSampleUserAsync(db, "09120002002", "ترانه شرکت‌کننده", Gender.Female, new DateOnly(1996, 8, 9), "https://i.pravatar.cc/160?img=45", cancellationToken);
        await GetOrCreateSampleUserAsync(db, "09120002003", "مانی شرکت‌کننده", Gender.Male, new DateOnly(1991, 12, 3), "https://i.pravatar.cc/160?img=15", cancellationToken);

        var planner = await db.Users
            .Include(item => item.Profile)
            .SingleOrDefaultAsync(item => item.MobileNumber == "09120003001", cancellationToken);
        if (planner is null)
        {
            planner = new User("09120003001");
            planner.CreateProfile(
                "سینا برگزارکننده",
                new DateOnly(1988, 2, 19),
                Gender.Male,
                new Location("ایران", "تهران", new Coordinates(35.7219m, 51.3347m), "ونک"),
                new Height(178));
            db.Users.Add(planner);
        }

        if (planner.Profile is null)
        {
            planner.CreateProfile(
                "سینا برگزارکننده",
                new DateOnly(1988, 2, 19),
                Gender.Male,
                new Location("ایران", "تهران", new Coordinates(35.7219m, 51.3347m), "ونک"),
                new Height(178));
        }
        planner.Profile.UpdateEducationLevel(EducationLevel.Postgraduate);
        planner.BecomeEventPlanner();

        if (!await db.EventPlannerProfiles.AnyAsync(item => item.UserId == planner.Id, cancellationToken))
        {
            db.EventPlannerProfiles.Add(new EventPlannerProfile(
                planner,
                "استودیو تجربه‌های اجتماعی راندوو",
                "https://i.pravatar.cc/160?img=32",
                "طراحی و اجرای رویدادهای گفت‌وگو محور، بازی‌های گروهی و تجربه‌های اجتماعی امن برای آشنایی واقعی."));
        }

        var country = await db.Countries.SingleAsync(item => item.Id == 1L, cancellationToken);
        var tehran = await db.Cities.SingleAsync(item => item.Id == 1L, cancellationToken);
        var esfahan = await db.Cities.SingleAsync(item => item.Id == 4L, cancellationToken);
        var inPerson = await db.EventModes.SingleAsync(item => item.Id == 2L, cancellationToken);
        var online = await db.EventModes.SingleAsync(item => item.Id == 1L, cancellationToken);
        var zoom = await db.OnlineEventPlatforms.SingleAsync(item => item.Id == 2L, cancellationToken);

        var startBase = DateTime.UtcNow.Date.AddDays(14).AddHours(15);
        var samples = new[]
        {
            new SampleCheckoutEvent("کارگاه مکالمه و بازی‌های گروهی تهران", 8L, tehran, startBase, EventPaymentCollectionMethod.PlatformGateway, false, 960000m, 940000m, EventEducationLevelRestriction.BachelorOrHigher, "https://images.unsplash.com/photo-1529156069898-49953e39b3ac?auto=format&fit=crop&w=1200&q=80"),
            new SampleCheckoutEvent("شب شعر و گفت‌وگوی آرام", 3L, tehran, startBase.AddDays(3), EventPaymentCollectionMethod.PlatformManualTransfer, false, 890000m, 870000m, EventEducationLevelRestriction.WithoutLimit, "https://images.unsplash.com/photo-1511795409834-ef04bbd61622?auto=format&fit=crop&w=1200&q=80"),
            new SampleCheckoutEvent("آشنایی آنلاین برای آدم‌های کتاب‌خوان", 8L, tehran, startBase.AddDays(6), EventPaymentCollectionMethod.OrganizerManualTransfer, true, 620000m, 620000m, EventEducationLevelRestriction.DiplomaOrHigher, "https://images.unsplash.com/photo-1516321318423-f06f85e504b3?auto=format&fit=crop&w=1200&q=80"),
            new SampleCheckoutEvent("کافه‌گیم اصفهان: مافیا و معاشرت", 1L, esfahan, startBase.AddDays(9), EventPaymentCollectionMethod.PlatformGateway, false, 780000m, 760000m, EventEducationLevelRestriction.WithoutLimit, "https://images.unsplash.com/photo-1610890716171-6b1bb98ffd09?auto=format&fit=crop&w=1200&q=80"),
            new SampleCheckoutEvent("قدم‌زدن شهری و گفت‌وگوی سبک", 5L, tehran, startBase.AddDays(12), EventPaymentCollectionMethod.OrganizerManualTransfer, false, 690000m, 690000m, EventEducationLevelRestriction.WithoutLimit, "https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?auto=format&fit=crop&w=1200&q=80")
        };

        foreach (var sample in samples)
        {
            var eventType = await db.EventTypes.SingleAsync(item => item.Id == sample.EventTypeId, cancellationToken);
            var existing = await db.DatingEvents
                .Include(item => item.DiscountCodes)
                .SingleOrDefaultAsync(item => item.Title == sample.Title, cancellationToken);

            var location = new Location("ایران", sample.City.Name, new Coordinates(sample.City.Latitude, sample.City.Longitude), sample.City.Name == "تهران" ? "ونک" : "جلفا");
            var description = $"<p>{sample.Title} یک رویداد واقعی‌نما برای تست کامل خرید بلیت، ظرفیت، کد تخفیف و روش‌های پرداخت در محیط توسعه است.</p><p>فضا دوستانه است و اطلاعات شرکت‌کنندگان فقط بعد از شروع رویداد برای دارندگان بلیت معتبر باز می‌شود.</p>";
            if (existing is null)
            {
                existing = new DatingEvent(
                    planner,
                    sample.Title,
                    location,
                    sample.IsOnline ? "آنلاین" : $"{sample.City.Name}، خانه تجربه راندوو",
                    sample.StartsAtUtc,
                    sample.StartsAtUtc.AddHours(3),
                    eventType,
                    new AgeRange(24, 38),
                    new AgeRange(22, 36),
                    18,
                    18,
                    5,
                    sample.MalePrice,
                    sample.FemalePrice,
                    sample.EducationRestriction,
                    new[] { "گفتگو", "آشنایی", "تجربه" },
                    sample.ImageUrl,
                    null,
                    null,
                    description,
                    12,
                    "IRR",
                    "IRR",
                    sample.PaymentMethod,
                    sample.PaymentMethod == EventPaymentCollectionMethod.OrganizerManualTransfer ? "واریز به کارت ۶۱۰۴-۳۳۳۳-۲۲۲۲-۱۱۱۱ به نام استودیو تجربه‌های اجتماعی راندوو و سپس بارگذاری رسید." : null);
                db.DatingEvents.Add(existing);
            }
            else
            {
                existing.UpdateDetails(
                    sample.Title,
                    location,
                    sample.IsOnline ? "آنلاین" : $"{sample.City.Name}، خانه تجربه راندوو",
                    sample.StartsAtUtc,
                    sample.StartsAtUtc.AddHours(3),
                    eventType,
                    new AgeRange(24, 38),
                    new AgeRange(22, 36),
                    18,
                    18,
                    5,
                    sample.MalePrice,
                    sample.FemalePrice,
                    sample.EducationRestriction,
                    new[] { "گفتگو", "آشنایی", "تجربه" },
                    sample.ImageUrl,
                    null,
                    null,
                    description,
                    "IRR",
                    "IRR",
                    sample.PaymentMethod,
                    sample.PaymentMethod == EventPaymentCollectionMethod.OrganizerManualTransfer ? "واریز به کارت ۶۱۰۴-۳۳۳۳-۲۲۲۲-۱۱۱۱ به نام استودیو تجربه‌های اجتماعی راندوو و سپس بارگذاری رسید." : null);
            }

            existing.SetLocationLookup(country.Id, sample.City.Id);
            existing.SetEventDelivery(sample.IsOnline ? online : inPerson, sample.IsOnline ? zoom : null, sample.IsOnline ? "https://meet.randevoo.local/sample" : null, sample.IsOnline ? "لینک ورود پس از تایید پرداخت در اختیار شرکت‌کنندگان قرار می‌گیرد." : null);
            existing.ApproveByAdmin();
            existing.OpenForSell();
            EnsureDiscountCodes(existing);
        }
    }

    private static void EnsureDiscountCodes(DatingEvent datingEvent)
    {
        var now = DateTime.UtcNow;
        AddDiscountIfMissing(datingEvent, "WELCOME20", EventDiscountGenderScope.All, EventDiscountType.Percentage, 20, now.AddDays(-1), now.AddMonths(2), 100, true, "تخفیف خوش‌آمدگویی");
        AddDiscountIfMissing(datingEvent, "MENONLY15", EventDiscountGenderScope.Male, EventDiscountType.Percentage, 15, now.AddDays(-1), now.AddMonths(2), 50, true, "تخفیف آقایان");
        AddDiscountIfMissing(datingEvent, "WOMENONLY15", EventDiscountGenderScope.Female, EventDiscountType.Percentage, 15, now.AddDays(-1), now.AddMonths(2), 50, true, "تخفیف خانم‌ها");
        AddDiscountIfMissing(datingEvent, "EXPIRED10", EventDiscountGenderScope.All, EventDiscountType.Percentage, 10, now.AddMonths(-2), now.AddMonths(-1), 50, true, "کد منقضی");
        AddDiscountIfMissing(datingEvent, "INACTIVE10", EventDiscountGenderScope.All, EventDiscountType.Percentage, 10, now.AddDays(-1), now.AddMonths(2), 50, false, "کد غیرفعال");
    }

    private static void AddDiscountIfMissing(
        DatingEvent datingEvent,
        string code,
        EventDiscountGenderScope genderScope,
        EventDiscountType discountType,
        decimal value,
        DateTime startsAtUtc,
        DateTime endsAtUtc,
        int maxUsageCount,
        bool isActive,
        string title)
    {
        if (datingEvent.DiscountCodes.Any(item => item.Code == code))
            return;

        datingEvent.AddDiscountCode(code, genderScope, discountType, value, startsAtUtc, endsAtUtc, maxUsageCount, isActive, title);
    }

    private sealed record SampleCheckoutEvent(
        string Title,
        long EventTypeId,
        City City,
        DateTime StartsAtUtc,
        EventPaymentCollectionMethod PaymentMethod,
        bool IsOnline,
        decimal MalePrice,
        decimal FemalePrice,
        EventEducationLevelRestriction EducationRestriction,
        string ImageUrl);
}
