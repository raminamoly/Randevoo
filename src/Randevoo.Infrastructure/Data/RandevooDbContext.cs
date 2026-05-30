
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
    public DbSet<Interest> Interests => Set<Interest>();
    public DbSet<EventPlannerProfile> EventPlannerProfiles => Set<EventPlannerProfile>();
    public DbSet<BalanceAccount> BalanceAccounts => Set<BalanceAccount>();
    public DbSet<BalanceTransaction> BalanceTransactions => Set<BalanceTransaction>();
    public DbSet<DatingEvent> DatingEvents => Set<DatingEvent>();
    public DbSet<EventTicket> EventTickets => Set<EventTicket>();
    public DbSet<EventConversation> EventConversations => Set<EventConversation>();
    public DbSet<EventChatMessage> EventChatMessages => Set<EventChatMessage>();
    public DbSet<EventChatBlock> EventChatBlocks => Set<EventChatBlock>();
    public DbSet<EventSurveyResponse> EventSurveyResponses => Set<EventSurveyResponse>();
    public DbSet<EventSurveyRating> EventSurveyRatings => Set<EventSurveyRating>();
    public DbSet<EventType> EventTypes => Set<EventType>();
    public DbSet<ModerationReport> ModerationReports => Set<ModerationReport>();

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

        modelBuilder.Entity<EventPlannerProfile>(b =>
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.Title).IsRequired().HasMaxLength(100);
            b.Property(p => p.PictureUrl).HasMaxLength(500);
            b.Property(p => p.Resume).IsRequired().HasMaxLength(4000);
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

        // UserProfile
        modelBuilder.Entity<UserProfile>(b =>
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.DisplayName).IsRequired().HasMaxLength(50);
            b.HasIndex(p => p.DisplayName).IsUnique();
            b.HasIndex(p => p.UserId).IsUnique();
            b.HasQueryFilter(p => !p.IsDeleted);
            b.OwnsOne(p => p.Height, hb =>
            {
                hb.Property(h => h.Centimeters)
                    .HasColumnName("HeightCentimeters")
                    .IsRequired();
            });

            b.OwnsOne(p => p.Location, lb =>
            {
                lb.Property(l => l.Country).HasColumnName("Location_Country").HasMaxLength(100).IsRequired();
                lb.Property(l => l.City).HasColumnName("Location_City").HasMaxLength(100).IsRequired();
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
            b.Property(e => e.EventType).IsRequired().HasMaxLength(100);
            b.Property(e => e.EventPlannerCommissionPercent).HasPrecision(5, 2).IsRequired();
            b.Property(e => e.TicketPrice).HasPrecision(18, 2).IsRequired();
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
                lb.Property(l => l.Country).HasColumnName("Location_Country").HasMaxLength(100).IsRequired();
                lb.Property(l => l.City).HasColumnName("Location_City").HasMaxLength(100).IsRequired();
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
    }
}
