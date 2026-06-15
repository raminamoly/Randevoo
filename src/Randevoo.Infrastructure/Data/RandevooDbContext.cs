
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
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
    public DbSet<ZodiacSignLookup> ZodiacSigns => Set<ZodiacSignLookup>();
    public DbSet<UserRoleLookup> UserRoles => Set<UserRoleLookup>();
    public DbSet<EventReviewStatusLookup> EventReviewStatuses => Set<EventReviewStatusLookup>();
    public DbSet<EventApprovalStatusLookup> EventApprovalStatuses => Set<EventApprovalStatusLookup>();
    public DbSet<EventSaleStatusLookup> EventSaleStatuses => Set<EventSaleStatusLookup>();
    public DbSet<EventLifecycleStatusLookup> EventLifecycleStatuses => Set<EventLifecycleStatusLookup>();
    public DbSet<EventWorkflowActionTypeLookup> EventWorkflowActionTypes => Set<EventWorkflowActionTypeLookup>();
    public DbSet<EventRequestStatusLookup> EventRequestStatuses => Set<EventRequestStatusLookup>();
    public DbSet<EventDiscountTypeLookup> EventDiscountTypes => Set<EventDiscountTypeLookup>();
    public DbSet<BalanceTransactionTypeLookup> BalanceTransactionTypes => Set<BalanceTransactionTypeLookup>();
    public DbSet<CurrencyLookup> Currencies => Set<CurrencyLookup>();
    public DbSet<CurrencyExchangeRate> CurrencyExchangeRates => Set<CurrencyExchangeRate>();
    public DbSet<BalanceAccount> BalanceAccounts => Set<BalanceAccount>();
    public DbSet<BalanceTransaction> BalanceTransactions => Set<BalanceTransaction>();
    public DbSet<OnlinePayment> OnlinePayments => Set<OnlinePayment>();
    public DbSet<ManualPaymentReceipt> ManualPaymentReceipts => Set<ManualPaymentReceipt>();
    public DbSet<TicketRefundRequest> TicketRefundRequests => Set<TicketRefundRequest>();
    public DbSet<PlannerWithdrawalRequest> PlannerWithdrawalRequests => Set<PlannerWithdrawalRequest>();
    public DbSet<PlannerBankAccount> PlannerBankAccounts => Set<PlannerBankAccount>();
    public DbSet<DatingEvent> DatingEvents => Set<DatingEvent>();
    public DbSet<TicketOrder> TicketOrders => Set<TicketOrder>();
    public DbSet<EventModeLookup> EventModes => Set<EventModeLookup>();
    public DbSet<OnlineEventPlatform> OnlineEventPlatforms => Set<OnlineEventPlatform>();
    public DbSet<EventFaq> EventFaqs => Set<EventFaq>();
    public DbSet<EventDiscountCode> EventDiscountCodes => Set<EventDiscountCode>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<EventTag> EventTags => Set<EventTag>();
    public DbSet<EventTicket> EventTickets => Set<EventTicket>();
    public DbSet<EventLike> EventLikes => Set<EventLike>();
    public DbSet<EventConversation> EventConversations => Set<EventConversation>();
    public DbSet<EventChatMessage> EventChatMessages => Set<EventChatMessage>();
    public DbSet<EventChatBlock> EventChatBlocks => Set<EventChatBlock>();
    public DbSet<EventSurveyResponse> EventSurveyResponses => Set<EventSurveyResponse>();
    public DbSet<EventSurveyRating> EventSurveyRatings => Set<EventSurveyRating>();
    public DbSet<EventType> EventTypes => Set<EventType>();
    public DbSet<ModerationReport> ModerationReports => Set<ModerationReport>();
    public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
    public DbSet<SupportTicketMessage> SupportTicketMessages => Set<SupportTicketMessage>();
    public DbSet<SupportTicketAttachment> SupportTicketAttachments => Set<SupportTicketAttachment>();
    public DbSet<SupportTicketHistoryEntry> SupportTicketHistoryEntries => Set<SupportTicketHistoryEntry>();
    public DbSet<SupportTicketAssignmentCursor> SupportTicketAssignmentCursors => Set<SupportTicketAssignmentCursor>();
    public DbSet<SupportTicketStatusLookup> SupportTicketStatuses => Set<SupportTicketStatusLookup>();
    public DbSet<SupportTicketCategoryLookup> SupportTicketCategories => Set<SupportTicketCategoryLookup>();
    public DbSet<SupportTicketRecipientTypeLookup> SupportTicketRecipientTypes => Set<SupportTicketRecipientTypeLookup>();
    public DbSet<EventParticipantSmsRequest> EventParticipantSmsRequests => Set<EventParticipantSmsRequest>();
    public DbSet<SmsQueueItem> SmsQueueItems => Set<SmsQueueItem>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationRecipient> NotificationRecipients => Set<NotificationRecipient>();
    public DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();
    public DbSet<NotificationMessageTypeLookup> NotificationMessageTypes => Set<NotificationMessageTypeLookup>();
    public DbSet<NotificationPriorityLookup> NotificationPriorities => Set<NotificationPriorityLookup>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<PermissionAction> PermissionActions => Set<PermissionAction>();
    public DbSet<RoleOperationPermission> RoleOperationPermissions => Set<RoleOperationPermission>();
    public DbSet<UserOperationPermissionOverride> UserOperationPermissionOverrides => Set<UserOperationPermissionOverride>();
    public DbSet<EventWorkflowLog> EventWorkflowLogs => Set<EventWorkflowLog>();
    public DbSet<EventChangeRequest> EventChangeRequests => Set<EventChangeRequest>();
    public DbSet<EventCancellationRequest> EventCancellationRequests => Set<EventCancellationRequest>();
    public DbSet<EventSettlementRequest> EventSettlementRequests => Set<EventSettlementRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasSequence<int>("EventCodeSequence", "dbo")
            .StartsAt(1200)
            .IncrementsBy(1);

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
            b.Property(log => log.ActorDisplayName).HasMaxLength(200);
            b.Property(log => log.ActorRole).HasMaxLength(80);
            b.Property(log => log.Action).IsRequired().HasMaxLength(120);
            b.Property(log => log.LogType).IsRequired().HasMaxLength(80);
            b.Property(log => log.Module).HasMaxLength(120);
            b.Property(log => log.Description).HasMaxLength(1000);
            b.Property(log => log.TargetType).IsRequired().HasMaxLength(120);
            b.Property(log => log.TargetId).IsRequired().HasMaxLength(120);
            b.Property(log => log.BeforeJson).HasMaxLength(8000);
            b.Property(log => log.AfterJson).HasMaxLength(8000);
            b.Property(log => log.Reason).HasMaxLength(1000);
            b.Property(log => log.IpAddress).HasMaxLength(64);
            b.Property(log => log.RequestPath).HasMaxLength(500);
            b.Property(log => log.UserAgent).HasMaxLength(1000);
            b.Property(log => log.Status).IsRequired().HasMaxLength(40);
            b.Property(log => log.MetadataJson).HasMaxLength(8000);
            b.Property(log => log.CorrelationId).HasMaxLength(100);
            b.HasIndex(log => log.ActorUserId);
            b.HasIndex(log => new { log.LogType, log.CreatedAt });
            b.HasIndex(log => new { log.Module, log.CreatedAt });
            b.HasIndex(log => new { log.Status, log.CreatedAt });
            b.HasIndex(log => new { log.TargetType, log.TargetId });
            b.HasIndex(log => log.CreatedAt);
        });

        modelBuilder.Entity<UserRoleLookup>(b =>
        {
            b.ToTable("UserRoles");
            b.HasKey(role => role.Id);
            b.Property(role => role.Name).IsRequired().HasMaxLength(50);
            b.Property(role => role.DisplayNameFa).IsRequired().HasMaxLength(80);
            b.Property(role => role.IsActive).IsRequired();
            b.Property(role => role.DisplayOrder).IsRequired();
            b.HasIndex(role => role.Name).IsUnique();
            b.HasQueryFilter(role => !role.IsDeleted);
            b.HasData(
                new { Id = 1L, Name = "EndUser", DisplayNameFa = "شرکت‌کننده", IsActive = true, DisplayOrder = 1, CreatedAt = new DateTime(2026, 6, 7, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 2L, Name = "EventPlanner", DisplayNameFa = "برگزارکننده", IsActive = true, DisplayOrder = 2, CreatedAt = new DateTime(2026, 6, 7, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 3L, Name = "Admin", DisplayNameFa = "مدیر", IsActive = true, DisplayOrder = 3, CreatedAt = new DateTime(2026, 6, 7, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 4L, Name = "platform-support-team", DisplayNameFa = "کارشناس پشتیبانی", IsActive = true, DisplayOrder = 4, CreatedAt = new DateTime(2026, 6, 8, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false });
        });

        modelBuilder.Entity<PermissionAction>(b =>
        {
            b.ToTable("PermissionActions");
            b.HasKey(action => action.Id);
            b.Property(action => action.Entity).IsRequired().HasMaxLength(80);
            b.Property(action => action.EntityLabel).IsRequired().HasMaxLength(120).HasDefaultValue("");
            b.Property(action => action.Action).IsRequired().HasMaxLength(80);
            b.Property(action => action.Label).IsRequired().HasMaxLength(120);
            b.Property(action => action.Description).HasMaxLength(500);
            b.Property(action => action.GroupKey).IsRequired().HasMaxLength(80).HasDefaultValue("");
            b.Property(action => action.GroupLabel).IsRequired().HasMaxLength(120).HasDefaultValue("");
            b.Property(action => action.PagePath).HasMaxLength(160);
            b.Property(action => action.HandlerName).HasMaxLength(120);
            b.Property(action => action.UiSurface).IsRequired().HasMaxLength(40).HasDefaultValue("Manual");
            b.Property(action => action.RiskLevel).IsRequired().HasMaxLength(20).HasDefaultValue("Low");
            b.Property(action => action.IsSystemAction).HasDefaultValue(true);
            b.Property(action => action.IsDeprecated).HasDefaultValue(false);
            b.Property(action => action.IsActive).IsRequired();
            b.Property(action => action.DisplayOrder).IsRequired();
            b.HasIndex(action => new { action.Entity, action.Action }).IsUnique();
            b.HasIndex(action => new { action.GroupKey, action.DisplayOrder });
            b.HasIndex(action => action.RiskLevel);
            b.HasQueryFilter(action => !action.IsDeleted);
        });

        modelBuilder.Entity<RoleOperationPermission>(b =>
        {
            b.ToTable("RoleOperationPermissions");
            b.HasKey(permission => permission.Id);
            b.Property(permission => permission.Entity).IsRequired().HasMaxLength(80);
            b.Property(permission => permission.Action).IsRequired().HasMaxLength(80);
            b.Property(permission => permission.Role).IsRequired();
            b.Property(permission => permission.Allowed).IsRequired();
            b.HasIndex(permission => new { permission.Role, permission.Entity, permission.Action }).IsUnique();
            b.HasIndex(permission => new { permission.Entity, permission.Action });
            b.HasQueryFilter(permission => !permission.IsDeleted);
        });

        modelBuilder.Entity<UserOperationPermissionOverride>(b =>
        {
            b.ToTable("UserOperationPermissionOverrides");
            b.HasKey(permission => permission.Id);
            b.Property(permission => permission.Entity).IsRequired().HasMaxLength(80);
            b.Property(permission => permission.Action).IsRequired().HasMaxLength(80);
            b.Property(permission => permission.Allowed).IsRequired();
            b.Property(permission => permission.Note).HasMaxLength(500);
            b.Property(permission => permission.ExpiresAtUtc);
            b.HasIndex(permission => new { permission.UserId, permission.Entity, permission.Action }).IsUnique();
            b.HasIndex(permission => new { permission.Entity, permission.Action });
            b.HasIndex(permission => permission.ExpiresAtUtc);
            b.HasQueryFilter(permission => !permission.IsDeleted);
            b.HasOne(permission => permission.User)
                .WithMany()
                .HasForeignKey(permission => permission.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EventReviewStatusLookup>(b =>
        {
            b.ToTable("ReviewStatuses");
            b.HasKey(status => status.Id);
            b.Property(status => status.Name).IsRequired().HasMaxLength(50);
            b.Property(status => status.DisplayNameFa).IsRequired().HasMaxLength(80);
            b.Property(status => status.IsActive).IsRequired();
            b.Property(status => status.DisplayOrder).IsRequired();
            b.HasIndex(status => status.Name).IsUnique();
            b.HasQueryFilter(status => !status.IsDeleted);
            b.HasData(
                new { Id = 1L, Name = "NotSubmitted", DisplayNameFa = "ارسال نشده", IsActive = true, DisplayOrder = 1, CreatedAt = new DateTime(2026, 6, 7, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 2L, Name = "PendingReview", DisplayNameFa = "در انتظار بررسی", IsActive = true, DisplayOrder = 2, CreatedAt = new DateTime(2026, 6, 7, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 3L, Name = "Approved", DisplayNameFa = "تایید شده توسط مدیر", IsActive = true, DisplayOrder = 3, CreatedAt = new DateTime(2026, 6, 7, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 4L, Name = "Rejected", DisplayNameFa = "رد شده توسط مدیر", IsActive = true, DisplayOrder = 4, CreatedAt = new DateTime(2026, 6, 7, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false });
        });

        modelBuilder.Entity<EventApprovalStatusLookup>(b =>
        {
            b.ToTable("EventApprovalStatuses");
            b.HasKey(status => status.Id);
            b.Property(status => status.Name).IsRequired().HasMaxLength(50);
            b.Property(status => status.DisplayNameFa).IsRequired().HasMaxLength(80);
            b.Property(status => status.IsActive).IsRequired();
            b.Property(status => status.DisplayOrder).IsRequired();
            b.HasIndex(status => status.Name).IsUnique();
            b.HasQueryFilter(status => !status.IsDeleted);
            b.HasData(
                new { Id = 1L, Name = "Draft", DisplayNameFa = "پیش‌نویس", IsActive = true, DisplayOrder = 1, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 2L, Name = "PendingReview", DisplayNameFa = "در انتظار بررسی مدیر", IsActive = true, DisplayOrder = 2, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 3L, Name = "Approved", DisplayNameFa = "تایید شده", IsActive = true, DisplayOrder = 3, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false });
        });

        modelBuilder.Entity<EventSaleStatusLookup>(b =>
        {
            b.ToTable("EventSaleStatuses");
            b.HasKey(status => status.Id);
            b.Property(status => status.Name).IsRequired().HasMaxLength(50);
            b.Property(status => status.DisplayNameFa).IsRequired().HasMaxLength(80);
            b.Property(status => status.IsActive).IsRequired();
            b.Property(status => status.DisplayOrder).IsRequired();
            b.HasIndex(status => status.Name).IsUnique();
            b.HasQueryFilter(status => !status.IsDeleted);
            b.HasData(
                new { Id = 1L, Name = "Closed", DisplayNameFa = "فروش بسته", IsActive = true, DisplayOrder = 1, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 2L, Name = "Open", DisplayNameFa = "در حال فروش", IsActive = true, DisplayOrder = 2, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false });
        });

        modelBuilder.Entity<EventLifecycleStatusLookup>(b =>
        {
            b.ToTable("EventLifecycleStatuses");
            b.HasKey(status => status.Id);
            b.Property(status => status.Name).IsRequired().HasMaxLength(50);
            b.Property(status => status.DisplayNameFa).IsRequired().HasMaxLength(80);
            b.Property(status => status.IsActive).IsRequired();
            b.Property(status => status.DisplayOrder).IsRequired();
            b.HasIndex(status => status.Name).IsUnique();
            b.HasQueryFilter(status => !status.IsDeleted);
            b.HasData(
                new { Id = 1L, Name = "Active", DisplayNameFa = "فعال", IsActive = true, DisplayOrder = 1, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 2L, Name = "Cancelled", DisplayNameFa = "لغو شده", IsActive = true, DisplayOrder = 2, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 3L, Name = "Completed", DisplayNameFa = "تمام شده", IsActive = true, DisplayOrder = 3, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false });
        });

        modelBuilder.Entity<EventRequestStatusLookup>(b =>
        {
            b.ToTable("EventRequestStatuses");
            b.HasKey(status => status.Id);
            b.Property(status => status.Name).IsRequired().HasMaxLength(50);
            b.Property(status => status.DisplayNameFa).IsRequired().HasMaxLength(80);
            b.Property(status => status.IsActive).IsRequired();
            b.Property(status => status.DisplayOrder).IsRequired();
            b.HasIndex(status => status.Name).IsUnique();
            b.HasQueryFilter(status => !status.IsDeleted);
            b.HasData(
                new { Id = 1L, Name = "Pending", DisplayNameFa = "در انتظار بررسی", IsActive = true, DisplayOrder = 1, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 2L, Name = "Approved", DisplayNameFa = "تایید شده", IsActive = true, DisplayOrder = 2, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 3L, Name = "Rejected", DisplayNameFa = "رد شده", IsActive = true, DisplayOrder = 3, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 4L, Name = "Cancelled", DisplayNameFa = "لغو شده", IsActive = true, DisplayOrder = 4, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false });
        });

        modelBuilder.Entity<EventWorkflowActionTypeLookup>(b =>
        {
            b.ToTable("EventWorkflowActionTypes");
            b.HasKey(type => type.Id);
            b.Property(type => type.Name).IsRequired().HasMaxLength(80);
            b.Property(type => type.DisplayNameFa).IsRequired().HasMaxLength(120);
            b.Property(type => type.IsActive).IsRequired();
            b.Property(type => type.DisplayOrder).IsRequired();
            b.HasIndex(type => type.Name).IsUnique();
            b.HasQueryFilter(type => !type.IsDeleted);
            b.HasData(
                new { Id = 1L, Name = "DraftSaved", DisplayNameFa = "ذخیره پیش‌نویس", IsActive = true, DisplayOrder = 1, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 2L, Name = "SubmittedForReview", DisplayNameFa = "ارسال برای بررسی", IsActive = true, DisplayOrder = 2, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 3L, Name = "Approved", DisplayNameFa = "تایید رویداد", IsActive = true, DisplayOrder = 3, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 4L, Name = "ReturnedToDraft", DisplayNameFa = "بازگشت برای اصلاح", IsActive = true, DisplayOrder = 4, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 5L, Name = "SaleOpened", DisplayNameFa = "باز شدن فروش", IsActive = true, DisplayOrder = 5, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 6L, Name = "SaleClosed", DisplayNameFa = "بسته شدن فروش", IsActive = true, DisplayOrder = 6, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 7L, Name = "ChangeRequested", DisplayNameFa = "درخواست تغییر", IsActive = true, DisplayOrder = 7, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 8L, Name = "ChangeApproved", DisplayNameFa = "تایید تغییر", IsActive = true, DisplayOrder = 8, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 9L, Name = "ChangeRejected", DisplayNameFa = "رد تغییر", IsActive = true, DisplayOrder = 9, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 10L, Name = "CancellationRequested", DisplayNameFa = "درخواست لغو", IsActive = true, DisplayOrder = 10, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 11L, Name = "Cancelled", DisplayNameFa = "لغو رویداد", IsActive = true, DisplayOrder = 11, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 12L, Name = "Completed", DisplayNameFa = "اتمام رویداد", IsActive = true, DisplayOrder = 12, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 13L, Name = "SettlementRequested", DisplayNameFa = "درخواست تسویه رویداد", IsActive = true, DisplayOrder = 13, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 14L, Name = "SettlementApproved", DisplayNameFa = "تایید تسویه رویداد", IsActive = true, DisplayOrder = 14, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 15L, Name = "SettlementRejected", DisplayNameFa = "رد تسویه رویداد", IsActive = true, DisplayOrder = 15, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 16L, Name = "OrganizerCredited", DisplayNameFa = "بستانکاری برگزارکننده", IsActive = true, DisplayOrder = 16, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 17L, Name = "WithdrawalRequested", DisplayNameFa = "درخواست برداشت", IsActive = true, DisplayOrder = 17, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false });
        });

        modelBuilder.Entity<EventDiscountTypeLookup>(b =>
        {
            b.ToTable("DiscountTypes");
            b.HasKey(type => type.Id);
            b.Property(type => type.Name).IsRequired().HasMaxLength(50);
            b.Property(type => type.DisplayNameFa).IsRequired().HasMaxLength(80);
            b.Property(type => type.IsActive).IsRequired();
            b.Property(type => type.DisplayOrder).IsRequired();
            b.HasIndex(type => type.Name).IsUnique();
            b.HasQueryFilter(type => !type.IsDeleted);
            b.HasData(
                new { Id = 1L, Name = "FixedAmount", DisplayNameFa = "مبلغ ثابت", IsActive = true, DisplayOrder = 1, CreatedAt = new DateTime(2026, 6, 7, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 2L, Name = "Percentage", DisplayNameFa = "درصدی", IsActive = true, DisplayOrder = 2, CreatedAt = new DateTime(2026, 6, 7, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false });
        });

        modelBuilder.Entity<BalanceTransactionTypeLookup>(b =>
        {
            b.ToTable("BalanceTransactionTypes");
            b.HasKey(type => type.Id);
            b.Property(type => type.Name).IsRequired().HasMaxLength(80);
            b.Property(type => type.DisplayNameFa).IsRequired().HasMaxLength(120);
            b.Property(type => type.IsActive).IsRequired();
            b.Property(type => type.DisplayOrder).IsRequired();
            b.HasIndex(type => type.Name).IsUnique();
            b.HasQueryFilter(type => !type.IsDeleted);
            b.HasData(
                new { Id = 1L, Name = "AdminAdjustment", DisplayNameFa = "اصلاح مدیر", IsActive = true, DisplayOrder = 1, CreatedAt = new DateTime(2026, 6, 7, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 2L, Name = "TicketPurchase", DisplayNameFa = "خرید بلیت", IsActive = true, DisplayOrder = 2, CreatedAt = new DateTime(2026, 6, 7, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 3L, Name = "TicketRefund", DisplayNameFa = "بازگشت بلیت", IsActive = true, DisplayOrder = 3, CreatedAt = new DateTime(2026, 6, 7, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 4L, Name = "EventPlannerIncome", DisplayNameFa = "درآمد برگزارکننده", IsActive = true, DisplayOrder = 4, CreatedAt = new DateTime(2026, 6, 7, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 5L, Name = "PlatformCommission", DisplayNameFa = "کمیسیون پلتفرم", IsActive = true, DisplayOrder = 5, CreatedAt = new DateTime(2026, 6, 7, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 6L, Name = "EmergencyRemovalRefund", DisplayNameFa = "بازگشت حذف اضطراری", IsActive = true, DisplayOrder = 6, CreatedAt = new DateTime(2026, 6, 7, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 7L, Name = "PlannerWithdrawalPayout", DisplayNameFa = "تسویه برگزارکننده", IsActive = true, DisplayOrder = 7, CreatedAt = new DateTime(2026, 6, 7, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 8L, Name = "EventPlannerIncomeReversal", DisplayNameFa = "برگشت درآمد برگزارکننده", IsActive = true, DisplayOrder = 8, CreatedAt = new DateTime(2026, 6, 7, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 9L, Name = "EventSettlementCredit", DisplayNameFa = "بستانکاری تسویه رویداد", IsActive = true, DisplayOrder = 9, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 10L, Name = "EventSettlementReversal", DisplayNameFa = "برگشت بستانکاری رویداد", IsActive = true, DisplayOrder = 10, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 11L, Name = "PlatformCommissionRecognized", DisplayNameFa = "شناسایی کمیسیون پلتفرم", IsActive = true, DisplayOrder = 11, CreatedAt = new DateTime(2026, 6, 11, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 12L, Name = "ManualReceiptWalletCredit", DisplayNameFa = "اعتبار کیف پول بابت رسید دستی", IsActive = true, DisplayOrder = 12, CreatedAt = new DateTime(2026, 6, 14, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 13L, Name = "OrganizerManualReceiptLiability", DisplayNameFa = "بدهی برگزارکننده بابت رسید دستی", IsActive = true, DisplayOrder = 13, CreatedAt = new DateTime(2026, 6, 14, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false });
        });

        modelBuilder.Entity<CurrencyLookup>(b =>
        {
            b.ToTable("Currencies");
            b.HasKey(currency => currency.Id);
            b.Property(currency => currency.Code).IsRequired().HasMaxLength(3);
            b.Property(currency => currency.DisplayNameFa).IsRequired().HasMaxLength(80);
            b.Property(currency => currency.Symbol).IsRequired().HasMaxLength(12);
            b.Property(currency => currency.DecimalPlaces).IsRequired().HasDefaultValue(2);
            b.Property(currency => currency.IsActive).IsRequired();
            b.Property(currency => currency.DisplayOrder).IsRequired();
            b.HasIndex(currency => currency.Code).IsUnique();
            b.HasQueryFilter(currency => !currency.IsDeleted);
            b.HasData(
                new { Id = 1L, Code = "IRR", DisplayNameFa = "ریال ایران", Symbol = "ریال", DecimalPlaces = 0, IsActive = true, DisplayOrder = 1, CreatedAt = new DateTime(2026, 6, 8, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 2L, Code = "EUR", DisplayNameFa = "یورو", Symbol = "€", DecimalPlaces = 2, IsActive = true, DisplayOrder = 2, CreatedAt = new DateTime(2026, 6, 8, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 3L, Code = "USD", DisplayNameFa = "دلار آمریکا", Symbol = "$", DecimalPlaces = 2, IsActive = true, DisplayOrder = 3, CreatedAt = new DateTime(2026, 6, 8, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 4L, Code = "CAD", DisplayNameFa = "دلار کانادا", Symbol = "C$", DecimalPlaces = 2, IsActive = true, DisplayOrder = 4, CreatedAt = new DateTime(2026, 6, 8, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 5L, Code = "GBP", DisplayNameFa = "پوند انگلیس", Symbol = "£", DecimalPlaces = 2, IsActive = true, DisplayOrder = 5, CreatedAt = new DateTime(2026, 6, 8, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 6L, Code = "AED", DisplayNameFa = "درهم امارات", Symbol = "AED", DecimalPlaces = 2, IsActive = true, DisplayOrder = 6, CreatedAt = new DateTime(2026, 6, 8, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 7L, Code = "TRY", DisplayNameFa = "لیر ترکیه", Symbol = "₺", DecimalPlaces = 2, IsActive = true, DisplayOrder = 7, CreatedAt = new DateTime(2026, 6, 8, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false });
        });

        modelBuilder.Entity<CurrencyExchangeRate>(b =>
        {
            b.ToTable("CurrencyExchangeRates");
            b.HasKey(rate => rate.Id);
            b.Property(rate => rate.FromCurrencyCode).IsRequired().HasMaxLength(3);
            b.Property(rate => rate.ToCurrencyCode).IsRequired().HasMaxLength(3);
            b.Property(rate => rate.Rate).HasPrecision(18, 6).IsRequired();
            b.Property(rate => rate.EffectiveFromUtc).IsRequired();
            b.Property(rate => rate.EffectiveToUtc);
            b.Property(rate => rate.Source).IsRequired().HasMaxLength(80);
            b.HasIndex(rate => new { rate.FromCurrencyCode, rate.ToCurrencyCode, rate.EffectiveFromUtc }).IsUnique();
            b.HasIndex(rate => new { rate.FromCurrencyCode, rate.ToCurrencyCode, rate.EffectiveToUtc });
            b.HasQueryFilter(rate => !rate.IsDeleted);
            b.HasData(
                new { Id = 1L, FromCurrencyCode = "IRR", ToCurrencyCode = "IRR", Rate = 1m, EffectiveFromUtc = new DateTime(2026, 6, 8, 0, 0, 0, DateTimeKind.Utc), EffectiveToUtc = (DateTime?)null, Source = "Seed", CreatedByUserId = (long?)null, CreatedAt = new DateTime(2026, 6, 8, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 2L, FromCurrencyCode = "USD", ToCurrencyCode = "IRR", Rate = 1750000m, EffectiveFromUtc = new DateTime(2026, 6, 8, 0, 0, 0, DateTimeKind.Utc), EffectiveToUtc = (DateTime?)null, Source = "Seed", CreatedByUserId = (long?)null, CreatedAt = new DateTime(2026, 6, 8, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 3L, FromCurrencyCode = "EUR", ToCurrencyCode = "IRR", Rate = 2000000m, EffectiveFromUtc = new DateTime(2026, 6, 8, 0, 0, 0, DateTimeKind.Utc), EffectiveToUtc = (DateTime?)null, Source = "Seed", CreatedByUserId = (long?)null, CreatedAt = new DateTime(2026, 6, 8, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 4L, FromCurrencyCode = "CAD", ToCurrencyCode = "IRR", Rate = 1280000m, EffectiveFromUtc = new DateTime(2026, 6, 8, 0, 0, 0, DateTimeKind.Utc), EffectiveToUtc = (DateTime?)null, Source = "Seed", CreatedByUserId = (long?)null, CreatedAt = new DateTime(2026, 6, 8, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 5L, FromCurrencyCode = "GBP", ToCurrencyCode = "IRR", Rate = 2350000m, EffectiveFromUtc = new DateTime(2026, 6, 8, 0, 0, 0, DateTimeKind.Utc), EffectiveToUtc = (DateTime?)null, Source = "Seed", CreatedByUserId = (long?)null, CreatedAt = new DateTime(2026, 6, 8, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 6L, FromCurrencyCode = "AED", ToCurrencyCode = "IRR", Rate = 476500m, EffectiveFromUtc = new DateTime(2026, 6, 8, 0, 0, 0, DateTimeKind.Utc), EffectiveToUtc = (DateTime?)null, Source = "Seed", CreatedByUserId = (long?)null, CreatedAt = new DateTime(2026, 6, 8, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 7L, FromCurrencyCode = "TRY", ToCurrencyCode = "IRR", Rate = 54000m, EffectiveFromUtc = new DateTime(2026, 6, 8, 0, 0, 0, DateTimeKind.Utc), EffectiveToUtc = (DateTime?)null, Source = "Seed", CreatedByUserId = (long?)null, CreatedAt = new DateTime(2026, 6, 8, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false });
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

        modelBuilder.Entity<ZodiacSignLookup>(b =>
        {
            b.ToTable("ZodiacSigns");
            b.HasKey(sign => sign.Id);
            b.Property(sign => sign.Code).IsRequired().HasMaxLength(30);
            b.Property(sign => sign.Title).IsRequired().HasMaxLength(80);
            b.Property(sign => sign.IsActive).IsRequired();
            b.Property(sign => sign.DisplayOrder).IsRequired();
            b.HasIndex(sign => sign.Code).IsUnique();
            b.HasIndex(sign => sign.Title).IsUnique();
            b.HasQueryFilter(sign => !sign.IsDeleted);
            b.HasData(
                new { Id = 1L, Code = "Aries", Title = "حمل", IsActive = true, DisplayOrder = 1, CreatedAt = new DateTime(2026, 6, 6, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 2L, Code = "Taurus", Title = "ثور", IsActive = true, DisplayOrder = 2, CreatedAt = new DateTime(2026, 6, 6, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 3L, Code = "Gemini", Title = "جوزا", IsActive = true, DisplayOrder = 3, CreatedAt = new DateTime(2026, 6, 6, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 4L, Code = "Cancer", Title = "سرطان", IsActive = true, DisplayOrder = 4, CreatedAt = new DateTime(2026, 6, 6, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 5L, Code = "Leo", Title = "اسد", IsActive = true, DisplayOrder = 5, CreatedAt = new DateTime(2026, 6, 6, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 6L, Code = "Virgo", Title = "سنبله", IsActive = true, DisplayOrder = 6, CreatedAt = new DateTime(2026, 6, 6, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 7L, Code = "Libra", Title = "میزان", IsActive = true, DisplayOrder = 7, CreatedAt = new DateTime(2026, 6, 6, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 8L, Code = "Scorpio", Title = "عقرب", IsActive = true, DisplayOrder = 8, CreatedAt = new DateTime(2026, 6, 6, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 9L, Code = "Sagittarius", Title = "قوس", IsActive = true, DisplayOrder = 9, CreatedAt = new DateTime(2026, 6, 6, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 10L, Code = "Capricorn", Title = "جدی", IsActive = true, DisplayOrder = 10, CreatedAt = new DateTime(2026, 6, 6, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 11L, Code = "Aquarius", Title = "دلو", IsActive = true, DisplayOrder = 11, CreatedAt = new DateTime(2026, 6, 6, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 12L, Code = "Pisces", Title = "حوت", IsActive = true, DisplayOrder = 12, CreatedAt = new DateTime(2026, 6, 6, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false });
        });

        modelBuilder.Entity<EventPlannerProfile>(b =>
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.Title).IsRequired().HasMaxLength(100);
            b.Property(p => p.PictureUrl).HasMaxLength(500);
            b.Property(p => p.Resume).IsRequired().HasMaxLength(4000);
            b.Property(p => p.SettlementCurrencyCode).IsRequired().HasMaxLength(3).HasDefaultValue("IRR");
            b.Property(p => p.SettlementCurrencyLockReason).HasMaxLength(300);
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

        modelBuilder.Entity<EventModeLookup>(b =>
        {
            b.ToTable("EventModes");
            b.HasKey(mode => mode.Id);
            b.Property(mode => mode.Name).IsRequired().HasMaxLength(80);
            b.Property(mode => mode.IsOnline).IsRequired();
            b.Property(mode => mode.IsActive).IsRequired();
            b.Property(mode => mode.DisplayOrder).IsRequired();
            b.HasIndex(mode => mode.Name).IsUnique();
            b.HasQueryFilter(mode => !mode.IsDeleted);
            b.HasData(
                new { Id = 1L, Name = "آنلاین", IsOnline = true, IsActive = true, DisplayOrder = 1, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 2L, Name = "حضوری", IsOnline = false, IsActive = true, DisplayOrder = 2, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false });
        });

        modelBuilder.Entity<OnlineEventPlatform>(b =>
        {
            b.ToTable("OnlineEventPlatforms");
            b.HasKey(platform => platform.Id);
            b.Property(platform => platform.Name).IsRequired().HasMaxLength(80);
            b.Property(platform => platform.IsActive).IsRequired();
            b.Property(platform => platform.DisplayOrder).IsRequired();
            b.HasIndex(platform => platform.Name).IsUnique();
            b.HasQueryFilter(platform => !platform.IsDeleted);
            b.HasData(
                new { Id = 1L, Name = "Google Meet", IsActive = true, DisplayOrder = 1, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 2L, Name = "Zoom", IsActive = true, DisplayOrder = 2, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 3L, Name = "اسکای روم", IsActive = true, DisplayOrder = 3, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 4L, Name = "ادوبی کانکت", IsActive = true, DisplayOrder = 4, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 5L, Name = "سایر", IsActive = true, DisplayOrder = 5, CreatedAt = new DateTime(2026, 6, 4, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false });
        });

        modelBuilder.Entity<BalanceAccount>(b =>
        {
            b.HasKey(a => a.Id);
            b.Property(a => a.Balance).HasPrecision(18, 2).IsRequired();
            b.Property(a => a.ReportingCurrencyCode).IsRequired().HasMaxLength(3).HasDefaultValue("IRR");
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
            b.Property(t => t.CurrencyCode).IsRequired().HasMaxLength(3).HasDefaultValue("IRR");
            b.Property(t => t.ReportingCurrencyCode).IsRequired().HasMaxLength(3).HasDefaultValue("IRR");
            b.Property(t => t.ReportingAmountIrr).HasPrecision(18, 2).IsRequired();
            b.Property(t => t.ExchangeRateToIrr).HasPrecision(18, 6).IsRequired().HasDefaultValue(1m);
            b.Property(t => t.ExchangeRateCapturedAtUtc).HasDefaultValueSql("GETUTCDATE()").IsRequired();
            b.Property(t => t.Description).IsRequired().HasMaxLength(300);
            b.Property(t => t.ReferenceType).HasMaxLength(100);
            b.HasIndex(t => t.UserId);
            b.HasIndex(t => t.CurrencyCode);
            b.HasIndex(t => t.ExchangeRateId);
            b.HasIndex(t => t.TicketOrderId);
            b.HasQueryFilter(t => !t.BalanceAccount.IsDeleted);
            b.HasOne(t => t.ExchangeRate)
                .WithMany()
                .HasForeignKey(t => t.ExchangeRateId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(t => t.TicketOrder)
                .WithMany()
                .HasForeignKey(t => t.TicketOrderId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OnlinePayment>(b =>
        {
            b.HasKey(payment => payment.Id);
            b.Property(payment => payment.Amount).HasPrecision(18, 2).IsRequired();
            b.Property(payment => payment.CurrencyCode).IsRequired().HasMaxLength(3).HasDefaultValue("IRR");
            b.Property(payment => payment.ReportingAmountIrr).HasPrecision(18, 2).IsRequired();
            b.Property(payment => payment.ExchangeRateToIrr).HasPrecision(18, 6).IsRequired().HasDefaultValue(1m);
            b.Property(payment => payment.ExchangeRateCapturedAtUtc).HasDefaultValueSql("GETUTCDATE()").IsRequired();
            b.Property(payment => payment.GatewayName).IsRequired().HasMaxLength(80);
            b.Property(payment => payment.TrackingCode).IsRequired().HasMaxLength(120);
            b.Property(payment => payment.Status).IsRequired();
            b.Property(payment => payment.FailureReason).HasMaxLength(500);
            b.HasIndex(payment => payment.UserId);
            b.HasIndex(payment => payment.DatingEventId);
            b.HasIndex(payment => payment.EventTicketId);
            b.HasIndex(payment => payment.TicketOrderId);
            b.HasIndex(payment => payment.BalanceTransactionId);
            b.HasIndex(payment => payment.CurrencyCode);
            b.HasIndex(payment => payment.ExchangeRateId);
            b.HasIndex(payment => payment.TrackingCode).IsUnique();
            b.HasQueryFilter(payment => !payment.IsDeleted && !payment.User.IsDeleted);
            b.HasOne(payment => payment.User)
                .WithMany()
                .HasForeignKey(payment => payment.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(payment => payment.DatingEvent)
                .WithMany()
                .HasForeignKey(payment => payment.DatingEventId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(payment => payment.EventTicket)
                .WithMany()
                .HasForeignKey(payment => payment.EventTicketId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(payment => payment.TicketOrder)
                .WithMany()
                .HasForeignKey(payment => payment.TicketOrderId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(payment => payment.BalanceTransaction)
                .WithMany()
                .HasForeignKey(payment => payment.BalanceTransactionId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(payment => payment.ExchangeRate)
                .WithMany()
                .HasForeignKey(payment => payment.ExchangeRateId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ManualPaymentReceipt>(b =>
        {
            b.ToTable("ManualPaymentReceipts");
            b.HasKey(receipt => receipt.Id);
            b.Property(receipt => receipt.PaymentCollectionMethod).IsRequired();
            b.Property(receipt => receipt.DestinationType).IsRequired();
            b.Property(receipt => receipt.OriginalAmount).HasPrecision(18, 2).IsRequired();
            b.Property(receipt => receipt.DiscountAmount).HasPrecision(18, 2).IsRequired();
            b.Property(receipt => receipt.Amount).HasPrecision(18, 2).IsRequired();
            b.Property(receipt => receipt.CurrencyCode).IsRequired().HasMaxLength(3).HasDefaultValue("IRR");
            b.Property(receipt => receipt.ReportingCurrencyCode).IsRequired().HasMaxLength(3).HasDefaultValue("IRR");
            b.Property(receipt => receipt.ReportingAmountIrr).HasPrecision(18, 2).IsRequired();
            b.Property(receipt => receipt.ExchangeRateToIrr).HasPrecision(18, 6).IsRequired().HasDefaultValue(1m);
            b.Property(receipt => receipt.ExchangeRateCapturedAtUtc).HasDefaultValueSql("GETUTCDATE()").IsRequired();
            b.Property(receipt => receipt.UploadedFilePath).IsRequired().HasMaxLength(1000);
            b.Property(receipt => receipt.TrackingNumber).HasMaxLength(120);
            b.Property(receipt => receipt.PayerNote).HasMaxLength(1000);
            b.Property(receipt => receipt.Status).IsRequired();
            b.Property(receipt => receipt.SubmittedAtUtc).HasDefaultValueSql("GETUTCDATE()").IsRequired();
            b.Property(receipt => receipt.RejectReason).HasMaxLength(1000);
            b.Property(receipt => receipt.DiscountCode).HasMaxLength(50);
            b.HasIndex(receipt => new { receipt.DestinationType, receipt.Status, receipt.SubmittedAtUtc });
            b.HasIndex(receipt => receipt.DatingEventId);
            b.HasIndex(receipt => receipt.ParticipantUserId);
            b.HasIndex(receipt => receipt.PlannerUserId);
            b.HasIndex(receipt => receipt.EventTicketId);
            b.HasIndex(receipt => receipt.TicketOrderId);
            b.HasIndex(receipt => receipt.WalletCreditTransactionId);
            b.HasIndex(receipt => receipt.EventDiscountCodeId);
            b.HasIndex(receipt => receipt.CurrencyCode);
            b.HasIndex(receipt => receipt.ExchangeRateId);
            b.HasQueryFilter(receipt => !receipt.IsDeleted
                && !receipt.DatingEvent.IsDeleted
                && !receipt.ParticipantUser.IsDeleted
                && !receipt.PlannerUser.IsDeleted);
            b.HasOne(receipt => receipt.DatingEvent)
                .WithMany()
                .HasForeignKey(receipt => receipt.DatingEventId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(receipt => receipt.ParticipantUser)
                .WithMany()
                .HasForeignKey(receipt => receipt.ParticipantUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(receipt => receipt.PlannerUser)
                .WithMany()
                .HasForeignKey(receipt => receipt.PlannerUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(receipt => receipt.EventTicket)
                .WithMany()
                .HasForeignKey(receipt => receipt.EventTicketId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(receipt => receipt.TicketOrder)
                .WithMany()
                .HasForeignKey(receipt => receipt.TicketOrderId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(receipt => receipt.WalletCreditTransaction)
                .WithMany()
                .HasForeignKey(receipt => receipt.WalletCreditTransactionId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(receipt => receipt.EventDiscountCode)
                .WithMany()
                .HasForeignKey(receipt => receipt.EventDiscountCodeId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(receipt => receipt.ReviewedByUser)
                .WithMany()
                .HasForeignKey(receipt => receipt.ReviewedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(receipt => receipt.ExchangeRate)
                .WithMany()
                .HasForeignKey(receipt => receipt.ExchangeRateId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TicketRefundRequest>(b =>
        {
            b.ToTable("TicketRefundRequests");
            b.HasKey(request => request.Id);
            b.Property(request => request.Status).IsRequired();
            b.Property(request => request.RequestedAmount).HasPrecision(18, 2).IsRequired();
            b.Property(request => request.ApprovedAmount).HasPrecision(18, 2).IsRequired();
            b.Property(request => request.CurrencyCode).IsRequired().HasMaxLength(3).HasDefaultValue("IRR");
            b.Property(request => request.ReportingRequestedAmountIrr).HasPrecision(18, 2).IsRequired();
            b.Property(request => request.ReportingApprovedAmountIrr).HasPrecision(18, 2).IsRequired();
            b.Property(request => request.ExchangeRateToIrr).HasPrecision(18, 6).IsRequired().HasDefaultValue(1m);
            b.Property(request => request.ExchangeRateCapturedAtUtc).HasDefaultValueSql("GETUTCDATE()").IsRequired();
            b.Property(request => request.RequestReason).IsRequired().HasMaxLength(1000);
            b.Property(request => request.RequestedAtUtc).HasDefaultValueSql("GETUTCDATE()").IsRequired();
            b.Property(request => request.ReviewNote).HasMaxLength(1000);
            b.HasIndex(request => new { request.Status, request.RequestedAtUtc });
            b.HasIndex(request => request.EventTicketId);
            b.HasIndex(request => request.TicketOrderId);
            b.HasIndex(request => request.DatingEventId);
            b.HasIndex(request => request.BuyerUserId);
            b.HasIndex(request => request.ParticipantUserId);
            b.HasIndex(request => request.RequestedByUserId);
            b.HasIndex(request => request.ReviewedByUserId);
            b.HasIndex(request => request.WalletCreditTransactionId);
            b.HasIndex(request => request.ExchangeRateId);
            b.HasQueryFilter(request => !request.IsDeleted && !request.DatingEvent.IsDeleted);
            b.HasOne(request => request.EventTicket)
                .WithMany()
                .HasForeignKey(request => request.EventTicketId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(request => request.TicketOrder)
                .WithMany()
                .HasForeignKey(request => request.TicketOrderId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(request => request.DatingEvent)
                .WithMany()
                .HasForeignKey(request => request.DatingEventId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(request => request.BuyerUser)
                .WithMany()
                .HasForeignKey(request => request.BuyerUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(request => request.ParticipantUser)
                .WithMany()
                .HasForeignKey(request => request.ParticipantUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(request => request.RequestedByUser)
                .WithMany()
                .HasForeignKey(request => request.RequestedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(request => request.ReviewedByUser)
                .WithMany()
                .HasForeignKey(request => request.ReviewedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(request => request.WalletCreditTransaction)
                .WithMany()
                .HasForeignKey(request => request.WalletCreditTransactionId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(request => request.ExchangeRate)
                .WithMany()
                .HasForeignKey(request => request.ExchangeRateId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PlannerWithdrawalRequest>(b =>
        {
            b.HasKey(request => request.Id);
            b.Property(request => request.Amount).HasPrecision(18, 2).IsRequired();
            b.Property(request => request.CurrencyCode).IsRequired().HasMaxLength(3).HasDefaultValue("IRR");
            b.Property(request => request.ReportingAmountIrr).HasPrecision(18, 2).IsRequired();
            b.Property(request => request.ExchangeRateToIrr).HasPrecision(18, 6).IsRequired().HasDefaultValue(1m);
            b.Property(request => request.ExchangeRateCapturedAtUtc).HasDefaultValueSql("GETUTCDATE()").IsRequired();
            b.Property(request => request.Status).IsRequired();
            b.HasIndex(request => request.CurrencyCode);
            b.HasIndex(request => request.ExchangeRateId);
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
            b.HasOne(request => request.ExchangeRate)
                .WithMany()
                .HasForeignKey(request => request.ExchangeRateId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PlannerBankAccount>(b =>
        {
            b.HasKey(account => account.Id);
            b.Property(account => account.CurrencyCode).IsRequired().HasMaxLength(3).HasDefaultValue("IRR");
            b.Property(account => account.PayoutMethod).IsRequired().HasDefaultValue(PlannerPayoutMethod.IranianBankCard);
            b.Property(account => account.AccountHolderName).IsRequired().HasMaxLength(120).HasDefaultValue("برگزارکننده");
            b.Property(account => account.Country).HasMaxLength(80);
            b.Property(account => account.CardNumber).HasMaxLength(19);
            b.Property(account => account.Iban).HasMaxLength(34);
            b.Property(account => account.BankName).HasMaxLength(80);
            b.Property(account => account.AccountNumber).HasMaxLength(80);
            b.Property(account => account.SwiftCode).HasMaxLength(20);
            b.Property(account => account.AccountIdentifier).HasMaxLength(160);
            b.Property(account => account.PublicPaymentInstructions).HasMaxLength(1200);
            b.Property(account => account.IsActive).IsRequired();
            b.HasIndex(account => account.UserId);
            b.HasIndex(account => account.CurrencyCode);
            b.HasIndex(account => account.Iban).IsUnique().HasFilter("[Iban] IS NOT NULL");
            b.HasQueryFilter(account => !account.IsDeleted && !account.User.IsDeleted);
            b.HasOne(account => account.User)
                .WithMany()
                .HasForeignKey(account => account.UserId)
                .OnDelete(DeleteBehavior.Cascade);
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
            b.HasIndex(p => p.ZodiacSignId);
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
            b.HasOne(p => p.ZodiacSignLookup)
                .WithMany()
                .HasForeignKey(p => p.ZodiacSignId)
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
            b.Property(e => e.EventCode)
                .IsRequired()
                .HasDefaultValueSql("NEXT VALUE FOR dbo.EventCodeSequence");
            b.Property(e => e.Title).IsRequired().HasMaxLength(150);
            b.Property(e => e.Address).IsRequired().HasMaxLength(300);
            b.HasIndex(e => e.EventCode).IsUnique();
            b.HasIndex(e => e.EventTypeId);
            b.Property(e => e.EventModeId).HasDefaultValue(2L).IsRequired();
            b.Property(e => e.OnlineJoinUrl).HasMaxLength(500);
            b.Property(e => e.OnlineAccessInstructions).HasMaxLength(1200);
            b.HasIndex(e => e.EventModeId);
            b.HasIndex(e => e.OnlineEventPlatformId);
            b.HasIndex(e => e.CountryId);
            b.HasIndex(e => e.CityId);
            b.HasIndex(e => e.MinimumEducationLevelId);
            b.HasIndex(e => new { e.IsCancelled, e.DateTimeEnd });
            b.HasIndex(e => new { e.IsCancelled, e.IsOpenForSell, e.DateTimeEnd });
            b.HasIndex(e => new { e.ReviewStatus, e.DateTimeStart });
            b.HasIndex(e => new { e.ApprovalStatus, e.DateTimeStart });
            b.HasIndex(e => new { e.LifecycleStatus, e.SaleStatus, e.DateTimeEnd });
            b.HasIndex(e => new { e.DateTimeStart, e.Id });
            b.HasIndex(e => new { e.UpdatedAt, e.Id });
            b.Property(e => e.EventPlannerCommissionPercent).HasPrecision(5, 2).IsRequired();
            b.Property(e => e.PaymentCollectionMethod).HasDefaultValue(EventPaymentCollectionMethod.PlatformGateway).IsRequired();
            b.Property(e => e.OrganizerPaymentInstructions).HasMaxLength(1200);
            b.HasIndex(e => e.OrganizerPaymentAccountId);
            b.Property(e => e.NumberOfLikesAllowed).HasColumnName("NumberOfLikesAllowed").IsRequired();
            b.Property(e => e.ReviewStatus).IsRequired();
            b.Property(e => e.ApprovalStatus).HasDefaultValue(EventApprovalStatus.Draft).IsRequired();
            b.Property(e => e.SaleStatus).HasDefaultValue(EventSaleStatus.Closed).IsRequired();
            b.Property(e => e.LifecycleStatus).HasDefaultValue(EventLifecycleStatus.Active).IsRequired();
            b.Property(e => e.AdminReviewNote).HasMaxLength(1000);
            b.Property(e => e.CancellationReason).HasMaxLength(1000);
            b.Property(e => e.CurrencyCode).IsRequired().HasMaxLength(3).HasDefaultValue("IRR");
            b.Property(e => e.MaleTicketPrice).HasPrecision(18, 2).IsRequired();
            b.Property(e => e.MaleTicketCurrencyCode).IsRequired().HasMaxLength(3).HasDefaultValue("IRR");
            b.Property(e => e.FemaleTicketPrice).HasPrecision(18, 2).IsRequired();
            b.Property(e => e.FemaleTicketCurrencyCode).IsRequired().HasMaxLength(3).HasDefaultValue("IRR");
            b.Property(e => e.EducationLevelRestriction).IsRequired();
            b.Property(e => e.EventImage1).HasMaxLength(500);
            b.Property(e => e.EventImage2).HasMaxLength(500);
            b.Property(e => e.EventImage3).HasMaxLength(500);
            b.Property(e => e.EventDescriptionHtml).IsRequired().HasMaxLength(10000);
            b.HasIndex(e => e.EventPlannerUserId);
            b.HasIndex(e => e.CurrencyCode);
            b.HasIndex(e => e.MaleTicketCurrencyCode);
            b.HasIndex(e => e.FemaleTicketCurrencyCode);
            b.HasQueryFilter(e => !e.IsDeleted);
            b.HasOne(e => e.EventPlannerUser)
                .WithMany()
                .HasForeignKey(e => e.EventPlannerUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(e => e.ApprovedByUser)
                .WithMany()
                .HasForeignKey(e => e.ApprovedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(e => e.CancelledByUser)
                .WithMany()
                .HasForeignKey(e => e.CancelledByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(e => e.OrganizerPaymentAccount)
                .WithMany()
                .HasForeignKey(e => e.OrganizerPaymentAccountId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(e => e.EventType)
                .WithMany()
                .HasForeignKey(e => e.EventTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(e => e.EventMode)
                .WithMany()
                .HasForeignKey(e => e.EventModeId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(e => e.OnlineEventPlatform)
                .WithMany()
                .HasForeignKey(e => e.OnlineEventPlatformId)
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
            b.HasMany(e => e.Faqs)
                .WithOne(faq => faq.DatingEvent)
                .HasForeignKey(faq => faq.DatingEventId)
                .OnDelete(DeleteBehavior.Cascade);
            b.Navigation(e => e.Faqs).UsePropertyAccessMode(PropertyAccessMode.Field);
            b.HasMany(e => e.DiscountCodes)
                .WithOne(discountCode => discountCode.DatingEvent)
                .HasForeignKey(discountCode => discountCode.DatingEventId)
                .OnDelete(DeleteBehavior.Restrict);
            b.Navigation(e => e.DiscountCodes).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<EventFaq>(b =>
        {
            b.ToTable("EventFaqs");
            b.HasKey(faq => faq.Id);
            b.Property(faq => faq.Question).IsRequired().HasMaxLength(250);
            b.Property(faq => faq.Answer).IsRequired().HasMaxLength(1200);
            b.Property(faq => faq.DisplayOrder).IsRequired();
            b.HasIndex(faq => new { faq.DatingEventId, faq.DisplayOrder }).IsUnique();
            b.HasQueryFilter(faq => !faq.DatingEvent.IsDeleted);
        });

        modelBuilder.Entity<EventDiscountCode>(b =>
        {
            b.ToTable("EventDiscountCodes");
            b.HasKey(discountCode => discountCode.Id);
            b.Property(discountCode => discountCode.Code).IsRequired().HasMaxLength(50);
            b.Property(discountCode => discountCode.Title).HasMaxLength(120);
            b.Property(discountCode => discountCode.Description).HasMaxLength(500);
            b.Property(discountCode => discountCode.Value).HasPrecision(18, 2).IsRequired();
            b.Property(discountCode => discountCode.GenderScope).IsRequired();
            b.Property(discountCode => discountCode.DiscountType).IsRequired();
            b.Property(discountCode => discountCode.StartsAtUtc).IsRequired();
            b.Property(discountCode => discountCode.EndsAtUtc).IsRequired();
            b.Property(discountCode => discountCode.MaxUsageCount).IsRequired();
            b.Property(discountCode => discountCode.UsedCount).IsRequired();
            b.Property(discountCode => discountCode.IsActive).IsRequired();
            b.HasIndex(discountCode => new { discountCode.DatingEventId, discountCode.Code }).IsUnique();
            b.HasIndex(discountCode => discountCode.Code)
                .IsUnique()
                .HasDatabaseName("IX_EventDiscountCodes_Global_Code")
                .HasFilter("[DatingEventId] IS NULL");
            b.HasIndex(discountCode => new { discountCode.IsActive, discountCode.StartsAtUtc, discountCode.EndsAtUtc });
            b.HasOne(discountCode => discountCode.DatingEvent)
                .WithMany(datingEvent => datingEvent.DiscountCodes)
                .HasForeignKey(discountCode => discountCode.DatingEventId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasQueryFilter(discountCode => !discountCode.IsDeleted && (discountCode.DatingEvent == null || !discountCode.DatingEvent.IsDeleted));
        });

        modelBuilder.Entity<EventLike>(b =>
        {
            b.ToTable("EventLikes");
            b.HasKey(like => like.Id);
            b.Property(like => like.Status).IsRequired();
            b.HasIndex(like => new { like.DatingEventId, like.FromUserId, like.ToUserId }).IsUnique();
            b.HasIndex(like => new { like.DatingEventId, like.ToUserId, like.Status });
            b.HasQueryFilter(like => !like.IsDeleted && !like.DatingEvent.IsDeleted);
            b.HasOne(like => like.DatingEvent)
                .WithMany()
                .HasForeignKey(like => like.DatingEventId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(like => like.FromUser)
                .WithMany()
                .HasForeignKey(like => like.FromUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(like => like.ToUser)
                .WithMany()
                .HasForeignKey(like => like.ToUserId)
                .OnDelete(DeleteBehavior.Restrict);
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

        modelBuilder.Entity<TicketOrder>(b =>
        {
            b.ToTable("TicketOrders");
            b.HasKey(order => order.Id);
            b.Property(order => order.CurrencyCode).IsRequired().HasMaxLength(3).HasDefaultValue("IRR");
            b.Property(order => order.GrossAmount).HasPrecision(18, 2).IsRequired();
            b.Property(order => order.DiscountAmount).HasPrecision(18, 2).IsRequired();
            b.Property(order => order.NetAmount).HasPrecision(18, 2).IsRequired();
            b.Property(order => order.PlatformCommissionAmount).HasPrecision(18, 2).IsRequired();
            b.Property(order => order.OrganizerIncomeAmount).HasPrecision(18, 2).IsRequired();
            b.Property(order => order.PaymentCollectionMethod).IsRequired();
            b.Property(order => order.PaymentStatus).IsRequired();
            b.Property(order => order.OrderStatus).IsRequired();
            b.Property(order => order.DiscountCode).HasMaxLength(50);
            b.Property(order => order.ReportingCurrencyCode).IsRequired().HasMaxLength(3).HasDefaultValue("IRR");
            b.Property(order => order.ReportingGrossAmountIrr).HasPrecision(18, 2).IsRequired();
            b.Property(order => order.ReportingDiscountAmountIrr).HasPrecision(18, 2).IsRequired();
            b.Property(order => order.ReportingNetAmountIrr).HasPrecision(18, 2).IsRequired();
            b.Property(order => order.ReportingPlatformCommissionIrr).HasPrecision(18, 2).IsRequired();
            b.Property(order => order.ReportingOrganizerIncomeIrr).HasPrecision(18, 2).IsRequired();
            b.Property(order => order.ExchangeRateToIrr).HasPrecision(18, 6).IsRequired().HasDefaultValue(1m);
            b.Property(order => order.ExchangeRateCapturedAtUtc).HasDefaultValueSql("GETUTCDATE()").IsRequired();
            b.Property(order => order.Notes).HasMaxLength(500);
            b.HasIndex(order => order.DatingEventId);
            b.HasIndex(order => order.BuyerUserId);
            b.HasIndex(order => new { order.PaymentStatus, order.CreatedAt });
            b.HasIndex(order => new { order.OrderStatus, order.CreatedAt });
            b.HasIndex(order => order.CurrencyCode);
            b.HasIndex(order => order.ExchangeRateId);
            b.HasIndex(order => order.EventDiscountCodeId);
            b.HasIndex(order => order.ApprovedByUserId);
            b.HasQueryFilter(order => !order.IsDeleted && !order.DatingEvent.IsDeleted && !order.BuyerUser.IsDeleted);
            b.HasOne(order => order.DatingEvent)
                .WithMany()
                .HasForeignKey(order => order.DatingEventId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(order => order.BuyerUser)
                .WithMany()
                .HasForeignKey(order => order.BuyerUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasMany(order => order.Tickets)
                .WithOne(ticket => ticket.TicketOrder)
                .HasForeignKey(ticket => ticket.TicketOrderId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(order => order.EventDiscountCode)
                .WithMany()
                .HasForeignKey(order => order.EventDiscountCodeId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(order => order.ExchangeRate)
                .WithMany()
                .HasForeignKey(order => order.ExchangeRateId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(order => order.ApprovedByUser)
                .WithMany()
                .HasForeignKey(order => order.ApprovedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.Navigation(order => order.Tickets).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<EventTicket>(b =>
        {
            b.HasKey(t => t.Id);
            b.Property(t => t.TicketOrderId).IsRequired();
            b.Property(t => t.OriginalPrice).HasPrecision(18, 2).IsRequired();
            b.Property(t => t.CurrencyCode).IsRequired().HasMaxLength(3).HasDefaultValue("IRR");
            b.Property(t => t.ReportingOriginalPriceIrr).HasPrecision(18, 2).IsRequired();
            b.Property(t => t.ReportingPriceIrr).HasPrecision(18, 2).IsRequired();
            b.Property(t => t.ExchangeRateToIrr).HasPrecision(18, 6).IsRequired().HasDefaultValue(1m);
            b.Property(t => t.ExchangeRateCapturedAtUtc).HasDefaultValueSql("GETUTCDATE()").IsRequired();
            b.Property(t => t.DiscountAmount).HasPrecision(18, 2).IsRequired();
            b.Property(t => t.DiscountCode).HasMaxLength(50);
            b.Property(t => t.Price).HasPrecision(18, 2).IsRequired();
            b.Property(t => t.RemovalReason).HasMaxLength(500);
            b.HasIndex(t => new { t.DatingEventId, t.UserId }).IsUnique();
            b.HasIndex(t => t.TicketOrderId);
            b.HasIndex(t => t.CurrencyCode);
            b.HasIndex(t => t.ExchangeRateId);
            b.HasIndex(t => t.EventDiscountCodeId);
            b.HasQueryFilter(t => !t.DatingEvent.IsDeleted);
            b.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(t => t.TicketOrder)
                .WithMany(order => order.Tickets)
                .HasForeignKey(t => t.TicketOrderId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(t => t.EventDiscountCode)
                .WithMany()
                .HasForeignKey(t => t.EventDiscountCodeId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(t => t.ExchangeRate)
                .WithMany()
                .HasForeignKey(t => t.ExchangeRateId)
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

        modelBuilder.Entity<SupportTicket>(b =>
        {
            b.ToTable("SupportTickets");
            b.HasKey(ticket => ticket.Id);
            b.Property(ticket => ticket.Title).IsRequired().HasMaxLength(180);
            b.Property(ticket => ticket.TicketTypeId).HasDefaultValue(3L).IsRequired();
            b.Property(ticket => ticket.TicketStatusId).HasDefaultValue(1L).IsRequired();
            b.Property(ticket => ticket.TicketRecipientTypeId).HasDefaultValue(1L).IsRequired();
            b.Property(ticket => ticket.Category).IsRequired();
            b.Property(ticket => ticket.Status).IsRequired();
            b.Property(ticket => ticket.SubmitterRole).IsRequired();
            b.HasIndex(ticket => new { ticket.Status, ticket.Category, ticket.CreatedAt });
            b.HasIndex(ticket => new { ticket.TicketStatusId, ticket.TicketTypeId, ticket.TicketRecipientTypeId, ticket.CreatedAt });
            b.HasIndex(ticket => ticket.SubmitterUserId);
            b.HasIndex(ticket => ticket.AssignedSupportUserId);
            b.HasIndex(ticket => ticket.RecipientPlannerUserId);
            b.HasIndex(ticket => ticket.DatingEventId);
            b.HasQueryFilter(ticket => !ticket.IsDeleted);
            b.HasOne(ticket => ticket.TicketType)
                .WithMany()
                .HasForeignKey(ticket => ticket.TicketTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(ticket => ticket.TicketStatus)
                .WithMany()
                .HasForeignKey(ticket => ticket.TicketStatusId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(ticket => ticket.TicketRecipientType)
                .WithMany()
                .HasForeignKey(ticket => ticket.TicketRecipientTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(ticket => ticket.SubmitterUser)
                .WithMany()
                .HasForeignKey(ticket => ticket.SubmitterUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(ticket => ticket.AssignedSupportUser)
                .WithMany()
                .HasForeignKey(ticket => ticket.AssignedSupportUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(ticket => ticket.DatingEvent)
                .WithMany()
                .HasForeignKey(ticket => ticket.DatingEventId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(ticket => ticket.RecipientPlannerUser)
                .WithMany()
                .HasForeignKey(ticket => ticket.RecipientPlannerUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasMany(ticket => ticket.Messages)
                .WithOne(message => message.SupportTicket)
                .HasForeignKey(message => message.SupportTicketId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasMany(ticket => ticket.History)
                .WithOne(history => history.SupportTicket)
                .HasForeignKey(history => history.SupportTicketId)
                .OnDelete(DeleteBehavior.Cascade);
            b.Navigation(ticket => ticket.Messages).UsePropertyAccessMode(PropertyAccessMode.Field);
            b.Navigation(ticket => ticket.History).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<SupportTicketStatusLookup>(b =>
        {
            b.ToTable("SupportTicketStatuses");
            b.HasKey(status => status.Id);
            b.Property(status => status.Name).IsRequired().HasMaxLength(80);
            b.Property(status => status.DisplayNameFa).IsRequired().HasMaxLength(120);
            b.Property(status => status.IsActive).IsRequired();
            b.Property(status => status.DisplayOrder).IsRequired();
            b.HasIndex(status => status.Name).IsUnique();
            b.HasQueryFilter(status => !status.IsDeleted);
        });

        modelBuilder.Entity<SupportTicketCategoryLookup>(b =>
        {
            b.ToTable("SupportTicketCategories");
            b.HasKey(category => category.Id);
            b.Property(category => category.Name).IsRequired().HasMaxLength(80);
            b.Property(category => category.DisplayNameFa).IsRequired().HasMaxLength(120);
            b.Property(category => category.IsActive).IsRequired();
            b.Property(category => category.DisplayOrder).IsRequired();
            b.HasIndex(category => category.Name).IsUnique();
            b.HasQueryFilter(category => !category.IsDeleted);
        });

        modelBuilder.Entity<SupportTicketRecipientTypeLookup>(b =>
        {
            b.ToTable("SupportTicketRecipientTypes");
            b.HasKey(recipient => recipient.Id);
            b.Property(recipient => recipient.Name).IsRequired().HasMaxLength(80);
            b.Property(recipient => recipient.DisplayNameFa).IsRequired().HasMaxLength(120);
            b.Property(recipient => recipient.IsActive).IsRequired();
            b.Property(recipient => recipient.DisplayOrder).IsRequired();
            b.HasIndex(recipient => recipient.Name).IsUnique();
            b.HasQueryFilter(recipient => !recipient.IsDeleted);
        });

        modelBuilder.Entity<SupportTicketMessage>(b =>
        {
            b.ToTable("SupportTicketMessages");
            b.HasKey(message => message.Id);
            b.Property(message => message.Body).IsRequired().HasMaxLength(4000);
            b.Property(message => message.SenderRole).IsRequired();
            b.HasIndex(message => message.SupportTicketId);
            b.HasQueryFilter(message => !message.SupportTicket.IsDeleted);
            b.HasOne(message => message.SenderUser)
                .WithMany()
                .HasForeignKey(message => message.SenderUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(message => message.RepresentedUser)
                .WithMany()
                .HasForeignKey(message => message.RepresentedUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasMany(message => message.Attachments)
                .WithOne(attachment => attachment.Message)
                .HasForeignKey(attachment => attachment.SupportTicketMessageId)
                .OnDelete(DeleteBehavior.Cascade);
            b.Navigation(message => message.Attachments).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<SupportTicketAttachment>(b =>
        {
            b.ToTable("SupportTicketAttachments");
            b.HasKey(attachment => attachment.Id);
            b.Property(attachment => attachment.FileName).IsRequired().HasMaxLength(260);
            b.Property(attachment => attachment.ContentType).IsRequired().HasMaxLength(100);
            b.Property(attachment => attachment.Url).IsRequired().HasMaxLength(1000);
            b.Property(attachment => attachment.SizeBytes).IsRequired();
            b.HasQueryFilter(attachment => !attachment.Message.SupportTicket.IsDeleted);
        });

        modelBuilder.Entity<SupportTicketHistoryEntry>(b =>
        {
            b.ToTable("SupportTicketHistoryEntries");
            b.HasKey(history => history.Id);
            b.Property(history => history.Action).IsRequired().HasMaxLength(80);
            b.Property(history => history.Note).HasMaxLength(1000);
            b.HasIndex(history => history.SupportTicketId);
            b.HasQueryFilter(history => !history.SupportTicket.IsDeleted);
            b.HasOne(history => history.ActorUser)
                .WithMany()
                .HasForeignKey(history => history.ActorUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<SupportTicketAssignmentCursor>(b =>
        {
            b.ToTable("SupportTicketAssignmentCursors");
            b.HasKey(cursor => cursor.Id);
            b.Property(cursor => cursor.QueueName).IsRequired().HasMaxLength(80);
            b.HasIndex(cursor => cursor.QueueName).IsUnique();
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

        modelBuilder.Entity<NotificationMessageTypeLookup>(b =>
        {
            b.ToTable("NotificationMessageTypes");
            b.HasKey(item => item.Id);
            b.Property(item => item.Type).IsRequired();
            b.Property(item => item.Code).IsRequired().HasMaxLength(80);
            b.Property(item => item.DisplayNameFa).IsRequired().HasMaxLength(120);
            b.Property(item => item.DescriptionFa).IsRequired().HasMaxLength(500);
            b.Property(item => item.RequiresApproval).IsRequired();
            b.Property(item => item.SupportsSms).IsRequired();
            b.Property(item => item.AllowedSenderRoles).IsRequired().HasMaxLength(200);
            b.Property(item => item.AllowedTargets).IsRequired().HasMaxLength(300);
            b.Property(item => item.DefaultPriority).IsRequired();
            b.Property(item => item.IsActive).IsRequired();
            b.Property(item => item.DisplayOrder).IsRequired();
            b.HasIndex(item => item.Code).IsUnique();
            b.HasIndex(item => new { item.IsActive, item.DisplayOrder });
            b.HasQueryFilter(item => !item.IsDeleted);
            b.HasData(
                new { Id = 1L, Type = NotificationType.System, Code = "System", DisplayNameFa = "پیام سیستمی", DescriptionFa = "پیام‌های خودکار سیستم برای اطلاع‌رسانی داخلی.", RequiresApproval = false, SupportsSms = false, AllowedSenderRoles = "Admin,PlatformSupportTeam", AllowedTargets = "User,EventParticipants,EventBuyers,Planners", DefaultPriority = NotificationPriority.Normal, IsActive = true, DisplayOrder = 10, CreatedAt = new DateTime(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 2L, Type = NotificationType.AdminToPlanner, Code = "AdminToPlanner", DisplayNameFa = "پیام مدیر به برگزارکننده", DescriptionFa = "پیام مدیریتی یا عملیاتی برای برگزارکننده‌ها.", RequiresApproval = false, SupportsSms = false, AllowedSenderRoles = "Admin,PlatformSupportTeam", AllowedTargets = "User,Planners", DefaultPriority = NotificationPriority.Normal, IsActive = true, DisplayOrder = 20, CreatedAt = new DateTime(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 3L, Type = NotificationType.PlannerToParticipant, Code = "PlannerToParticipant", DisplayNameFa = "پیام برگزارکننده به شرکت‌کننده", DescriptionFa = "پیام برگزارکننده فقط برای شرکت‌کنندگان یا خریداران رویدادهای خودش.", RequiresApproval = true, SupportsSms = true, AllowedSenderRoles = "EventPlanner", AllowedTargets = "EventParticipants,EventBuyers,User", DefaultPriority = NotificationPriority.Important, IsActive = true, DisplayOrder = 30, CreatedAt = new DateTime(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 4L, Type = NotificationType.AdminToUser, Code = "AdminToUser", DisplayNameFa = "پیام مدیر به کاربر", DescriptionFa = "پیام مستقیم مدیر یا پشتیبان به یک کاربر یا گروه مجاز.", RequiresApproval = false, SupportsSms = false, AllowedSenderRoles = "Admin,PlatformSupportTeam", AllowedTargets = "User,EventParticipants,EventBuyers", DefaultPriority = NotificationPriority.Normal, IsActive = true, DisplayOrder = 40, CreatedAt = new DateTime(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 5L, Type = NotificationType.EventUpdate, Code = "EventUpdate", DisplayNameFa = "اطلاع‌رسانی رویداد", DescriptionFa = "اطلاع‌رسانی تغییرات زمان، مکان یا جزئیات رویداد.", RequiresApproval = true, SupportsSms = true, AllowedSenderRoles = "Admin,PlatformSupportTeam,EventPlanner", AllowedTargets = "EventParticipants,EventBuyers", DefaultPriority = NotificationPriority.Important, IsActive = true, DisplayOrder = 50, CreatedAt = new DateTime(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 6L, Type = NotificationType.Finance, Code = "Finance", DisplayNameFa = "پیام مالی", DescriptionFa = "اطلاع‌رسانی مالی، رسید، تسویه یا وضعیت پرداخت.", RequiresApproval = false, SupportsSms = false, AllowedSenderRoles = "Admin,PlatformSupportTeam", AllowedTargets = "User,EventParticipants,EventBuyers,Planners", DefaultPriority = NotificationPriority.Important, IsActive = true, DisplayOrder = 60, CreatedAt = new DateTime(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 7L, Type = NotificationType.Refund, Code = "Refund", DisplayNameFa = "بازگشت وجه", DescriptionFa = "اطلاع‌رسانی مربوط به درخواست یا نتیجه بازگشت وجه.", RequiresApproval = false, SupportsSms = false, AllowedSenderRoles = "Admin,PlatformSupportTeam", AllowedTargets = "User,EventParticipants,EventBuyers", DefaultPriority = NotificationPriority.Important, IsActive = true, DisplayOrder = 70, CreatedAt = new DateTime(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false });
        });

        modelBuilder.Entity<NotificationPriorityLookup>(b =>
        {
            b.ToTable("NotificationPriorities");
            b.HasKey(item => item.Id);
            b.Property(item => item.Priority).IsRequired();
            b.Property(item => item.Code).IsRequired().HasMaxLength(80);
            b.Property(item => item.DisplayNameFa).IsRequired().HasMaxLength(80);
            b.Property(item => item.DescriptionFa).IsRequired().HasMaxLength(300);
            b.Property(item => item.IsActive).IsRequired();
            b.Property(item => item.DisplayOrder).IsRequired();
            b.HasIndex(item => item.Code).IsUnique();
            b.HasQueryFilter(item => !item.IsDeleted);
            b.HasData(
                new { Id = 1L, Priority = NotificationPriority.Normal, Code = "Normal", DisplayNameFa = "عادی", DescriptionFa = "پیام اطلاع‌رسانی معمولی.", IsActive = true, DisplayOrder = 10, CreatedAt = new DateTime(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 2L, Priority = NotificationPriority.Important, Code = "Important", DisplayNameFa = "مهم", DescriptionFa = "پیامی که بهتر است کاربر زودتر ببیند.", IsActive = true, DisplayOrder = 20, CreatedAt = new DateTime(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 3L, Priority = NotificationPriority.Critical, Code = "Critical", DisplayNameFa = "فوری", DescriptionFa = "پیام حساس درباره تغییر مهم، مالی یا لغو.", IsActive = true, DisplayOrder = 30, CreatedAt = new DateTime(2026, 6, 15, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false });
        });

        modelBuilder.Entity<Notification>(b =>
        {
            b.ToTable("Notifications");
            b.HasKey(notification => notification.Id);
            b.Property(notification => notification.Type).IsRequired();
            b.Property(notification => notification.Priority).IsRequired();
            b.Property(notification => notification.ApprovalStatus).IsRequired();
            b.Property(notification => notification.Title).IsRequired().HasMaxLength(180);
            b.Property(notification => notification.Body).IsRequired().HasMaxLength(2000);
            b.Property(notification => notification.ReferenceType).HasMaxLength(100);
            b.Property(notification => notification.ReviewNote).HasMaxLength(1000);
            b.HasIndex(notification => new { notification.ApprovalStatus, notification.CreatedAt });
            b.HasIndex(notification => new { notification.Type, notification.CreatedAt });
            b.HasIndex(notification => notification.DatingEventId);
            b.HasIndex(notification => notification.CreatedByUserId);
            b.HasIndex(notification => notification.ReviewedByUserId);
            b.HasIndex(notification => new { notification.ReferenceType, notification.ReferenceId });
            b.HasQueryFilter(notification => !notification.IsDeleted);
            b.HasOne(notification => notification.DatingEvent)
                .WithMany()
                .HasForeignKey(notification => notification.DatingEventId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(notification => notification.CreatedByUser)
                .WithMany()
                .HasForeignKey(notification => notification.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(notification => notification.ReviewedByUser)
                .WithMany()
                .HasForeignKey(notification => notification.ReviewedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasMany(notification => notification.Recipients)
                .WithOne(recipient => recipient.Notification)
                .HasForeignKey(recipient => recipient.NotificationId)
                .OnDelete(DeleteBehavior.Cascade);
            b.Navigation(notification => notification.Recipients).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<NotificationRecipient>(b =>
        {
            b.ToTable("NotificationRecipients");
            b.HasKey(recipient => recipient.Id);
            b.Property(recipient => recipient.Channel).IsRequired();
            b.Property(recipient => recipient.Status).IsRequired();
            b.Property(recipient => recipient.FailureReason).HasMaxLength(1000);
            b.HasIndex(recipient => new { recipient.RecipientUserId, recipient.Status, recipient.CreatedAt });
            b.HasIndex(recipient => new { recipient.NotificationId, recipient.RecipientUserId, recipient.Channel }).IsUnique();
            b.HasIndex(recipient => recipient.ReadAtUtc);
            b.HasQueryFilter(recipient => !recipient.IsDeleted && !recipient.Notification.IsDeleted && !recipient.RecipientUser.IsDeleted);
            b.HasOne(recipient => recipient.RecipientUser)
                .WithMany()
                .HasForeignKey(recipient => recipient.RecipientUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<NotificationTemplate>(b =>
        {
            b.ToTable("NotificationTemplates");
            b.HasKey(template => template.Id);
            b.Property(template => template.Code).IsRequired().HasMaxLength(80);
            b.Property(template => template.TitleTemplate).IsRequired().HasMaxLength(180);
            b.Property(template => template.BodyTemplate).IsRequired().HasMaxLength(2000);
            b.Property(template => template.Type).IsRequired();
            b.Property(template => template.Priority).IsRequired();
            b.Property(template => template.RequiresApproval).IsRequired();
            b.Property(template => template.IsActive).IsRequired();
            b.HasIndex(template => template.Code).IsUnique();
            b.HasIndex(template => new { template.Type, template.IsActive });
            b.HasQueryFilter(template => !template.IsDeleted);
            b.HasData(
                new { Id = 1L, Code = "event-cancelled", TitleTemplate = "لغو رویداد", BodyTemplate = "رویداد {EventTitle} لغو شد و مبلغ پرداختی به کیف پول شما اضافه می‌شود.", Type = NotificationType.EventUpdate, Priority = NotificationPriority.Critical, RequiresApproval = false, IsActive = true, CreatedAt = new DateTime(2026, 6, 14, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 2L, Code = "refund-approved", TitleTemplate = "بازگشت وجه تایید شد", BodyTemplate = "درخواست بازگشت وجه شما برای {EventTitle} تایید و مبلغ به کیف پول اضافه شد.", Type = NotificationType.Refund, Priority = NotificationPriority.Important, RequiresApproval = false, IsActive = true, CreatedAt = new DateTime(2026, 6, 14, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false },
                new { Id = 3L, Code = "manual-receipt-wallet-credit", TitleTemplate = "رسید پرداخت به کیف پول منتقل شد", BodyTemplate = "رسید پرداخت شما برای رویداد لغوشده {EventTitle} تایید شد و مبلغ به کیف پول اضافه شد.", Type = NotificationType.Finance, Priority = NotificationPriority.Important, RequiresApproval = false, IsActive = true, CreatedAt = new DateTime(2026, 6, 14, 0, 0, 0, DateTimeKind.Utc), IsDeleted = false });
        });

        modelBuilder.Entity<EventWorkflowLog>(b =>
        {
            b.HasKey(log => log.Id);
            b.Property(log => log.ActionType).IsRequired();
            b.Property(log => log.Reason).HasMaxLength(1000);
            b.Property(log => log.BeforeJson).HasMaxLength(8000);
            b.Property(log => log.AfterJson).HasMaxLength(8000);
            b.Property(log => log.MetadataJson).HasMaxLength(4000);
            b.HasIndex(log => new { log.DatingEventId, log.CreatedAt });
            b.HasIndex(log => log.ActionType);
            b.HasQueryFilter(log => !log.IsDeleted);
            b.HasOne(log => log.DatingEvent)
                .WithMany()
                .HasForeignKey(log => log.DatingEventId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(log => log.ActorUser)
                .WithMany()
                .HasForeignKey(log => log.ActorUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EventChangeRequest>(b =>
        {
            b.HasKey(request => request.Id);
            b.Property(request => request.Status).IsRequired();
            b.Property(request => request.Reason).HasMaxLength(1000);
            b.Property(request => request.BeforeJson).IsRequired().HasMaxLength(8000);
            b.Property(request => request.AfterJson).IsRequired().HasMaxLength(8000);
            b.Property(request => request.ReviewNote).HasMaxLength(1000);
            b.HasIndex(request => new { request.DatingEventId, request.Status, request.RequestedAtUtc });
            b.HasQueryFilter(request => !request.IsDeleted);
            b.HasOne(request => request.DatingEvent)
                .WithMany()
                .HasForeignKey(request => request.DatingEventId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(request => request.RequestedByUser)
                .WithMany()
                .HasForeignKey(request => request.RequestedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(request => request.ReviewedByUser)
                .WithMany()
                .HasForeignKey(request => request.ReviewedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EventCancellationRequest>(b =>
        {
            b.HasKey(request => request.Id);
            b.Property(request => request.Status).IsRequired();
            b.Property(request => request.Reason).IsRequired().HasMaxLength(1000);
            b.Property(request => request.ReviewNote).HasMaxLength(1000);
            b.Property(request => request.PublicMessage).HasMaxLength(1000);
            b.Property(request => request.PreviewJson).HasMaxLength(8000);
            b.Property(request => request.ExecutedAtUtc);
            b.HasIndex(request => new { request.DatingEventId, request.Status, request.RequestedAtUtc });
            b.HasQueryFilter(request => !request.IsDeleted);
            b.HasOne(request => request.DatingEvent)
                .WithMany()
                .HasForeignKey(request => request.DatingEventId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(request => request.RequestedByUser)
                .WithMany()
                .HasForeignKey(request => request.RequestedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(request => request.ReviewedByUser)
                .WithMany()
                .HasForeignKey(request => request.ReviewedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EventSettlementRequest>(b =>
        {
            b.HasKey(request => request.Id);
            b.Property(request => request.Status).IsRequired();
            b.Property(request => request.GrossAmount).HasPrecision(18, 2).IsRequired();
            b.Property(request => request.PlatformCommissionAmount).HasPrecision(18, 2).IsRequired();
            b.Property(request => request.OrganizerIncomeAmount).HasPrecision(18, 2).IsRequired();
            b.Property(request => request.ReportingOrganizerIncomeIrr).HasPrecision(18, 2).IsRequired();
            b.Property(request => request.RequestNote).HasMaxLength(1000);
            b.Property(request => request.ReviewNote).HasMaxLength(1000);
            b.HasIndex(request => new { request.DatingEventId, request.Status, request.RequestedAtUtc });
            b.HasIndex(request => request.OrganizerCreditTransactionId);
            b.HasQueryFilter(request => !request.IsDeleted);
            b.HasOne(request => request.DatingEvent)
                .WithMany()
                .HasForeignKey(request => request.DatingEventId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(request => request.RequestedByUser)
                .WithMany()
                .HasForeignKey(request => request.RequestedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(request => request.ReviewedByUser)
                .WithMany()
                .HasForeignKey(request => request.ReviewedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.HasOne(request => request.OrganizerCreditTransaction)
                .WithMany()
                .HasForeignKey(request => request.OrganizerCreditTransactionId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static object[] CreatePermissionActionSeeds()
    {
        var createdAt = new DateTime(2026, 6, 10, 0, 0, 0, DateTimeKind.Utc);
        return new object[]
        {
            new { Id = 1L, Entity = "participants", Action = "list", Label = "مشاهده فهرست", Description = "دیدن فهرست شرکت‌کنندگان", IsActive = true, DisplayOrder = 1, CreatedAt = createdAt, IsDeleted = false },
            new { Id = 2L, Entity = "participants", Action = "viewDetails", Label = "مشاهده جزئیات", Description = "باز کردن پروفایل شرکت‌کننده", IsActive = true, DisplayOrder = 2, CreatedAt = createdAt, IsDeleted = false },
            new { Id = 3L, Entity = "participants", Action = "viewContactInfo", Label = "مشاهده اطلاعات تماس", Description = "نمایش شماره موبایل و اطلاعات تماس شرکت‌کننده", IsActive = true, DisplayOrder = 3, CreatedAt = createdAt, IsDeleted = false },
            new { Id = 4L, Entity = "participants", Action = "editProfile", Label = "ویرایش پروفایل", Description = "ویرایش اطلاعات پروفایل شرکت‌کننده", IsActive = true, DisplayOrder = 4, CreatedAt = createdAt, IsDeleted = false },
            new { Id = 5L, Entity = "participants", Action = "viewFinance", Label = "مشاهده مالی", Description = "مشاهده موجودی و پرداخت‌های شرکت‌کننده", IsActive = true, DisplayOrder = 5, CreatedAt = createdAt, IsDeleted = false },
            new { Id = 6L, Entity = "participants", Action = "viewOrder", Label = "مشاهده سفارش", Description = "مشاهده تراکنش یا سفارش مرتبط با بلیت", IsActive = true, DisplayOrder = 6, CreatedAt = createdAt, IsDeleted = false },
            new { Id = 7L, Entity = "participants", Action = "resendProfileLink", Label = "ارسال لینک تکمیل پروفایل", Description = "ارسال یا ارسال مجدد دعوت تکمیل پروفایل", IsActive = true, DisplayOrder = 7, CreatedAt = createdAt, IsDeleted = false },
            new { Id = 8L, Entity = "participants", Action = "changeStatus", Label = "تغییر وضعیت", Description = "تغییر وضعیت شرکت‌کننده در رویداد", IsActive = true, DisplayOrder = 8, CreatedAt = createdAt, IsDeleted = false },
            new { Id = 9L, Entity = "participants", Action = "replaceParticipant", Label = "جایگزینی شرکت‌کننده", Description = "جایگزین کردن شرکت‌کننده بلیت", IsActive = true, DisplayOrder = 9, CreatedAt = createdAt, IsDeleted = false },
            new { Id = 10L, Entity = "participants", Action = "emergencyRefund", Label = "بازگشت اضطراری", Description = "حذف اضطراری شرکت‌کننده و بازگشت وجه", IsActive = true, DisplayOrder = 10, CreatedAt = createdAt, IsDeleted = false },
            new { Id = 11L, Entity = "participants", Action = "export", Label = "خروجی گرفتن", Description = "دریافت خروجی از فهرست شرکت‌کنندگان", IsActive = true, DisplayOrder = 11, CreatedAt = createdAt, IsDeleted = false },
            new { Id = 12L, Entity = "events", Action = "viewParticipants", Label = "فهرست شرکت‌کنندگان رویداد", Description = "ورود از رویداد به فهرست شرکت‌کنندگان", IsActive = true, DisplayOrder = 1, CreatedAt = createdAt, IsDeleted = false },
            new { Id = 13L, Entity = "orders", Action = "view", Label = "مشاهده سفارش", Description = "مشاهده سفارش‌ها و تراکنش‌های مرتبط", IsActive = true, DisplayOrder = 1, CreatedAt = createdAt, IsDeleted = false },
            new { Id = 14L, Entity = "users", Action = "manageOperationPermissions", Label = "مدیریت دسترسی عملیات", Description = "تغییر سطح دسترسی نقش‌ها و کاربران", IsActive = true, DisplayOrder = 1, CreatedAt = createdAt, IsDeleted = false }
        };
    }

    private static object[] CreateRoleOperationPermissionSeeds()
    {
        var createdAt = new DateTime(2026, 6, 10, 0, 0, 0, DateTimeKind.Utc);
        var seeds = new List<object>();
        long id = 1;
        var participantActions = new[]
        {
            "list",
            "viewDetails",
            "viewContactInfo",
            "editProfile",
            "viewFinance",
            "viewOrder",
            "resendProfileLink",
            "changeStatus",
            "replaceParticipant",
            "emergencyRefund",
            "export"
        };

        foreach (var action in participantActions)
        {
            seeds.Add(new { Id = id++, Role = UserRole.Admin, Entity = "participants", Action = action, Allowed = true, CreatedAt = createdAt, IsDeleted = false });
        }

        foreach (var action in participantActions)
        {
            var allowed = action is "list" or "viewDetails" or "viewContactInfo" or "viewOrder" or "resendProfileLink";
            seeds.Add(new { Id = id++, Role = UserRole.EventPlanner, Entity = "participants", Action = action, Allowed = allowed, CreatedAt = createdAt, IsDeleted = false });
        }

        foreach (var action in participantActions)
        {
            var allowed = action is "list" or "viewDetails" or "viewContactInfo" or "viewOrder" or "resendProfileLink" or "changeStatus";
            seeds.Add(new { Id = id++, Role = UserRole.PlatformSupportTeam, Entity = "participants", Action = action, Allowed = allowed, CreatedAt = createdAt, IsDeleted = false });
        }

        seeds.Add(new { Id = id++, Role = UserRole.Admin, Entity = "events", Action = "viewParticipants", Allowed = true, CreatedAt = createdAt, IsDeleted = false });
        seeds.Add(new { Id = id++, Role = UserRole.EventPlanner, Entity = "events", Action = "viewParticipants", Allowed = true, CreatedAt = createdAt, IsDeleted = false });
        seeds.Add(new { Id = id++, Role = UserRole.PlatformSupportTeam, Entity = "events", Action = "viewParticipants", Allowed = true, CreatedAt = createdAt, IsDeleted = false });
        seeds.Add(new { Id = id++, Role = UserRole.Admin, Entity = "orders", Action = "view", Allowed = true, CreatedAt = createdAt, IsDeleted = false });
        seeds.Add(new { Id = id++, Role = UserRole.EventPlanner, Entity = "orders", Action = "view", Allowed = true, CreatedAt = createdAt, IsDeleted = false });
        seeds.Add(new { Id = id++, Role = UserRole.PlatformSupportTeam, Entity = "orders", Action = "view", Allowed = true, CreatedAt = createdAt, IsDeleted = false });
        seeds.Add(new { Id = id, Role = UserRole.Admin, Entity = "users", Action = "manageOperationPermissions", Allowed = true, CreatedAt = createdAt, IsDeleted = false });

        return seeds.ToArray();
    }
}
