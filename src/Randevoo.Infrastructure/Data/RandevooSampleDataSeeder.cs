using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;
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

        EnsureProfile(admin, "مدیر راندوو", new DateOnly(1988, 4, 12), Gender.Male, "تهران", "ونک");
        EnsureProfile(planner, "پویا فرهی", new DateOnly(1991, 7, 23), Gender.Male, "تهران", "ولیعصر");
        EnsureProfile(guestOne, "آرزو", new DateOnly(1997, 9, 2), Gender.Female, "تهران", "جردن");
        EnsureProfile(guestTwo, "کیان", new DateOnly(1995, 2, 14), Gender.Male, "تهران", "یوسف آباد");
        guestOne.Profile?.UpdateEducationLevel(EducationLevel.Graduated);
        guestTwo.Profile?.UpdateEducationLevel(EducationLevel.Graduated);

        await db.SaveChangesAsync(cancellationToken);

        EnsurePlannerProfile(db, planner);
        EnsureBalance(db, admin, 50000000m, "شارژ اولیه مدیر");
        EnsureBalance(db, planner, 25000000m, "شارژ اولیه برگزارکننده");
        EnsureBalance(db, guestOne, 12000000m, "شارژ نمونه کاربر");
        EnsureBalance(db, guestTwo, 12000000m, "شارژ نمونه کاربر");

        await db.SaveChangesAsync(cancellationToken);
        await EnsurePlannerBankAccountsAsync(db, planner, cancellationToken);

        var eventTypes = await EnsureEventTypesAsync(db, cancellationToken);
        var tags = await EnsureTagsAsync(db, cancellationToken);
        var sampleUsers = await EnsureSampleEndUsersAsync(db, cancellationToken);

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
                EventEducationLevelRestriction.WithoutLimit,
                new[] { "اجتماعی", "حضوری", "منتخب", "تهران" },
                "/images/logo.png",
                null,
                null,
                "<p>یک شب اجتماعی منتخب با مهمان های تایید شده، مدیریت ظرفیت و تجربه حرفه ای برای شروع گفتگوهای باکیفیت.</p>",
                12m);

            socialEvent.SetLocationLookup(1, 1);
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
                EventEducationLevelRestriction.BachelorOrHigher,
                new[] { "شام", "روف تاپ", "ویژه", "تهران" },
                "/images/logo.png",
                null,
                null,
                "<p>این رویداد برای معرفی فضای شام روف تاپ طراحی شده و پیش از باز شدن فروش، توسط مدیر بازبینی می شود.</p>",
                15m);

            dinnerEvent.SetLocationLookup(1, 1);
            db.DatingEvents.AddRange(socialEvent, dinnerEvent);
            await db.SaveChangesAsync(cancellationToken);
            socialEvent.ReplaceTags(tags.Values.Where(tag => tag.Name is "شب اجتماعی" or "بازی").ToList());
            dinnerEvent.ReplaceTags(tags.Values.Where(tag => tag.Name is "شام" or "روف تاپ").ToList());

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

        await EnsureSampleEventTagsAsync(db, tags, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);
        await EnsureSampleTicketsAsync(db, planner, sampleUsers.Append(guestOne).Append(guestTwo).ToList(), cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        await EnsureSampleSurveysAndConversationsAsync(db, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        await EnsureSampleOnlinePaymentsAsync(db, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        await EnsureEventSmsRequestsAsync(db, admin, planner, guestOne, guestTwo, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    private sealed record SampleUserProfileSeed(
        string Mobile,
        string DisplayName,
        DateOnly BirthDate,
        Gender Gender,
        EducationLevel Education,
        string City,
        string Region,
        int Height,
        bool Smoking,
        string ImageUrl,
        string[] Interests);

    private static async Task<IReadOnlyList<User>> EnsureSampleEndUsersAsync(RandevooDbContext db, CancellationToken cancellationToken)
    {
        var samples = new[]
        {
            new SampleUserProfileSeed("09120001001", "رامین", new DateOnly(1992, 5, 21), Gender.Male, EducationLevel.Postgraduate, "تهران", "فرشته", 181, false, "/images/sample-profiles/ramin.jpg", ["کافه", "موسیقی", "سفر", "گفتگو"]),
            new SampleUserProfileSeed("09120001002", "آرین", new DateOnly(1998, 9, 12), Gender.Male, EducationLevel.Graduated, "تهران", "زعفرانیه", 184, false, "/images/sample-profiles/arian.jpg", ["فیلم", "ورزش", "کافه", "تکنولوژی"]),
            new SampleUserProfileSeed("09120001003", "بهاره", new DateOnly(1996, 4, 9), Gender.Female, EducationLevel.Postgraduate, "تهران", "جردن", 168, false, "/images/sample-profiles/bahareh.jpg", ["هنر", "گالری", "کتاب", "سفر"]),
            new SampleUserProfileSeed("09120001004", "علی رضا", new DateOnly(1993, 11, 3), Gender.Male, EducationLevel.Graduated, "تهران", "نیاوران", 178, false, "/images/sample-profiles/alireza.jpg", ["شام", "بازی", "دوچرخه", "موسیقی"]),
            new SampleUserProfileSeed("09120001005", "شایان", new DateOnly(1999, 2, 17), Gender.Male, EducationLevel.Undergraduate, "تهران", "سعادت آباد", 183, false, "/images/sample-profiles/shayan.jpg", ["فوتبال", "فیلم", "قرار قهوه", "بازی"]),
            new SampleUserProfileSeed("09120001006", "یاسمن", new DateOnly(1997, 7, 28), Gender.Female, EducationLevel.Graduated, "تهران", "ونک", 166, false, "/images/sample-profiles/yasaman.jpg", ["یوگا", "کتاب", "کافه", "رویداد هنری"])
        };

        var users = new List<User>();
        foreach (var sample in samples)
        {
            var user = await EnsureUserAsync(db, sample.Mobile, UserRole.EndUser, cancellationToken);
            EnsureProfile(user, sample.DisplayName, sample.BirthDate, sample.Gender, sample.City, sample.Region);
            user.Profile!.UpdateDisplayName(sample.DisplayName);
            user.Profile.UpdateGender(sample.Gender);
            user.Profile.UpdateEducationLevel(sample.Education);
            user.Profile.UpdateHeight(new Height(sample.Height));
            user.Profile.SetSmoking(sample.Smoking);
            EnsureBalance(db, user, 15000000m, $"شارژ نمونه {sample.DisplayName}");
            users.Add(user);
        }

        await db.SaveChangesAsync(cancellationToken);

        var interestMap = await EnsureInterestsAsync(db, samples.SelectMany(item => item.Interests).Distinct().ToList(), cancellationToken);
        foreach (var sample in samples)
        {
            var user = await db.Users
                .Include(item => item.Profile)!.ThenInclude(profile => profile!.Interests)
                .Include(item => item.Profile)!.ThenInclude(profile => profile!.Images)
                .SingleAsync(item => item.MobileNumber == sample.Mobile, cancellationToken);
            var profile = user.Profile!;

            foreach (var interestName in sample.Interests)
            {
                if (!profile.Interests.Any(item => item.Name == interestName))
                {
                    profile.AddInterest(interestMap[interestName]);
                }
            }

            if (!profile.Images.Any(item => item.ImageUrl == sample.ImageUrl))
            {
                profile.AddImage(sample.ImageUrl, 1, true);
            }

            db.UserProfiles.Update(profile);
        }

        await db.SaveChangesAsync(cancellationToken);
        return users;
    }

    private static async Task<Dictionary<string, Interest>> EnsureInterestsAsync(RandevooDbContext db, IReadOnlyCollection<string> names, CancellationToken cancellationToken)
    {
        var existing = await db.Interests.IgnoreQueryFilters().Where(item => !item.IsDeleted).ToListAsync(cancellationToken);
        foreach (var name in names)
        {
            var interest = existing.SingleOrDefault(item => item.Name == name);
            if (interest is null)
            {
                interest = new Interest(name, "سبک زندگی");
                db.Interests.Add(interest);
                existing.Add(interest);
            }
        }

        await db.SaveChangesAsync(cancellationToken);
        return existing.ToDictionary(item => item.Name, StringComparer.Ordinal);
    }

    private static async Task EnsureSampleTicketsAsync(RandevooDbContext db, User planner, IReadOnlyList<User> sampleUsers, CancellationToken cancellationToken)
    {
        var events = await db.DatingEvents
            .Include(item => item.Tickets)
            .Include(item => item.EventPlannerUser)
            .Where(item => item.Title == "شب اجتماعی تهران" || item.Title == "پیش نمایش شام روف تاپ")
            .OrderBy(item => item.Title)
            .ToListAsync(cancellationToken);
        if (events.Count == 0)
            return;

        foreach (var datingEvent in events)
        {
            if (!datingEvent.IsOpenForSell)
                datingEvent.OpenForSell();
        }

        foreach (var user in sampleUsers)
        {
            var userWithProfile = await db.Users
                .Include(item => item.Profile)
                .SingleAsync(item => item.Id == user.Id, cancellationToken);
            if (userWithProfile.Profile is null)
                continue;

            foreach (var datingEvent in events)
            {
                if (datingEvent.Tickets.Any(ticket => ticket.UserId == userWithProfile.Id))
                    continue;

                var buyerBalance = await db.BalanceAccounts.SingleAsync(item => item.UserId == userWithProfile.Id, cancellationToken);
                var plannerBalance = await db.BalanceAccounts.SingleAsync(item => item.UserId == planner.Id, cancellationToken);
                EventTicket ticket;
                try
                {
                    ticket = datingEvent.SellTicket(userWithProfile, userWithProfile.Profile);
                }
                catch (BusinessRuleViolationException)
                {
                    continue;
                }

                buyerBalance.Debit(ticket.Price, BalanceTransactionType.TicketPurchase, $"خرید بلیت {datingEvent.Title}", datingEvent.Id);
                var plannerIncome = ticket.Price * (100 - datingEvent.EventPlannerCommissionPercent) / 100;
                plannerBalance.Credit(plannerIncome, BalanceTransactionType.EventPlannerIncome, $"درآمد بلیت {datingEvent.Title}", datingEvent.Id);

                db.DatingEvents.Update(datingEvent);
                db.BalanceAccounts.UpdateRange(buyerBalance, plannerBalance);
            }
        }
    }

    private static async Task EnsurePlannerBankAccountsAsync(RandevooDbContext db, User planner, CancellationToken cancellationToken)
    {
        if (await db.PlannerBankAccounts.AnyAsync(item => item.UserId == planner.Id, cancellationToken))
            return;

        db.PlannerBankAccounts.AddRange(
            new PlannerBankAccount(planner, "6037991234567890", "IR820540102680020817909002", "بانک پارسیان", true),
            new PlannerBankAccount(planner, "6274121234567890", "IR060120000000000123456789", "بانک اقتصاد نوین", false));
    }

    private static async Task EnsureSampleOnlinePaymentsAsync(RandevooDbContext db, CancellationToken cancellationToken)
    {
        var purchaseTransactions = await db.BalanceTransactions
            .Where(item => item.Type == BalanceTransactionType.TicketPurchase && item.DatingEventId != null)
            .OrderBy(item => item.CreatedAt)
            .ToListAsync(cancellationToken);

        foreach (var transaction in purchaseTransactions)
        {
            var trackingCode = $"SIM-{transaction.Id:000000}";
            if (await db.OnlinePayments.AnyAsync(item => item.BalanceTransactionId == transaction.Id || item.TrackingCode == trackingCode, cancellationToken))
                continue;

            var user = await db.Users.SingleAsync(item => item.Id == transaction.UserId, cancellationToken);
            var datingEvent = await db.DatingEvents
                .Include(item => item.Tickets)
                .SingleAsync(item => item.Id == transaction.DatingEventId!.Value, cancellationToken);
            var ticket = datingEvent.Tickets.FirstOrDefault(item => item.UserId == transaction.UserId);

            db.OnlinePayments.Add(new OnlinePayment(
                user,
                Math.Abs(transaction.Amount),
                "درگاه نمونه زرین پال",
                trackingCode,
                OnlinePaymentStatus.Succeeded,
                datingEvent,
                ticket,
                transaction));
        }
    }

    private static async Task EnsureSampleSurveysAndConversationsAsync(RandevooDbContext db, CancellationToken cancellationToken)
    {
        var socialEvent = await db.DatingEvents
            .Include(item => item.Tickets)
            .SingleOrDefaultAsync(item => item.Title == "شب اجتماعی تهران", cancellationToken);
        if (socialEvent is null)
            return;

        var participantIds = socialEvent.Tickets
            .Where(item => !item.IsRefunded && !item.IsRemoved)
            .Select(item => item.UserId)
            .Distinct()
            .Take(4)
            .ToList();
        if (participantIds.Count < 2)
            return;

        var participants = await db.Users
            .Include(item => item.Profile)
            .Where(item => participantIds.Contains(item.Id))
            .ToListAsync(cancellationToken);

        foreach (var participant in participants.Take(3))
        {
            if (await db.EventSurveyResponses.AnyAsync(item => item.DatingEventId == socialEvent.Id && item.UserId == participant.Id, cancellationToken))
                continue;

            db.EventSurveyResponses.Add(new EventSurveyResponse(
                socialEvent,
                participant,
                new[]
                {
                    new EventSurveyRatingInput(SurveyFactor.OverallExperience, 5),
                    new EventSurveyRatingInput(SurveyFactor.EventOrganization, 4),
                    new EventSurveyRatingInput(SurveyFactor.VenueAndLocation, 5),
                    new EventSurveyRatingInput(SurveyFactor.ParticipantQuality, 4),
                    new EventSurveyRatingInput(SurveyFactor.SafetyAndComfort, 5)
                },
                "فضا صمیمی و کنترل ورود خیلی منظم بود. برای رویداد بعدی زمان گفتگوها کمی بیشتر باشد بهتر است."));
        }

        var first = participants[0];
        var second = participants[1];
        if (!await db.EventConversations.AnyAsync(item => item.DatingEventId == socialEvent.Id && item.StarterUserId == first.Id && item.ParticipantUserId == second.Id, cancellationToken))
        {
            var conversation = new EventConversation(socialEvent, first, second);
            conversation.SendMessage(first.Id, "سلام، از گفتگوی دیشب خوشحال شدم. امیدوارم رویداد خوبی گذشته باشد.");
            conversation.SendMessage(second.Id, "سلام، من هم همینطور. اجرای برنامه منظم و جالب بود.");
            conversation.SendMessage(first.Id, "اگر رویداد کافه بعدی برگزار شد شاید دوباره شرکت کنم.");
            db.EventConversations.Add(conversation);
        }
    }

    private static async Task EnsureSampleEventTagsAsync(RandevooDbContext db, Dictionary<string, Tag> tags, CancellationToken cancellationToken)
    {
        var socialEvent = await db.DatingEvents
            .Include(item => item.EventTags)
            .SingleOrDefaultAsync(item => item.Title == "شب اجتماعی تهران", cancellationToken);
        if (socialEvent is not null && socialEvent.EventTags.Count == 0)
        {
            socialEvent.ReplaceTags(tags.Values.Where(tag => tag.Name is "شب اجتماعی" or "بازی").ToList());
            db.DatingEvents.Update(socialEvent);
        }

        var dinnerEvent = await db.DatingEvents
            .Include(item => item.EventTags)
            .SingleOrDefaultAsync(item => item.Title == "پیش نمایش شام روف تاپ", cancellationToken);
        if (dinnerEvent is not null && dinnerEvent.EventTags.Count == 0)
        {
            dinnerEvent.ReplaceTags(tags.Values.Where(tag => tag.Name is "شام" or "روف تاپ").ToList());
            db.DatingEvents.Update(dinnerEvent);
        }
    }

    private static async Task<Dictionary<string, Tag>> EnsureTagsAsync(RandevooDbContext db, CancellationToken cancellationToken)
    {
        var names = new[]
        {
            "شب اجتماعی",
            "شام",
            "کافه",
            "بازی",
            "هنر",
            "کارگاه",
            "موسیقی",
            "روف تاپ"
        };

        var existing = await db.Tags.IgnoreQueryFilters().Where(item => !item.IsDeleted).ToListAsync(cancellationToken);
        foreach (var name in names)
        {
            var tag = existing.SingleOrDefault(item => item.Name == name);
            if (tag is null)
            {
                tag = new Tag(name);
                db.Tags.Add(tag);
                existing.Add(tag);
                continue;
            }

            if (!tag.IsActive)
            {
                tag.Update(tag.Name, true);
                db.Tags.Update(tag);
            }
        }

        await db.SaveChangesAsync(cancellationToken);
        return existing.ToDictionary(item => item.Name, StringComparer.Ordinal);
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
            user.Profile.UpdateDateOfBirth(birthDate);
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

    private static async Task EnsureEventSmsRequestsAsync(
        RandevooDbContext db,
        User admin,
        User planner,
        User guestOne,
        User guestTwo,
        CancellationToken cancellationToken)
    {
        var socialEvent = await db.DatingEvents
            .SingleOrDefaultAsync(item => item.Title == "شب اجتماعی تهران", cancellationToken);
        var dinnerEvent = await db.DatingEvents
            .SingleOrDefaultAsync(item => item.Title == "پیش نمایش شام روف تاپ", cancellationToken);

        if (socialEvent is not null && !await db.EventParticipantSmsRequests.AnyAsync(item => item.DatingEventId == socialEvent.Id, cancellationToken))
        {
            var pendingScheduled = new EventParticipantSmsRequest(
                planner,
                socialEvent,
                "سلام. یادآوری می کنیم که ورود مهمان ها از 18:30 آغاز می شود و لطفا کمی زودتر در محل حاضر باشید.",
                DateTime.UtcNow.AddDays(2));

            var approvedEdited = new EventParticipantSmsRequest(
                planner,
                socialEvent,
                "سلام. رویداد شما فردا برگزار می شود. برای هماهنگی بهتر، 15 دقیقه زودتر در محل حضور داشته باشید.");
            approvedEdited.Approve(
                admin.Id,
                2,
                "سلام. رویداد شب اجتماعی تهران فردا برگزار می شود. لطفا 15 دقیقه زودتر در محل حضور داشته باشید.",
                null,
                "متن برای شفافیت زمان حضور توسط مدیر ویرایش شد.");

            db.EventParticipantSmsRequests.AddRange(pendingScheduled, approvedEdited);
            await db.SaveChangesAsync(cancellationToken);

            var approvedMessage = approvedEdited.GetEffectiveMessage();
            db.SmsQueueItems.AddRange(
                new SmsQueueItem(guestOne, socialEvent, approvedMessage, null, approvedEdited.Id),
                new SmsQueueItem(guestTwo, socialEvent, approvedMessage, null, approvedEdited.Id));
        }

        if (dinnerEvent is not null && !await db.EventParticipantSmsRequests.AnyAsync(item => item.DatingEventId == dinnerEvent.Id, cancellationToken))
        {
            var rejectedRequest = new EventParticipantSmsRequest(
                planner,
                dinnerEvent,
                "سلام. برای این رویداد لطفا لباس رسمی تیره بپوشید و کارت شناسایی همراه داشته باشید.");
            rejectedRequest.Reject(admin.Id, "این پیام فعلا زودهنگام است و باید نزدیک تر به زمان رویداد ارسال شود.");
            db.EventParticipantSmsRequests.Add(rejectedRequest);
        }
    }
}
