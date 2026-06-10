# Entity Catalog

## Purpose
Catalog all detected domain entities.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.Domain/Entities/AuditLog.cs`
- `src/Randevoo.Domain/Entities/BalanceAccount.cs`
- `src/Randevoo.Domain/Entities/BalanceTransaction.cs`
- `src/Randevoo.Domain/Entities/BalanceTransactionTypeLookup.cs`
- `src/Randevoo.Domain/Entities/City.cs`
- `src/Randevoo.Domain/Entities/Country.cs`
- `src/Randevoo.Domain/Entities/CurrencyExchangeRate.cs`
- `src/Randevoo.Domain/Entities/CurrencyLookup.cs`
- `src/Randevoo.Domain/Entities/DatingEvent.cs`
- `src/Randevoo.Domain/Entities/EducationLevelLookup.cs`
- `src/Randevoo.Domain/Entities/EventChatBlock.cs`
- `src/Randevoo.Domain/Entities/EventChatMessage.cs`
- `src/Randevoo.Domain/Entities/EventConversation.cs`
- `src/Randevoo.Domain/Entities/EventDiscountCode.cs`
- `src/Randevoo.Domain/Entities/EventDiscountTypeLookup.cs`
- `src/Randevoo.Domain/Entities/EventFaq.cs`
- `src/Randevoo.Domain/Entities/EventLike.cs`
- `src/Randevoo.Domain/Entities/EventModeLookup.cs`
- `src/Randevoo.Domain/Entities/EventParticipantSmsRequest.cs`
- `src/Randevoo.Domain/Entities/EventPlannerProfile.cs`
- `src/Randevoo.Domain/Entities/EventReviewStatusLookup.cs`
- `src/Randevoo.Domain/Entities/EventSurveyRating.cs`
- `src/Randevoo.Domain/Entities/EventSurveyResponse.cs`
- `src/Randevoo.Domain/Entities/EventTag.cs`
- `src/Randevoo.Domain/Entities/EventTicket.cs`
- `src/Randevoo.Domain/Entities/EventType.cs`
- `src/Randevoo.Domain/Entities/GenderLookup.cs`
- `src/Randevoo.Domain/Entities/Interest.cs`
- `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs`
- `src/Randevoo.Domain/Entities/ModerationReport.cs`
- `src/Randevoo.Domain/Entities/OnlineEventPlatform.cs`
- `src/Randevoo.Domain/Entities/OnlinePayment.cs`
- `src/Randevoo.Domain/Entities/PermissionAction.cs`
- `src/Randevoo.Domain/Entities/PlannerBankAccount.cs`
- `src/Randevoo.Domain/Entities/PlannerWithdrawalRequest.cs`
- `src/Randevoo.Domain/Entities/RefreshToken.cs`
- `src/Randevoo.Domain/Entities/RoleOperationPermission.cs`
- `src/Randevoo.Domain/Entities/SmsQueueItem.cs`
- `src/Randevoo.Domain/Entities/SupportTicket.cs`
- `src/Randevoo.Domain/Entities/SupportTicketAssignmentCursor.cs`
- `src/Randevoo.Domain/Entities/SupportTicketAttachment.cs`
- `src/Randevoo.Domain/Entities/SupportTicketCategoryLookup.cs`
- `src/Randevoo.Domain/Entities/SupportTicketHistoryEntry.cs`
- `src/Randevoo.Domain/Entities/SupportTicketMessage.cs`
- `src/Randevoo.Domain/Entities/SupportTicketRecipientTypeLookup.cs`
- `src/Randevoo.Domain/Entities/SupportTicketStatusLookup.cs`
- `src/Randevoo.Domain/Entities/Tag.cs`
- `src/Randevoo.Domain/Entities/TicketOrder.cs`
- `src/Randevoo.Domain/Entities/User.cs`
- `src/Randevoo.Domain/Entities/UserOperationPermissionOverride.cs`
- `src/Randevoo.Domain/Entities/UserProfile.cs`
- `src/Randevoo.Domain/Entities/UserProfileImage.cs`
- `src/Randevoo.Domain/Entities/UserRoleLookup.cs`
- `src/Randevoo.Domain/Entities/ZodiacSignLookup.cs`

| Entity | DbSet/Table | Property count | Source |
| --- | --- | ---: | --- |
| AuditLog | AuditLogs | 18 | `src/Randevoo.Domain/Entities/AuditLog.cs` |
| BalanceAccount | BalanceAccounts | 4 | `src/Randevoo.Domain/Entities/BalanceAccount.cs` |
| BalanceTransaction | BalanceTransactions | 19 | `src/Randevoo.Domain/Entities/BalanceTransaction.cs` |
| BalanceTransactionTypeLookup | BalanceTransactionTypes | 4 | `src/Randevoo.Domain/Entities/BalanceTransactionTypeLookup.cs` |
| City | Cities | 7 | `src/Randevoo.Domain/Entities/City.cs` |
| Country | Countries | 4 | `src/Randevoo.Domain/Entities/Country.cs` |
| CurrencyExchangeRate | CurrencyExchangeRates | 7 | `src/Randevoo.Domain/Entities/CurrencyExchangeRate.cs` |
| CurrencyLookup | Currencies | 6 | `src/Randevoo.Domain/Entities/CurrencyLookup.cs` |
| DatingEvent | DatingEvents | 42 | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| EducationLevelLookup | EducationLevels | 4 | `src/Randevoo.Domain/Entities/EducationLevelLookup.cs` |
| EventChatBlock | EventChatBlocks | 7 | `src/Randevoo.Domain/Entities/EventChatBlock.cs` |
| EventChatMessage | EventChatMessages | 5 | `src/Randevoo.Domain/Entities/EventChatMessage.cs` |
| EventConversation | EventConversations | 10 | `src/Randevoo.Domain/Entities/EventConversation.cs` |
| EventDiscountCode | EventDiscountCodes | 14 | `src/Randevoo.Domain/Entities/EventDiscountCode.cs` |
| EventDiscountTypeLookup | EventDiscountTypes | 4 | `src/Randevoo.Domain/Entities/EventDiscountTypeLookup.cs` |
| EventFaq | EventFaqs | 5 | `src/Randevoo.Domain/Entities/EventFaq.cs` |
| EventLike | EventLikes | 8 | `src/Randevoo.Domain/Entities/EventLike.cs` |
| EventModeLookup | EventModes | 4 | `src/Randevoo.Domain/Entities/EventModeLookup.cs` |
| EventParticipantSmsRequest | EventParticipantSmsRequests | 13 | `src/Randevoo.Domain/Entities/EventParticipantSmsRequest.cs` |
| EventPlannerProfile | EventPlannerProfiles | 23 | `src/Randevoo.Domain/Entities/EventPlannerProfile.cs` |
| EventReviewStatusLookup | EventReviewStatuses | 4 | `src/Randevoo.Domain/Entities/EventReviewStatusLookup.cs` |
| EventSurveyRating | EventSurveyRatings | 4 | `src/Randevoo.Domain/Entities/EventSurveyRating.cs` |
| EventSurveyResponse | EventSurveyResponses | 5 | `src/Randevoo.Domain/Entities/EventSurveyResponse.cs` |
| EventTag | EventTags | 4 | `src/Randevoo.Domain/Entities/EventTag.cs` |
| EventTicket | EventTickets | 25 | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| EventType | EventTypes | 3 | `src/Randevoo.Domain/Entities/EventType.cs` |
| GenderLookup | Genders | 3 | `src/Randevoo.Domain/Entities/GenderLookup.cs` |
| Interest | Interests | 4 | `src/Randevoo.Domain/Entities/Interest.cs` |
| ManualPaymentReceipt | ManualPaymentReceipts | 34 | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| ModerationReport | ModerationReports | 15 | `src/Randevoo.Domain/Entities/ModerationReport.cs` |
| OnlineEventPlatform | OnlineEventPlatforms | 3 | `src/Randevoo.Domain/Entities/OnlineEventPlatform.cs` |
| OnlinePayment | OnlinePayments | 22 | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |
| PermissionAction | PermissionActions | 6 | `src/Randevoo.Domain/Entities/PermissionAction.cs` |
| PlannerBankAccount | PlannerBankAccounts | 14 | `src/Randevoo.Domain/Entities/PlannerBankAccount.cs` |
| PlannerWithdrawalRequest | PlannerWithdrawalRequests | 15 | `src/Randevoo.Domain/Entities/PlannerWithdrawalRequest.cs` |
| RefreshToken | RefreshTokens | 6 | `src/Randevoo.Domain/Entities/RefreshToken.cs` |
| RoleOperationPermission | RoleOperationPermissions | 4 | `src/Randevoo.Domain/Entities/RoleOperationPermission.cs` |
| SmsQueueItem | SmsQueueItems | 13 | `src/Randevoo.Domain/Entities/SmsQueueItem.cs` |
| SupportTicket | SupportTickets | 19 | `src/Randevoo.Domain/Entities/SupportTicket.cs` |
| SupportTicketAssignmentCursor | SupportTicketAssignmentCursors | 2 | `src/Randevoo.Domain/Entities/SupportTicketAssignmentCursor.cs` |
| SupportTicketAttachment | SupportTicketAttachments | 6 | `src/Randevoo.Domain/Entities/SupportTicketAttachment.cs` |
| SupportTicketCategoryLookup | SupportTicketCategories | 4 | `src/Randevoo.Domain/Entities/SupportTicketCategoryLookup.cs` |
| SupportTicketHistoryEntry | SupportTicketHistoryEntries | 10 | `src/Randevoo.Domain/Entities/SupportTicketHistoryEntry.cs` |
| SupportTicketMessage | SupportTicketMessages | 8 | `src/Randevoo.Domain/Entities/SupportTicketMessage.cs` |
| SupportTicketRecipientTypeLookup | SupportTicketRecipientTypes | 4 | `src/Randevoo.Domain/Entities/SupportTicketRecipientTypeLookup.cs` |
| SupportTicketStatusLookup | SupportTicketStatuses | 4 | `src/Randevoo.Domain/Entities/SupportTicketStatusLookup.cs` |
| Tag | Tags | 2 | `src/Randevoo.Domain/Entities/Tag.cs` |
| TicketOrder | TicketOrders | 31 | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| User | Users | 15 | `src/Randevoo.Domain/Entities/User.cs` |
| UserOperationPermissionOverride | UserOperationPermissionOverrides | 7 | `src/Randevoo.Domain/Entities/UserOperationPermissionOverride.cs` |
| UserProfile | UserProfiles | 21 | `src/Randevoo.Domain/Entities/UserProfile.cs` |
| UserProfileImage | UserProfileImages | 5 | `src/Randevoo.Domain/Entities/UserProfileImage.cs` |
| UserRoleLookup | UserRoles | 4 | `src/Randevoo.Domain/Entities/UserRoleLookup.cs` |
| ZodiacSignLookup | ZodiacSigns | 4 | `src/Randevoo.Domain/Entities/ZodiacSignLookup.cs` |

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
