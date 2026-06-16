# Database Overview

## Purpose
Summarize EF Core persistence design.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

EF Core is the persistence layer. `RandevooDbContext` exposes 54 DbSets, migrations under `Infrastructure/Data/Migrations`, repositories under `Infrastructure/Repositories`, and startup initialization in `RandevooDatabaseInitializer`.

| DbSet | Entity | Source |
| --- | --- | --- |
| Users | User | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:14` |
| UserProfiles | UserProfile | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:15` |
| UserProfileImages | UserProfileImage | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:16` |
| Interests | Interest | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:17` |
| EventPlannerProfiles | EventPlannerProfile | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:18` |
| Countries | Country | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:19` |
| Cities | City | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:20` |
| EducationLevels | EducationLevelLookup | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:21` |
| Genders | GenderLookup | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:22` |
| ZodiacSigns | ZodiacSignLookup | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:23` |
| UserRoles | UserRoleLookup | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:24` |
| EventReviewStatuses | EventReviewStatusLookup | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:25` |
| EventDiscountTypes | EventDiscountTypeLookup | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:26` |
| BalanceTransactionTypes | BalanceTransactionTypeLookup | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:27` |
| Currencies | CurrencyLookup | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:28` |
| CurrencyExchangeRates | CurrencyExchangeRate | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:29` |
| BalanceAccounts | BalanceAccount | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:30` |
| BalanceTransactions | BalanceTransaction | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:31` |
| OnlinePayments | OnlinePayment | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:32` |
| ManualPaymentReceipts | ManualPaymentReceipt | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:33` |
| PlannerWithdrawalRequests | PlannerWithdrawalRequest | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:34` |
| PlannerBankAccounts | PlannerBankAccount | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:35` |
| DatingEvents | DatingEvent | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:36` |
| TicketOrders | TicketOrder | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:37` |
| EventModes | EventModeLookup | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:38` |
| OnlineEventPlatforms | OnlineEventPlatform | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:39` |
| EventFaqs | EventFaq | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:40` |
| EventDiscountCodes | EventDiscountCode | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:41` |
| Tags | Tag | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:42` |
| EventTags | EventTag | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:43` |
| EventTickets | EventTicket | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:44` |
| EventLikes | EventLike | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:45` |
| EventConversations | EventConversation | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:46` |
| EventChatMessages | EventChatMessage | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:47` |
| EventChatBlocks | EventChatBlock | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:48` |
| EventSurveyResponses | EventSurveyResponse | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:49` |
| EventSurveyRatings | EventSurveyRating | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:50` |
| EventTypes | EventType | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:51` |
| ModerationReports | ModerationReport | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:52` |
| SupportTickets | SupportTicket | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:53` |
| SupportTicketMessages | SupportTicketMessage | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:54` |
| SupportTicketAttachments | SupportTicketAttachment | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:55` |
| SupportTicketHistoryEntries | SupportTicketHistoryEntry | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:56` |
| SupportTicketAssignmentCursors | SupportTicketAssignmentCursor | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:57` |
| SupportTicketStatuses | SupportTicketStatusLookup | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:58` |
| SupportTicketCategories | SupportTicketCategoryLookup | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:59` |
| SupportTicketRecipientTypes | SupportTicketRecipientTypeLookup | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:60` |
| EventParticipantSmsRequests | EventParticipantSmsRequest | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:61` |
| SmsQueueItems | SmsQueueItem | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:62` |
| RefreshTokens | RefreshToken | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:63` |
| AuditLogs | AuditLog | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:64` |
| PermissionActions | PermissionAction | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:65` |
| RoleOperationPermissions | RoleOperationPermission | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:66` |
| UserOperationPermissionOverrides | UserOperationPermissionOverride | `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs:67` |

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
