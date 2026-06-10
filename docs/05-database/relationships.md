# Relationships

## Purpose
Summarize detected relationships from foreign key naming and DbContext.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

| Entity | Foreign key candidates | Source |
| --- | --- | --- |
| AuditLog | ActorUserId, TargetId, CorrelationId | `src/Randevoo.Domain/Entities/AuditLog.cs` |
| BalanceAccount | UserId | `src/Randevoo.Domain/Entities/BalanceAccount.cs` |
| BalanceTransaction | BalanceAccountId, UserId, ExchangeRateId, DatingEventId, TicketOrderId, ReferenceId, CreatedByUserId | `src/Randevoo.Domain/Entities/BalanceTransaction.cs` |
| BalanceTransactionTypeLookup | None detected | `src/Randevoo.Domain/Entities/BalanceTransactionTypeLookup.cs` |
| City | CountryId | `src/Randevoo.Domain/Entities/City.cs` |
| Country | None detected | `src/Randevoo.Domain/Entities/Country.cs` |
| CurrencyExchangeRate | CreatedByUserId | `src/Randevoo.Domain/Entities/CurrencyExchangeRate.cs` |
| CurrencyLookup | None detected | `src/Randevoo.Domain/Entities/CurrencyLookup.cs` |
| DatingEvent | EventTypeId, EventModeId, OnlineEventPlatformId, CountryId, CityId, EventPlannerUserId, MinimumEducationLevelId | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| EducationLevelLookup | None detected | `src/Randevoo.Domain/Entities/EducationLevelLookup.cs` |
| EventChatBlock | EventConversationId, BlockerUserId, BlockedUserId | `src/Randevoo.Domain/Entities/EventChatBlock.cs` |
| EventChatMessage | EventConversationId, SenderUserId | `src/Randevoo.Domain/Entities/EventChatMessage.cs` |
| EventConversation | DatingEventId, StarterUserId, ParticipantUserId, DisabledByUserId | `src/Randevoo.Domain/Entities/EventConversation.cs` |
| EventDiscountCode | DatingEventId | `src/Randevoo.Domain/Entities/EventDiscountCode.cs` |
| EventDiscountTypeLookup | None detected | `src/Randevoo.Domain/Entities/EventDiscountTypeLookup.cs` |
| EventFaq | DatingEventId | `src/Randevoo.Domain/Entities/EventFaq.cs` |
| EventLike | DatingEventId, FromUserId, ToUserId | `src/Randevoo.Domain/Entities/EventLike.cs` |
| EventModeLookup | None detected | `src/Randevoo.Domain/Entities/EventModeLookup.cs` |
| EventParticipantSmsRequest | DatingEventId, RequestedByUserId, ReviewedByAdminUserId | `src/Randevoo.Domain/Entities/EventParticipantSmsRequest.cs` |
| EventPlannerProfile | UserId, PendingReviewedByAdminUserId | `src/Randevoo.Domain/Entities/EventPlannerProfile.cs` |
| EventReviewStatusLookup | None detected | `src/Randevoo.Domain/Entities/EventReviewStatusLookup.cs` |
| EventSurveyRating | EventSurveyResponseId | `src/Randevoo.Domain/Entities/EventSurveyRating.cs` |
| EventSurveyResponse | DatingEventId, UserId | `src/Randevoo.Domain/Entities/EventSurveyResponse.cs` |
| EventTag | DatingEventId, TagId | `src/Randevoo.Domain/Entities/EventTag.cs` |
| EventTicket | TicketOrderId, DatingEventId, UserId, ExchangeRateId, EventDiscountCodeId, RemovedByUserId | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| EventType | None detected | `src/Randevoo.Domain/Entities/EventType.cs` |
| GenderLookup | None detected | `src/Randevoo.Domain/Entities/GenderLookup.cs` |
| Interest | None detected | `src/Randevoo.Domain/Entities/Interest.cs` |
| ManualPaymentReceipt | DatingEventId, ParticipantUserId, PlannerUserId, EventTicketId, TicketOrderId, EventDiscountCodeId, ExchangeRateId, ReviewedByUserId | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| ModerationReport | ReporterUserId, ReportedUserId, DatingEventId, EventConversationId, ReviewedByAdminUserId | `src/Randevoo.Domain/Entities/ModerationReport.cs` |
| OnlineEventPlatform | None detected | `src/Randevoo.Domain/Entities/OnlineEventPlatform.cs` |
| OnlinePayment | UserId, DatingEventId, EventTicketId, TicketOrderId, BalanceTransactionId, ExchangeRateId | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |
| PermissionAction | None detected | `src/Randevoo.Domain/Entities/PermissionAction.cs` |
| PlannerBankAccount | UserId | `src/Randevoo.Domain/Entities/PlannerBankAccount.cs` |
| PlannerWithdrawalRequest | UserId, ExchangeRateId, ReviewedByAdminUserId | `src/Randevoo.Domain/Entities/PlannerWithdrawalRequest.cs` |
| RefreshToken | UserId | `src/Randevoo.Domain/Entities/RefreshToken.cs` |
| RoleOperationPermission | None detected | `src/Randevoo.Domain/Entities/RoleOperationPermission.cs` |
| SmsQueueItem | EventParticipantSmsRequestId, DatingEventId, RecipientUserId | `src/Randevoo.Domain/Entities/SmsQueueItem.cs` |
| SupportTicket | TicketTypeId, TicketStatusId, TicketRecipientTypeId, SubmitterUserId, AssignedSupportUserId, DatingEventId, RecipientPlannerUserId | `src/Randevoo.Domain/Entities/SupportTicket.cs` |
| SupportTicketAssignmentCursor | LastAssignedUserId | `src/Randevoo.Domain/Entities/SupportTicketAssignmentCursor.cs` |
| SupportTicketAttachment | SupportTicketMessageId | `src/Randevoo.Domain/Entities/SupportTicketAttachment.cs` |
| SupportTicketCategoryLookup | None detected | `src/Randevoo.Domain/Entities/SupportTicketCategoryLookup.cs` |
| SupportTicketHistoryEntry | SupportTicketId, ActorUserId, OldAssigneeUserId, NewAssigneeUserId | `src/Randevoo.Domain/Entities/SupportTicketHistoryEntry.cs` |
| SupportTicketMessage | SupportTicketId, SenderUserId, RepresentedUserId | `src/Randevoo.Domain/Entities/SupportTicketMessage.cs` |
| SupportTicketRecipientTypeLookup | None detected | `src/Randevoo.Domain/Entities/SupportTicketRecipientTypeLookup.cs` |
| SupportTicketStatusLookup | None detected | `src/Randevoo.Domain/Entities/SupportTicketStatusLookup.cs` |
| Tag | None detected | `src/Randevoo.Domain/Entities/Tag.cs` |
| TicketOrder | DatingEventId, BuyerUserId, EventDiscountCodeId, ExchangeRateId, ApprovedByUserId | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| User | None detected | `src/Randevoo.Domain/Entities/User.cs` |
| UserOperationPermissionOverride | UserId | `src/Randevoo.Domain/Entities/UserOperationPermissionOverride.cs` |
| UserProfile | UserId, GenderId, ZodiacSignId, EducationLevelId, CountryId, CityId | `src/Randevoo.Domain/Entities/UserProfile.cs` |
| UserProfileImage | UserProfileId | `src/Randevoo.Domain/Entities/UserProfileImage.cs` |
| UserRoleLookup | None detected | `src/Randevoo.Domain/Entities/UserRoleLookup.cs` |
| ZodiacSignLookup | None detected | `src/Randevoo.Domain/Entities/ZodiacSignLookup.cs` |

## Delete behavior counts
- Cascade: 25
- Restrict: 70
- NoAction: 1

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
