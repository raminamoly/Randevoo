# DbContext Analysis

## Purpose
Analyze DbContext configuration, relationships, indexes, and owned types.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

## DbSet declarations
- Users: User (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:14`)
- UserProfiles: UserProfile (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:15`)
- UserProfileImages: UserProfileImage (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:16`)
- Interests: Interest (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:17`)
- EventPlannerProfiles: EventPlannerProfile (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:18`)
- Countries: Country (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:19`)
- Cities: City (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:20`)
- EducationLevels: EducationLevelLookup (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:21`)
- Genders: GenderLookup (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:22`)
- ZodiacSigns: ZodiacSignLookup (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:23`)
- UserRoles: UserRoleLookup (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:24`)
- EventReviewStatuses: EventReviewStatusLookup (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:25`)
- EventDiscountTypes: EventDiscountTypeLookup (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:26`)
- BalanceTransactionTypes: BalanceTransactionTypeLookup (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:27`)
- Currencies: CurrencyLookup (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:28`)
- CurrencyExchangeRates: CurrencyExchangeRate (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:29`)
- BalanceAccounts: BalanceAccount (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:30`)
- BalanceTransactions: BalanceTransaction (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:31`)
- OnlinePayments: OnlinePayment (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:32`)
- ManualPaymentReceipts: ManualPaymentReceipt (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:33`)
- PlannerWithdrawalRequests: PlannerWithdrawalRequest (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:34`)
- PlannerBankAccounts: PlannerBankAccount (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:35`)
- DatingEvents: DatingEvent (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:36`)
- TicketOrders: TicketOrder (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:37`)
- EventModes: EventModeLookup (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:38`)
- OnlineEventPlatforms: OnlineEventPlatform (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:39`)
- EventFaqs: EventFaq (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:40`)
- EventDiscountCodes: EventDiscountCode (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:41`)
- Tags: Tag (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:42`)
- EventTags: EventTag (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:43`)
- EventTickets: EventTicket (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:44`)
- EventLikes: EventLike (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:45`)
- EventConversations: EventConversation (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:46`)
- EventChatMessages: EventChatMessage (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:47`)
- EventChatBlocks: EventChatBlock (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:48`)
- EventSurveyResponses: EventSurveyResponse (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:49`)
- EventSurveyRatings: EventSurveyRating (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:50`)
- EventTypes: EventType (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:51`)
- ModerationReports: ModerationReport (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:52`)
- SupportTickets: SupportTicket (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:53`)
- SupportTicketMessages: SupportTicketMessage (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:54`)
- SupportTicketAttachments: SupportTicketAttachment (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:55`)
- SupportTicketHistoryEntries: SupportTicketHistoryEntry (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:56`)
- SupportTicketAssignmentCursors: SupportTicketAssignmentCursor (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:57`)
- SupportTicketStatuses: SupportTicketStatusLookup (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:58`)
- SupportTicketCategories: SupportTicketCategoryLookup (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:59`)
- SupportTicketRecipientTypes: SupportTicketRecipientTypeLookup (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:60`)
- EventParticipantSmsRequests: EventParticipantSmsRequest (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:61`)
- SmsQueueItems: SmsQueueItem (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:62`)
- RefreshTokens: RefreshToken (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:63`)
- AuditLogs: AuditLog (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:64`)
- PermissionActions: PermissionAction (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:65`)
- RoleOperationPermissions: RoleOperationPermission (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:66`)
- UserOperationPermissionOverrides: UserOperationPermissionOverride (`src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:67`)

## Index examples
- `b.HasIndex(u => u.MobileNumber).IsUnique();`
- `b.HasIndex(u => u.Email).IsUnique() .HasFilter("[Email] IS NOT NULL");`
- `b.HasIndex(token => token.TokenHash).IsUnique();`
- `b.HasIndex(token => token.UserId);`
- `b.HasIndex(log => log.ActorUserId);`
- `b.HasIndex(log => new { log.LogType, log.CreatedAt });`
- `b.HasIndex(log => new { log.Module, log.CreatedAt });`
- `b.HasIndex(log => new { log.Status, log.CreatedAt });`
- `b.HasIndex(log => new { log.TargetType, log.TargetId });`
- `b.HasIndex(log => log.CreatedAt);`
- `b.HasIndex(role => role.Name).IsUnique();`
- `b.HasIndex(action => new { action.Entity, action.Action }).IsUnique();`
- `b.HasIndex(permission => new { permission.Role, permission.Entity, permission.Action }).IsUnique();`
- `b.HasIndex(permission => new { permission.Entity, permission.Action });`
- `b.HasIndex(permission => new { permission.UserId, permission.Entity, permission.Action }).IsUnique();`
- `b.HasIndex(permission => new { permission.Entity, permission.Action });`
- `b.HasIndex(permission => permission.ExpiresAtUtc);`
- `b.HasIndex(status => status.Name).IsUnique();`
- `b.HasIndex(type => type.Name).IsUnique();`
- `b.HasIndex(type => type.Name).IsUnique();`
- `b.HasIndex(currency => currency.Code).IsUnique();`
- `b.HasIndex(rate => new { rate.FromCurrencyCode, rate.ToCurrencyCode, rate.EffectiveFromUtc }).IsUnique();`
- `b.HasIndex(rate => new { rate.FromCurrencyCode, rate.ToCurrencyCode, rate.EffectiveToUtc });`
- `b.HasIndex(country => country.Name).IsUnique();`
- `b.HasIndex(country => country.Code).IsUnique();`
- `b.HasIndex(city => new { city.CountryId, city.Name }).IsUnique();`
- `b.HasIndex(level => level.Title).IsUnique();`
- `b.HasIndex(gender => gender.Title).IsUnique();`
- `b.HasIndex(sign => sign.Code).IsUnique();`
- `b.HasIndex(sign => sign.Title).IsUnique();`
- `b.HasIndex(p => p.UserId).IsUnique();`
- `b.HasIndex(mode => mode.Name).IsUnique();`
- `b.HasIndex(platform => platform.Name).IsUnique();`
- `b.HasIndex(a => a.UserId).IsUnique();`
- `b.HasIndex(t => t.UserId);`
- `b.HasIndex(t => t.CurrencyCode);`
- `b.HasIndex(t => t.ExchangeRateId);`
- `b.HasIndex(t => t.TicketOrderId);`
- `b.HasIndex(payment => payment.UserId);`
- `b.HasIndex(payment => payment.DatingEventId);`
- `b.HasIndex(payment => payment.EventTicketId);`
- `b.HasIndex(payment => payment.TicketOrderId);`
- `b.HasIndex(payment => payment.BalanceTransactionId);`
- `b.HasIndex(payment => payment.CurrencyCode);`
- `b.HasIndex(payment => payment.ExchangeRateId);`
- `b.HasIndex(payment => payment.TrackingCode).IsUnique();`
- `b.HasIndex(receipt => new { receipt.DestinationType, receipt.Status, receipt.SubmittedAtUtc });`
- `b.HasIndex(receipt => receipt.DatingEventId);`
- `b.HasIndex(receipt => receipt.ParticipantUserId);`
- `b.HasIndex(receipt => receipt.PlannerUserId);`
- `b.HasIndex(receipt => receipt.EventTicketId);`
- `b.HasIndex(receipt => receipt.TicketOrderId);`
- `b.HasIndex(receipt => receipt.EventDiscountCodeId);`
- `b.HasIndex(receipt => receipt.CurrencyCode);`
- `b.HasIndex(receipt => receipt.ExchangeRateId);`
- `b.HasIndex(request => request.CurrencyCode);`
- `b.HasIndex(request => request.ExchangeRateId);`
- `b.HasIndex(request => request.UserId);`
- `b.HasIndex(request => new { request.Status, request.RequestedAtUtc });`
- `b.HasIndex(account => account.UserId);`
- `b.HasIndex(account => account.CurrencyCode);`
- `b.HasIndex(account => account.Iban).IsUnique().HasFilter("[Iban] IS NOT NULL");`
- `b.HasIndex(p => p.DisplayName).IsUnique();`
- `b.HasIndex(p => p.UserId).IsUnique();`
- `b.HasIndex(p => p.CountryId);`
- `b.HasIndex(p => p.CityId);`
- `b.HasIndex(p => p.EducationLevelId);`
- `b.HasIndex(p => p.GenderId);`
- `b.HasIndex(p => p.ZodiacSignId);`
- `b.HasIndex(image => new { image.UserProfileId, image.DisplayOrder }).IsUnique();`
- `b.HasIndex(i => i.Name).IsUnique();`
- `b.HasIndex(e => e.EventTypeId);`
- `b.HasIndex(e => e.EventModeId);`
- `b.HasIndex(e => e.OnlineEventPlatformId);`
- `b.HasIndex(e => e.CountryId);`
- `b.HasIndex(e => e.CityId);`
- `b.HasIndex(e => e.MinimumEducationLevelId);`
- `b.HasIndex(e => new { e.IsCancelled, e.DateTimeEnd });`
- `b.HasIndex(e => new { e.IsCancelled, e.IsOpenForSell, e.DateTimeEnd });`
- `b.HasIndex(e => new { e.ReviewStatus, e.DateTimeStart });`

## Delete behaviors detected
- Cascade: 25
- Restrict: 70
- NoAction: 1

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
