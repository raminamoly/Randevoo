
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Randevoo.Domain.Entities;
using Randevoo.Domain.ValueObjects;

namespace Randevoo.Infrastructure.Data;

public class RandevooDbContext : DbContext
{
    public RandevooDbContext(DbContextOptions<RandevooDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<UserProfileImage> UserProfileImages => Set<UserProfileImage>();
    public DbSet<Interest> Interests => Set<Interest>();
    public DbSet<EventPlannerProfile> EventPlannerProfiles => Set<EventPlannerProfile>();
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<EducationLevelLookup> EducationLevels => Set<EducationLevelLookup>();
    public DbSet<GenderLookup> Genders => Set<GenderLookup>();
    public DbSet<BalanceAccount> BalanceAccounts => Set<BalanceAccount>();
    public DbSet<BalanceTransaction> BalanceTransactions => Set<BalanceTransaction>();
    public DbSet<PlannerWithdrawalRequest> PlannerWithdrawalRequests => Set<PlannerWithdrawalRequest>();
    public DbSet<DatingEvent> DatingEvents => Set<DatingEvent>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<EventTag> EventTags => Set<EventTag>();
    public DbSet<EventTicket> EventTickets => Set<EventTicket>();
    public DbSet<EventConversation> EventConversations => Set<EventConversation>();
    public DbSet<EventChatMessage> EventChatMessages => Set<EventChatMessage>();
    public DbSet<EventChatBlock> EventChatBlocks => Set<EventChatBlock>();
    public DbSet<EventSurveyResponse> EventSurveyResponses => Set<EventSurveyResponse>();
    public DbSet<EventSurveyRating> EventSurveyRatings => Set<EventSurveyRating>();
    public DbSet<EventType> EventTypes => Set<EventType>();
    public DbSet<ModerationReport> ModerationReports => Set<ModerationReport>();
    public DbSet<EventParticipantSmsRequest> EventParticipantSmsRequests => Set<EventParticipantSmsRequest>();
    public DbSet<SmsQueueItem> SmsQueueItems => Set<SmsQueueItem>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(b =>
        {
            b.HasKey(u => u.Id);
            b.Property(u => u.MobileNumber).IsRequired().HasMaxLength(20);
            b.HasIndex(u => u.MobileNumber).IsUnique();
            b.Property(u => u.Email).HasMaxLength(100);
            b.HasIndex(u => u.Email).IsUnique()
                .HasFilter("[Email] IS NOT NULL");
            b.Property(u => u.PendingEmail).HasMaxLength(100);
            b.Property(u => u.MobileLoginCodeHash).HasMaxLength(128);
            b.Property(u => u.EmailConfirmationTokenHash).HasMaxLength(128);
            b.HasQueryFilter(u => !u.IsDeleted);
            b.HasOne(u => u.Profile)
             .WithOne(p => p.User)
             .HasForeignKey<UserProfile>(p => p.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RefreshToken>(b =>
        {
            b.HasKey(token => token.Id);
            b.Property(token => token.TokenHash).IsRequired().HasMaxLength(128);
            b.Property(token => token.ReplacedByTokenHash).HasMaxLength(128);
            b.HasIndex(token => token.TokenHash).IsUnique();
            b.HasIndex(token => token.UserId);
            b.HasQueryFilter(token => !token.IsDeleted);
            b.HasOne(token => token.User)
                .WithMany()
                .HasForeignKey(token => token.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AuditLog>(b =>
        {
            b.HasKey(log => log.Id);
            b.Property(log => log.Action).IsRequired().HasMaxLength(120);
            b.Property(log => log.TargetType).IsRequired().HasMaxLength(120);
            b.Property(log => log.TargetId).IsRequired().HasMaxLength(120);
            b.Property(log => log.BeforeJson).HasMaxLength(8000);
            b.Property(log => log.AfterJson).HasMaxLength(8000);
            b.Property(log => log.Reason).HasMaxLength(1000);
            b.Property(log => log.IpAddress).HasMaxLength(64);
            b.Property(log => log.CorrelationId).HasMaxLength(100);
            b.HasIndex(log => log.ActorUserId);
            b.HasIndex(log => new { log.TargetType, log.TargetId });
            b.HasIndex(log => log.CreatedAt);
        });

        modelBuilder.Entity<Country>(b =>
        {
            b.HasKey(country => country.Id);
            b.Property(country => country.Name).IsRequired().HasMaxLength(100);
            b.Property(country => country.Code).IsRequired().HasMaxLength(10);
            b.Property(country => country.IsActive).IsRequired();
            b.Property(country => country.DisplayOrder).IsRequired();
            b.HasIndex(country => country.Name).IsUnique();
            b.HasIndex(country => country.Code).IsUnique();
            b.HasQueryFilter(country => !country.IsDeleted);
            b.HasMany(country => country.Cities)
                .WithOne(city => city.Country)
                .HasForeignKey(city => city.CountryId)
                .OnDelete(DeleteBehavior.Cascade);
            b.Navigation(country => country.Cities).UsePropertyAccessMode(PropertyAccessMode.Field);
            b.HasData(
                new { Id = 1L, Name = "ایران", Code = "IR", IsActive = true, DisplayOrder = 1, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 2L, Name = "امارات متحده عربی", Code = "AE", IsActive = true, DisplayOrder = 2, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 3L, Name = "ترکیه", Code = "TR", IsActive = true, DisplayOrder = 3, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false });
        });

        modelBuilder.Entity<City>(b =>
        {
            b.HasKey(city => city.Id);
            b.Property(city => city.Name).IsRequired().HasMaxLength(100);
            b.Property(city => city.IsActive).IsRequired();
            b.Property(city => city.DisplayOrder).IsRequired();
            b.Property(city => city.Latitude).HasPrecision(9, 6).IsRequired();
            b.Property(city => city.Longitude).HasPrecision(9, 6).IsRequired();
            b.HasIndex(city => new { city.CountryId, city.Name }).IsUnique();
            b.HasQueryFilter(city => !city.IsDeleted && !city.Country.IsDeleted);
            b.HasData(
                new { Id = 1L, CountryId = 1L, Name = "تهران", IsActive = true, DisplayOrder = 1, Latitude = 35.689200m, Longitude = 51.389000m, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 2L, CountryId = 1L, Name = "مشهد", IsActive = true, DisplayOrder = 2, Latitude = 36.260500m, Longitude = 59.616800m, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 3L, CountryId = 1L, Name = "شیراز", IsActive = true, DisplayOrder = 3, Latitude = 29.591800m, Longitude = 52.583700m, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 4L, CountryId = 1L, Name = "اصفهان", IsActive = true, DisplayOrder = 4, Latitude = 32.654600m, Longitude = 51.668000m, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 5L, CountryId = 1L, Name = "تبریز", IsActive = true, DisplayOrder = 5, Latitude = 38.096200m, Longitude = 46.273800m, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 6L, CountryId = 2L, Name = "دبی", IsActive = true, DisplayOrder = 1, Latitude = 25.204800m, Longitude = 55.270800m, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 7L, CountryId = 2L, Name = "ابوظبی", IsActive = true, DisplayOrder = 2, Latitude = 24.453900m, Longitude = 54.377300m, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 8L, CountryId = 3L, Name = "استانبول", IsActive = true, DisplayOrder = 1, Latitude = 41.008200m, Longitude = 28.978400m, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 9L, CountryId = 3L, Name = "آنکارا", IsActive = true, DisplayOrder = 2, Latitude = 39.933400m, Longitude = 32.859700m, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false });
        });

        modelBuilder.Entity<EducationLevelLookup>(b =>
        {
            b.ToTable("EducationLevels");
            b.HasKey(level => level.Id);
            b.Property(level => level.Title).IsRequired().HasMaxLength(150);
            b.Property(level => level.Rank).IsRequired();
            b.Property(level => level.IsActive).IsRequired();
            b.Property(level => level.DisplayOrder).IsRequired();
            b.HasIndex(level => level.Title).IsUnique();
            b.HasQueryFilter(level => !level.IsDeleted);
            b.HasData(
                new { Id = 1L, Title = "ثبت نشده", Rank = 0, IsActive = true, DisplayOrder = 0, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 2L, Title = "دیپلم", Rank = 1, IsActive = true, DisplayOrder = 1, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 3L, Title = "لیسانس", Rank = 2, IsActive = true, DisplayOrder = 2, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 4L, Title = "فوق لیسانس", Rank = 3, IsActive = true, DisplayOrder = 3, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 5L, Title = "دکترای حرفه ای / PHD / پزشک / دندان پزشک", Rank = 4, IsActive = true, DisplayOrder = 4, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false });
        });

        modelBuilder.Entity<GenderLookup>(b =>
        {
            b.ToTable("Genders");
            b.HasKey(gender => gender.Id);
            b.Property(gender => gender.Title).IsRequired().HasMaxLength(50);
            b.Property(gender => gender.IsActive).IsRequired();
            b.Property(gender => gender.DisplayOrder).IsRequired();
            b.HasIndex(gender => gender.Title).IsUnique();
            b.HasQueryFilter(gender => !gender.IsDeleted);
            b.HasData(
                new { Id = 1L, Title = "نامشخص", IsActive = true, DisplayOrder = 0, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 2L, Title = "آقا", IsActive = true, DisplayOrder = 1, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 3L, Title = "خانم", IsActive = true, DisplayOrder = 2, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false });
        });

        modelBuilder.Entity<EventPlannerProfile>(b =>
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.Title).IsRequired().HasMaxLength(100);
            b.Property(p => p.PictureUrl).HasMaxLength(500);
            b.Property(p => p.Resume).IsRequired().HasMaxLength(4000);
            b.Property(p => p.PendingFullName).HasMaxLength(100);
            b.Property(p => p.PendingCity).HasMaxLength(100);
            b.Property(p => p.PendingTitle).HasMaxLength(100);
            b.Property(p => p.PendingPictureUrl).HasMaxLength(500);
            b.Property(p => p.PendingResume).HasMaxLength(4000);
            b.Property(p => p.PendingReviewNote).HasMaxLength(1000);
            b.Property(p => p.AverageRating).HasPrecision(3, 2).IsRequired();
            b.HasIndex(p => p.UserId).IsUnique();
            b.HasQueryFilter(p => !p.IsDeleted);
            b.HasOne(p => p.User)
                .WithOne()
                .HasForeignKey<EventPlannerProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BalanceAccount>(b =>
        {
            b.HasKey(a => a.Id);
            b.Property(a => a.Balance).HasPrecision(18, 2).IsRequired();
            b.HasIndex(a => a.UserId).IsUnique();
            b.HasQueryFilter(a => !a.IsDeleted);
            b.HasOne(a => a.User)
                .WithOne()
                .HasForeignKey<BalanceAccount>(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasMany(a => a.Transactions)
                .WithOne(t => t.BalanceAccount)
                .HasForeignKey(t => t.BalanceAccountId)
                .OnDelete(DeleteBehavior.Cascade);
            b.Navigation(a => a.Transactions).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<BalanceTransaction>(b =>
        {
            b.HasKey(t => t.Id);
            b.Property(t => t.Amount).HasPrecision(18, 2).IsRequired();
            b.Property(t => t.Description).IsRequired().HasMaxLength(300);
            b.Property(t => t.ReferenceType).HasMaxLength(100);
            b.HasIndex(t => t.UserId);
            b.HasQueryFilter(t => !t.BalanceAccount.IsDeleted);
        });

        modelBuilder.Entity<PlannerWithdrawalRequest>(b =>
        {
            b.HasKey(request => request.Id);
            b.Property(request => request.Amount).HasPrecision(18, 2).IsRequired();
            b.Property(request => request.Status).IsRequired();
            b.Property(request => request.ReviewNote).HasMaxLength(1000);
            b.HasIndex(request => request.UserId);
            b.HasIndex(request => new { request.Status, request.RequestedAtUtc });
            b.HasQueryFilter(request => !request.IsDeleted);
            b.HasOne(request => request.User)
                .WithMany()
                .HasForeignKey(request => request.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(request => request.ReviewedByAdminUser)
                .WithMany()
                .HasForeignKey(request => request.ReviewedByAdminUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // UserProfile
        modelBuilder.Entity<UserProfile>(b =>
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.DisplayName).IsRequired().HasMaxLength(50);
            b.Property(p => p.BirthMonth).IsRequired();
            b.Property(p => p.ZodiacSign).IsRequired().HasMaxLength(30);
            b.HasIndex(p => p.DisplayName).IsUnique();
            b.HasIndex(p => p.UserId).IsUnique();
            b.HasIndex(p => p.CountryId);
            b.HasIndex(p => p.CityId);
            b.HasIndex(p => p.EducationLevelId);
            b.HasIndex(p => p.GenderId);
            b.HasQueryFilter(p => !p.IsDeleted);
            b.HasOne(p => p.Country)
                .WithMany()
                .HasForeignKey(p => p.CountryId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(p => p.City)
                .WithMany()
                .HasForeignKey(p => p.CityId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(p => p.EducationLevelLookup)
                .WithMany()
                .HasForeignKey(p => p.EducationLevelId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(p => p.GenderLookup)
                .WithMany()
                .HasForeignKey(p => p.GenderId)
                .OnDelete(DeleteBehavior.Restrict);
            b.OwnsOne(p => p.Height, hb =>
            {
                hb.Property(h => h.Centimeters)
                    .HasColumnName("HeightCentimeters")
                    .IsRequired();
            });

            b.OwnsOne(p => p.Location, lb =>
            {
                lb.Ignore(l => l.Country);
                lb.Ignore(l => l.City);
                lb.Property(l => l.Region).HasColumnName("Location_Region").HasMaxLength(100);
                lb.OwnsOne(l => l.Coordinates, cb =>
                {
                    cb.Property(c => c.Latitude).HasColumnName("Location_Latitude").HasPrecision(9, 6).IsRequired();
                    cb.Property(c => c.Longitude).HasColumnName("Location_Longitude").HasPrecision(9, 6).IsRequired();
                });
            });

            b.HasMany(p => p.Interests)
             .WithMany(i => i.UserProfiles)
             .UsingEntity<Dictionary<string, object>>(
                 "UserProfileInterest",
                 r => r.HasOne<Interest>().WithMany().HasForeignKey("InterestId").OnDelete(DeleteBehavior.Cascade),
                 l => l.HasOne<UserProfile>().WithMany().HasForeignKey("UserProfileId").OnDelete(DeleteBehavior.Cascade),
                 j =>
                 {
                     j.HasKey("UserProfileId", "InterestId");
                     j.ToTable("UserProfileInterests");
                 });

            b.Navigation(p => p.Interests).UsePropertyAccessMode(PropertyAccessMode.Field);
            b.HasMany(p => p.Images)
                .WithOne(image => image.UserProfile)
                .HasForeignKey(image => image.UserProfileId)
                .OnDelete(DeleteBehavior.Cascade);
            b.Navigation(p => p.Images).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<UserProfileImage>(b =>
        {
            b.HasKey(image => image.Id);
            b.Property(image => image.ImageUrl).IsRequired().HasMaxLength(500);
            b.Property(image => image.DisplayOrder).IsRequired();
            b.Property(image => image.IsPrimary).IsRequired();
            b.HasIndex(image => new { image.UserProfileId, image.DisplayOrder }).IsUnique();
            b.HasQueryFilter(image => !image.IsDeleted && !image.UserProfile.IsDeleted);
        });

        // Interest
        modelBuilder.Entity<Interest>(b =>
        {
            b.HasKey(i => i.Id);
            b.Property(i => i.Name).IsRequired().HasMaxLength(50);
            b.HasIndex(i => i.Name).IsUnique();
            b.Property(i => i.Category).HasMaxLength(30);
            b.Property(i => i.UsageCount).IsRequired();
            b.HasQueryFilter(i => !i.IsDeleted);
        });

        modelBuilder.Entity<DatingEvent>(b =>
        {
            b.HasKey(e => e.Id);
            b.Property(e => e.Title).IsRequired().HasMaxLength(150);
            b.Property(e => e.Address).IsRequired().HasMaxLength(300);
            b.HasIndex(e => e.EventTypeId);
            b.HasIndex(e => e.CountryId);
            b.HasIndex(e => e.CityId);
            b.HasIndex(e => e.MinimumEducationLevelId);
            b.Property(e => e.EventPlannerCommissionPercent).HasPrecision(5, 2).IsRequired();
            b.Property(e => e.TicketPrice).HasPrecision(18, 2).IsRequired();
            b.Property(e => e.EducationLevelRestriction).IsRequired();
            b.Property(e => e.EventImage1).HasMaxLength(500);
            b.Property(e => e.EventImage2).HasMaxLength(500);
            b.Property(e => e.EventImage3).HasMaxLength(500);
            b.Property(e => e.EventDescriptionHtml).IsRequired().HasMaxLength(10000);
            b.HasIndex(e => e.EventPlannerUserId);
            b.HasQueryFilter(e => !e.IsDeleted);
            b.HasOne(e => e.EventPlannerUser)
                .WithMany()
                .HasForeignKey(e => e.EventPlannerUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(e => e.EventType)
                .WithMany()
                .HasForeignKey(e => e.EventTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(e => e.Country)
                .WithMany()
                .HasForeignKey(e => e.CountryId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(e => e.City)
                .WithMany()
                .HasForeignKey(e => e.CityId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(e => e.MinimumEducationLevel)
                .WithMany()
                .HasForeignKey(e => e.MinimumEducationLevelId)
                .OnDelete(DeleteBehavior.Restrict);

            b.OwnsOne(e => e.AgeRangeForMale, ar =>
            {
                ar.Property(a => a.Min).HasColumnName("MaleMinAge").IsRequired();
                ar.Property(a => a.Max).HasColumnName("MaleMaxAge").IsRequired();
            });
            b.OwnsOne(e => e.AgeRangeForFemale, ar =>
            {
                ar.Property(a => a.Min).HasColumnName("FemaleMinAge").IsRequired();
                ar.Property(a => a.Max).HasColumnName("FemaleMaxAge").IsRequired();
            });
            b.OwnsOne(e => e.Location, lb =>
            {
                lb.Ignore(l => l.Country);
                lb.Ignore(l => l.City);
                lb.Property(l => l.Region).HasColumnName("Location_Region").HasMaxLength(100);
                lb.OwnsOne(l => l.Coordinates, cb =>
                {
                    cb.Property(c => c.Latitude).HasColumnName("Location_Latitude").HasPrecision(9, 6).IsRequired();
                    cb.Property(c => c.Longitude).HasColumnName("Location_Longitude").HasPrecision(9, 6).IsRequired();
                });
            });
            b.HasMany(e => e.Tickets)
                .WithOne(t => t.DatingEvent)
                .HasForeignKey(t => t.DatingEventId)
                .OnDelete(DeleteBehavior.Cascade);
            b.Navigation(e => e.Tickets).UsePropertyAccessMode(PropertyAccessMode.Field);
            b.HasMany(e => e.EventTags)
                .WithOne(eventTag => eventTag.DatingEvent)
                .HasForeignKey(eventTag => eventTag.DatingEventId)
                .OnDelete(DeleteBehavior.Cascade);
            b.Navigation(e => e.EventTags).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<Tag>(b =>
        {
            b.HasKey(tag => tag.Id);
            b.Property(tag => tag.Name).IsRequired().HasMaxLength(50);
            b.Property(tag => tag.IsActive).IsRequired();
            b.HasIndex(tag => tag.Name).IsUnique();
            b.HasQueryFilter(tag => !tag.IsDeleted);
            b.Navigation(tag => tag.EventTags).UsePropertyAccessMode(PropertyAccessMode.Field);
            b.HasData(
                new { Id = 1L, Name = "شب اجتماعی", IsActive = true, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 2L, Name = "شام", IsActive = true, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 3L, Name = "کافه", IsActive = true, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 4L, Name = "بازی", IsActive = true, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 5L, Name = "هنر", IsActive = true, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 6L, Name = "کارگاه", IsActive = true, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 7L, Name = "موسیقی", IsActive = true, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 8L, Name = "روف تاپ", IsActive = true, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false });
        });

        modelBuilder.Entity<EventTag>(b =>
        {
            b.ToTable("EventTags");
            b.HasKey(eventTag => eventTag.Id);
            b.HasIndex(eventTag => new { eventTag.DatingEventId, eventTag.TagId }).IsUnique();
            b.HasIndex(eventTag => eventTag.TagId);
            b.HasQueryFilter(eventTag => !eventTag.DatingEvent.IsDeleted && !eventTag.Tag.IsDeleted);
            b.HasOne(eventTag => eventTag.Tag)
                .WithMany(tag => tag.EventTags)
                .HasForeignKey(eventTag => eventTag.TagId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EventTicket>(b =>
        {
            b.HasKey(t => t.Id);
            b.Property(t => t.Price).HasPrecision(18, 2).IsRequired();
            b.Property(t => t.RemovalReason).HasMaxLength(500);
            b.HasIndex(t => new { t.DatingEventId, t.UserId }).IsUnique();
            b.HasQueryFilter(t => !t.DatingEvent.IsDeleted);
            b.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EventConversation>(b =>
        {
            b.HasKey(c => c.Id);
            b.Property(c => c.DisabledReason).HasMaxLength(500);
            b.HasIndex(c => new { c.DatingEventId, c.StarterUserId, c.ParticipantUserId }).IsUnique();
            b.HasQueryFilter(c => !c.IsDeleted && !c.DatingEvent.IsDeleted);
            b.HasOne(c => c.DatingEvent)
                .WithMany()
                .HasForeignKey(c => c.DatingEventId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(c => c.StarterUser)
                .WithMany()
                .HasForeignKey(c => c.StarterUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(c => c.ParticipantUser)
                .WithMany()
                .HasForeignKey(c => c.ParticipantUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasMany(c => c.Messages)
                .WithOne(m => m.EventConversation)
                .HasForeignKey(m => m.EventConversationId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasMany(c => c.Blocks)
                .WithOne(block => block.EventConversation)
                .HasForeignKey(block => block.EventConversationId)
                .OnDelete(DeleteBehavior.Cascade);
            b.Navigation(c => c.Messages).UsePropertyAccessMode(PropertyAccessMode.Field);
            b.Navigation(c => c.Blocks).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<EventChatMessage>(b =>
        {
            b.HasKey(m => m.Id);
            b.Property(m => m.Body).IsRequired().HasMaxLength(2000);
            b.HasIndex(m => m.EventConversationId);
            b.HasQueryFilter(m => !m.EventConversation.IsDeleted);
            b.HasOne(m => m.SenderUser)
                .WithMany()
                .HasForeignKey(m => m.SenderUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EventChatBlock>(b =>
        {
            b.HasKey(block => block.Id);
            b.HasIndex(block => new { block.EventConversationId, block.BlockerUserId, block.BlockedUserId }).IsUnique();
            b.HasQueryFilter(block => !block.EventConversation.IsDeleted);
            b.HasOne(block => block.BlockerUser)
                .WithMany()
                .HasForeignKey(block => block.BlockerUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(block => block.BlockedUser)
                .WithMany()
                .HasForeignKey(block => block.BlockedUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EventSurveyResponse>(b =>
        {
            b.HasKey(response => response.Id);
            b.Property(response => response.Comment).HasMaxLength(2000);
            b.HasIndex(response => new { response.DatingEventId, response.UserId }).IsUnique();
            b.HasQueryFilter(response => !response.IsDeleted && !response.DatingEvent.IsDeleted);
            b.HasOne(response => response.DatingEvent)
                .WithMany()
                .HasForeignKey(response => response.DatingEventId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(response => response.User)
                .WithMany()
                .HasForeignKey(response => response.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasMany(response => response.Ratings)
                .WithOne(rating => rating.EventSurveyResponse)
                .HasForeignKey(rating => rating.EventSurveyResponseId)
                .OnDelete(DeleteBehavior.Cascade);
            b.Navigation(response => response.Ratings).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<EventSurveyRating>(b =>
        {
            b.HasKey(rating => rating.Id);
            b.HasIndex(rating => new { rating.EventSurveyResponseId, rating.Factor }).IsUnique();
            b.HasQueryFilter(rating => !rating.EventSurveyResponse.IsDeleted);
        });

        modelBuilder.Entity<EventType>(b =>
        {
            b.HasKey(type => type.Id);
            b.Property(type => type.Name).IsRequired().HasMaxLength(100);
            b.Property(type => type.Description).HasMaxLength(500);
            b.HasIndex(type => type.Name).IsUnique();
            b.HasQueryFilter(type => !type.IsDeleted);
            b.HasData(
                new { Id = 1L, Name = "Mafia", Description = "Social deduction event", IsActive = true, CreatedAt = new DateTime(2026, 5, 30, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 2L, Name = "Board Game", Description = "Board game social event", IsActive = true, CreatedAt = new DateTime(2026, 5, 30, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 3L, Name = "Poem Reading", Description = "Poetry and conversation event", IsActive = true, CreatedAt = new DateTime(2026, 5, 30, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 4L, Name = "Cafe Meetup", Description = "Casual cafe meetup", IsActive = true, CreatedAt = new DateTime(2026, 5, 30, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 5L, Name = "Hiking", Description = "Outdoor hiking event", IsActive = true, CreatedAt = new DateTime(2026, 5, 30, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 6L, Name = "Speed Dating", Description = "Structured short introductions", IsActive = true, CreatedAt = new DateTime(2026, 5, 30, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 7L, Name = "Game Tournament", Description = "Competitive game tournament", IsActive = true, CreatedAt = new DateTime(2026, 5, 30, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 8L, Name = "Workshop", Description = "Learning-focused social workshop", IsActive = true, CreatedAt = new DateTime(2026, 5, 30, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 9L, Name = "Art Night", Description = "Art and creativity event", IsActive = true, CreatedAt = new DateTime(2026, 5, 30, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 10L, Name = "Music Night", Description = "Music-focused social event", IsActive = true, CreatedAt = new DateTime(2026, 5, 30, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false });
        });

        modelBuilder.Entity<ModerationReport>(b =>
        {
            b.HasKey(report => report.Id);
            b.Property(report => report.Description).IsRequired().HasMaxLength(2000);
            b.Property(report => report.AdminReviewNote).HasMaxLength(2000);
            b.HasIndex(report => report.Status);
            b.HasIndex(report => report.ReporterUserId);
            b.HasIndex(report => report.ReportedUserId);
            b.HasQueryFilter(report => !report.IsDeleted);
            b.HasOne(report => report.ReporterUser)
                .WithMany()
                .HasForeignKey(report => report.ReporterUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(report => report.ReportedUser)
                .WithMany()
                .HasForeignKey(report => report.ReportedUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(report => report.DatingEvent)
                .WithMany()
                .HasForeignKey(report => report.DatingEventId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(report => report.EventConversation)
                .WithMany()
                .HasForeignKey(report => report.EventConversationId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(report => report.ReviewedByAdminUser)
                .WithMany()
                .HasForeignKey(report => report.ReviewedByAdminUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EventParticipantSmsRequest>(b =>
        {
            b.HasKey(request => request.Id);
            b.Property(request => request.Message).IsRequired().HasMaxLength(480);
            b.Property(request => request.ApprovedMessage).HasMaxLength(480);
            b.Property(request => request.PlannedSendAtUtc);
            b.Property(request => request.ReviewNote).HasMaxLength(1000);
            b.HasIndex(request => new { request.DatingEventId, request.Status, request.CreatedAt });
            b.HasIndex(request => request.PlannedSendAtUtc);
            b.HasQueryFilter(request => !request.IsDeleted);
            b.HasOne(request => request.DatingEvent)
                .WithMany()
                .HasForeignKey(request => request.DatingEventId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(request => request.RequestedByUser)
                .WithMany()
                .HasForeignKey(request => request.RequestedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(request => request.ReviewedByAdminUser)
                .WithMany()
                .HasForeignKey(request => request.ReviewedByAdminUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SmsQueueItem>(b =>
        {
            b.HasKey(item => item.Id);
            b.Property(item => item.MobileNumber).IsRequired().HasMaxLength(20);
            b.Property(item => item.Message).IsRequired().HasMaxLength(480);
            b.Property(item => item.PlannedSendAtUtc);
            b.Property(item => item.FailureReason).HasMaxLength(1000);
            b.HasIndex(item => new { item.Status, item.CreatedAt });
            b.HasIndex(item => item.EventParticipantSmsRequestId);
            b.HasIndex(item => item.PlannedSendAtUtc);
            b.HasQueryFilter(item => !item.IsDeleted);
            b.HasOne(item => item.DatingEvent)
                .WithMany()
                .HasForeignKey(item => item.DatingEventId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(item => item.RecipientUser)
                .WithMany()
                .HasForeignKey(item => item.RecipientUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(item => item.EventParticipantSmsRequest)
                .WithMany()
                .HasForeignKey(item => item.EventParticipantSmsRequestId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }
}
