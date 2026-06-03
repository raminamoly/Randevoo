using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.ValueObjects;

namespace Randevoo.Infrastructure.Data;

public static class RandevooSampleDataSeeder
{
    public static async Task MigrateAndSeedSampleDataAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
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

        var admin = await EnsureUserAsync(db, "09125177721", UserRole.Admin, cancellationToken);
        var planner = await EnsureUserAsync(db, "09125550000", UserRole.EventPlanner, cancellationToken);
        var guestOne = await EnsureUserAsync(db, "09123334455", UserRole.EndUser, cancellationToken);
        var guestTwo = await EnsureUserAsync(db, "09124445566", UserRole.EndUser, cancellationToken);

        EnsureProfile(admin, "مدیر رندوو", new DateOnly(1988, 4, 12), Gender.Male, "تهران", "ونک");
        EnsureProfile(planner, "پویا فرهی", new DateOnly(1991, 7, 23), Gender.Male, "تهران", "ولیعصر");
        EnsureProfile(guestOne, "آرزو", new DateOnly(1997, 9, 2), Gender.Female, "تهران", "جردن");
        EnsureProfile(guestTwo, "کیان", new DateOnly(1995, 2, 14), Gender.Male, "تهران", "یوسف آباد");

        await db.SaveChangesAsync(cancellationToken);

        EnsurePlannerProfile(db, planner);
        EnsureBalance(db, admin, 50000000m, "شارژ اولیه مدیر");
        EnsureBalance(db, planner, 25000000m, "شارژ اولیه برگزارکننده");
        EnsureBalance(db, guestOne, 12000000m, "شارژ نمونه کاربر");
        EnsureBalance(db, guestTwo, 12000000m, "شارژ نمونه کاربر");

        await db.SaveChangesAsync(cancellationToken);

        var eventTypes = await EnsureEventTypesAsync(db, cancellationToken);

        if (!await db.DatingEvents.AnyAsync(item => item.Title == "شب اجتماعی تهران", cancellationToken))
        {
            var socialEvent = new DatingEvent(
                planner,
                "شب اجتماعی تهران",
                new Location("ایران", "تهران", new Coordinates(35.7219m, 51.3347m), "ولیعصر"),
                "تهران، خیابان ولیعصر، سالن شمال",
                DateTime.UtcNow.Date.AddDays(10).AddHours(15),
                DateTime.UtcNow.Date.AddDays(10).AddHours(19),
                eventTypes["شب اجتماعی"],
                new AgeRange(25, 35),
                new AgeRange(25, 35),
                40,
                40,
                80,
                950000m,
                new[] { "اجتماعی", "حضوری", "منتخب", "تهران" },
                "/images/logo.png",
                null,
                null,
                "<p>یک شب اجتماعی منتخب با مهمان های تایید شده، مدیریت ظرفیت و تجربه حرفه ای برای شروع گفتگوهای باکیفیت.</p>",
                12m);

            socialEvent.OpenForSell();

            var dinnerEvent = new DatingEvent(
                planner,
                "پیش نمایش شام روف تاپ",
                new Location("ایران", "تهران", new Coordinates(35.7200m, 51.3300m), "پردیس"),
                "تهران، پردیس، بام آبی",
                DateTime.UtcNow.Date.AddDays(18).AddHours(16),
                DateTime.UtcNow.Date.AddDays(18).AddHours(20),
                eventTypes["شام اختصاصی"],
                new AgeRange(20, 30),
                new AgeRange(20, 30),
                30,
                30,
                60,
                880000m,
                new[] { "شام", "روف تاپ", "ویژه", "تهران" },
                "/images/logo.png",
                null,
                null,
                "<p>این رویداد برای معرفی فضای شام روف تاپ طراحی شده و پیش از باز شدن فروش، توسط مدیر بازبینی می شود.</p>",
                15m);

            db.DatingEvents.AddRange(socialEvent, dinnerEvent);
            await db.SaveChangesAsync(cancellationToken);

            var plannerBalance = await db.BalanceAccounts.SingleAsync(item => item.UserId == planner.Id, cancellationToken);
            var guestOneBalance = await db.BalanceAccounts.SingleAsync(item => item.UserId == guestOne.Id, cancellationToken);
            var guestTwoBalance = await db.BalanceAccounts.SingleAsync(item => item.UserId == guestTwo.Id, cancellationToken);

            socialEvent.SellTicket(guestOne, guestOne.Profile!);
            socialEvent.SellTicket(guestTwo, guestTwo.Profile!);

            guestOneBalance.Debit(950000m, BalanceTransactionType.TicketPurchase, "خرید بلیت شب اجتماعی تهران", socialEvent.Id);
            guestTwoBalance.Debit(950000m, BalanceTransactionType.TicketPurchase, "خرید بلیت شب اجتماعی تهران", socialEvent.Id);
            plannerBalance.Credit(1900000m, BalanceTransactionType.EventPlannerIncome, "فروش نمونه بلیت رویداد", socialEvent.Id);

            db.DatingEvents.Update(socialEvent);
            db.BalanceAccounts.UpdateRange(plannerBalance, guestOneBalance, guestTwoBalance);
        }

        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task<Dictionary<string, EventType>> EnsureEventTypesAsync(RandevooDbContext db, CancellationToken cancellationToken)
    {
        var seeded = new Dictionary<string, string>
        {
            ["شب اجتماعی"] = "رویداد اجتماعی منتخب با ظرفیت کنترل شده و مهمان های تایید شده.",
            ["شام اختصاصی"] = "رویداد شام با اجرای رسمی، چینش مهمان ها و مدیریت کیفیت تجربه.",
            ["قرار قهوه"] = "رویداد سبک و کوتاه برای آشنایی اولیه در فضای کافه.",
            ["روف تاپ"] = "رویداد عصرانه یا شبانه در فضای روف تاپ با ظرفیت محدود.",
            ["ورکشاپ"] = "رویداد تجربه محور همراه با کارگاه، گفتگو و آشنایی هدفمند.",
            ["گالری"] = "رویداد فرهنگی یا هنری با تمرکز بر تعامل اجتماعی کنترل شده."
        };

        var existing = await db.EventTypes.ToListAsync(cancellationToken);
        foreach (var pair in seeded)
        {
            var eventType = existing.SingleOrDefault(item => item.Name == pair.Key);
            if (eventType is null)
            {
                eventType = new EventType(pair.Key, pair.Value);
                db.EventTypes.Add(eventType);
                existing.Add(eventType);
                continue;
            }

            if (!eventType.IsActive || !string.Equals(eventType.Description, pair.Value, StringComparison.Ordinal))
            {
                eventType.Update(pair.Key, pair.Value, true);
                db.EventTypes.Update(eventType);
            }
        }

        await db.SaveChangesAsync(cancellationToken);
        return existing.ToDictionary(item => item.Name, StringComparer.Ordinal);
    }

    private static async Task<User> EnsureUserAsync(RandevooDbContext db, string mobileNumber, UserRole role, CancellationToken cancellationToken)
    {
        var user = await db.Users
            .Include(item => item.Profile)
            .SingleOrDefaultAsync(item => item.MobileNumber == mobileNumber, cancellationToken);
        var isNew = user is null;

        if (isNew)
        {
            user = new User(mobileNumber);
            db.Users.Add(user);
        }

        user ??= new User(mobileNumber);

        if (user.Role != role)
        {
            user.ChangeUserRole(role);
            if (!isNew)
            {
                db.Users.Update(user);
            }
        }

        return user;
    }

    private static void EnsureProfile(User user, string displayName, DateOnly birthDate, Gender gender, string city, string region)
    {
        if (user.Profile is not null)
        {
            return;
        }

        user.CreateProfile(
            displayName,
            birthDate,
            gender,
            new Location("ایران", city, new Coordinates(35.7219m, 51.3347m), region),
            new Height(172));
    }

    private static void EnsurePlannerProfile(RandevooDbContext db, User planner)
    {
        if (db.EventPlannerProfiles.Any(item => item.UserId == planner.Id))
        {
            return;
        }

        var profile = new EventPlannerProfile(
            planner,
            "برگزارکننده رویدادهای اجتماعی و شام های منتخب",
            "/images/logo.png",
            "پویا فرهی بیش از هفت سال در طراحی و اجرای رویدادهای اجتماعی خصوصی، شب های آشنایی و تجربه های گفتگو محور فعالیت داشته است و روی انتخاب مهمان های مناسب، زمان بندی دقیق و اجرای حرفه ای تمرکز دارد.");

        profile.UpdateMetrics(4.8m, 126, 34, 1, 29);
        db.EventPlannerProfiles.Add(profile);
    }

    private static void EnsureBalance(RandevooDbContext db, User user, decimal initialCredit, string description)
    {
        if (db.BalanceAccounts.Any(item => item.UserId == user.Id))
        {
            return;
        }

        var account = new BalanceAccount(user);
        if (initialCredit > 0)
        {
            account.Credit(initialCredit, BalanceTransactionType.AdminAdjustment, description);
        }

        db.BalanceAccounts.Add(account);
    }
}
