# Domain Layer

## Purpose
Document Domain responsibilities and model inventory.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.Domain`

## Entities
- AuditLog: `src/Randevoo.Domain/Entities/AuditLog.cs`
- BalanceAccount: `src/Randevoo.Domain/Entities/BalanceAccount.cs`
- BalanceTransaction: `src/Randevoo.Domain/Entities/BalanceTransaction.cs`
- BalanceTransactionTypeLookup: `src/Randevoo.Domain/Entities/BalanceTransactionTypeLookup.cs`
- City: `src/Randevoo.Domain/Entities/City.cs`
- Country: `src/Randevoo.Domain/Entities/Country.cs`
- CurrencyExchangeRate: `src/Randevoo.Domain/Entities/CurrencyExchangeRate.cs`
- CurrencyLookup: `src/Randevoo.Domain/Entities/CurrencyLookup.cs`
- DatingEvent: `src/Randevoo.Domain/Entities/DatingEvent.cs`
- EducationLevelLookup: `src/Randevoo.Domain/Entities/EducationLevelLookup.cs`
- EventChatBlock: `src/Randevoo.Domain/Entities/EventChatBlock.cs`
- EventChatMessage: `src/Randevoo.Domain/Entities/EventChatMessage.cs`
- EventConversation: `src/Randevoo.Domain/Entities/EventConversation.cs`
- EventDiscountCode: `src/Randevoo.Domain/Entities/EventDiscountCode.cs`
- EventDiscountTypeLookup: `src/Randevoo.Domain/Entities/EventDiscountTypeLookup.cs`
- EventFaq: `src/Randevoo.Domain/Entities/EventFaq.cs`
- EventLike: `src/Randevoo.Domain/Entities/EventLike.cs`
- EventModeLookup: `src/Randevoo.Domain/Entities/EventModeLookup.cs`
- EventParticipantSmsRequest: `src/Randevoo.Domain/Entities/EventParticipantSmsRequest.cs`
- EventPlannerProfile: `src/Randevoo.Domain/Entities/EventPlannerProfile.cs`
- EventReviewStatusLookup: `src/Randevoo.Domain/Entities/EventReviewStatusLookup.cs`
- EventSurveyRating: `src/Randevoo.Domain/Entities/EventSurveyRating.cs`
- EventSurveyResponse: `src/Randevoo.Domain/Entities/EventSurveyResponse.cs`
- EventTag: `src/Randevoo.Domain/Entities/EventTag.cs`
- EventTicket: `src/Randevoo.Domain/Entities/EventTicket.cs`
- EventType: `src/Randevoo.Domain/Entities/EventType.cs`
- GenderLookup: `src/Randevoo.Domain/Entities/GenderLookup.cs`
- Interest: `src/Randevoo.Domain/Entities/Interest.cs`
- ManualPaymentReceipt: `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs`
- ModerationReport: `src/Randevoo.Domain/Entities/ModerationReport.cs`
- OnlineEventPlatform: `src/Randevoo.Domain/Entities/OnlineEventPlatform.cs`
- OnlinePayment: `src/Randevoo.Domain/Entities/OnlinePayment.cs`
- PermissionAction: `src/Randevoo.Domain/Entities/PermissionAction.cs`
- PlannerBankAccount: `src/Randevoo.Domain/Entities/PlannerBankAccount.cs`
- PlannerWithdrawalRequest: `src/Randevoo.Domain/Entities/PlannerWithdrawalRequest.cs`
- RefreshToken: `src/Randevoo.Domain/Entities/RefreshToken.cs`
- RoleOperationPermission: `src/Randevoo.Domain/Entities/RoleOperationPermission.cs`
- SmsQueueItem: `src/Randevoo.Domain/Entities/SmsQueueItem.cs`
- SupportTicket: `src/Randevoo.Domain/Entities/SupportTicket.cs`
- SupportTicketAssignmentCursor: `src/Randevoo.Domain/Entities/SupportTicketAssignmentCursor.cs`
- SupportTicketAttachment: `src/Randevoo.Domain/Entities/SupportTicketAttachment.cs`
- SupportTicketCategoryLookup: `src/Randevoo.Domain/Entities/SupportTicketCategoryLookup.cs`
- SupportTicketHistoryEntry: `src/Randevoo.Domain/Entities/SupportTicketHistoryEntry.cs`
- SupportTicketMessage: `src/Randevoo.Domain/Entities/SupportTicketMessage.cs`
- SupportTicketRecipientTypeLookup: `src/Randevoo.Domain/Entities/SupportTicketRecipientTypeLookup.cs`
- SupportTicketStatusLookup: `src/Randevoo.Domain/Entities/SupportTicketStatusLookup.cs`
- Tag: `src/Randevoo.Domain/Entities/Tag.cs`
- TicketOrder: `src/Randevoo.Domain/Entities/TicketOrder.cs`
- User: `src/Randevoo.Domain/Entities/User.cs`
- UserOperationPermissionOverride: `src/Randevoo.Domain/Entities/UserOperationPermissionOverride.cs`
- UserProfile: `src/Randevoo.Domain/Entities/UserProfile.cs`
- UserProfileImage: `src/Randevoo.Domain/Entities/UserProfileImage.cs`
- UserRoleLookup: `src/Randevoo.Domain/Entities/UserRoleLookup.cs`
- ZodiacSignLookup: `src/Randevoo.Domain/Entities/ZodiacSignLookup.cs`

## Value objects
- AgeRange: `src/Randevoo.Domain/ValueObjects/AgeRange.cs`
- BaseValueObject: `src/Randevoo.Domain/ValueObjects/BaseValueObject.cs`
- Coordinates: `src/Randevoo.Domain/ValueObjects/Coordinates.cs`
- Height: `src/Randevoo.Domain/ValueObjects/Hight.cs`
- Location: `src/Randevoo.Domain/ValueObjects/Location.cs`

## Enums
- BalanceTransactionType: `src/Randevoo.Domain/Enums/BalanceTransactionType.cs`
- EducationLevel: `src/Randevoo.Domain/Enums/EducationLevel.cs`
- EventDiscountGenderScope: `src/Randevoo.Domain/Enums/EventDiscountGenderScope.cs`
- EventDiscountType: `src/Randevoo.Domain/Enums/EventDiscountType.cs`
- EventEducationLevelRestriction: `src/Randevoo.Domain/Enums/EventEducationLevelRestriction.cs`
- EventLikeStatus: `src/Randevoo.Domain/Enums/EventLikeStatus.cs`
- EventOperationalStatus: `src/Randevoo.Domain/Enums/EventOperationalStatus.cs`
- EventParticipantSmsRequestStatus: `src/Randevoo.Domain/Enums/EventParticipantSmsRequestStatus.cs`
- EventPaymentCollectionMethod: `src/Randevoo.Domain/Enums/EventPaymentCollectionMethod.cs`
- EventReviewStatus: `src/Randevoo.Domain/Enums/EventReviewStatus.cs`
- Gender: `src/Randevoo.Domain/Enums/Gender.cs`
- ManualPaymentDestinationType: `src/Randevoo.Domain/Enums/ManualPaymentDestinationType.cs`
- ManualPaymentReceiptStatus: `src/Randevoo.Domain/Enums/ManualPaymentReceiptStatus.cs`
- ModerationReportReason: `src/Randevoo.Domain/Enums/ModerationReportReason.cs`
- ModerationReportStatus: `src/Randevoo.Domain/Enums/ModerationReportStatus.cs`
- OnlinePaymentStatus: `src/Randevoo.Domain/Enums/OnlinePaymentStatus.cs`
- PlannerPayoutMethod: `src/Randevoo.Domain/Enums/PlannerPayoutMethod.cs`
- PlannerWithdrawalRequestStatus: `src/Randevoo.Domain/Enums/PlannerWithdrawalRequestStatus.cs`
- SmsQueueItemStatus: `src/Randevoo.Domain/Enums/SmsQueueItemStatus.cs`
- SupportTicketCategory: `src/Randevoo.Domain/Enums/SupportTicketCategory.cs`
- SupportTicketStatus: `src/Randevoo.Domain/Enums/SupportTicketStatus.cs`
- SurveyFactor: `src/Randevoo.Domain/Enums/SurveyFactor.cs`
- TicketOrderPaymentStatus: `src/Randevoo.Domain/Enums/TicketOrderPaymentStatus.cs`
- TicketOrderStatus: `src/Randevoo.Domain/Enums/TicketOrderStatus.cs`
- UserRole: `src/Randevoo.Domain/Enums/UserRole.cs`

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
