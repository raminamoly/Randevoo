# Glossary

## Purpose
Define domain vocabulary from entity and feature names.

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

- AuditLog: Audit Log domain concept represented by `src/Randevoo.Domain/Entities/AuditLog.cs`.
- BalanceAccount: Balance Account domain concept represented by `src/Randevoo.Domain/Entities/BalanceAccount.cs`.
- BalanceTransaction: Balance Transaction domain concept represented by `src/Randevoo.Domain/Entities/BalanceTransaction.cs`.
- BalanceTransactionTypeLookup: Balance Transaction Type Lookup domain concept represented by `src/Randevoo.Domain/Entities/BalanceTransactionTypeLookup.cs`.
- City: City domain concept represented by `src/Randevoo.Domain/Entities/City.cs`.
- Country: Country domain concept represented by `src/Randevoo.Domain/Entities/Country.cs`.
- CurrencyExchangeRate: Currency Exchange Rate domain concept represented by `src/Randevoo.Domain/Entities/CurrencyExchangeRate.cs`.
- CurrencyLookup: Currency Lookup domain concept represented by `src/Randevoo.Domain/Entities/CurrencyLookup.cs`.
- DatingEvent: Dating Event domain concept represented by `src/Randevoo.Domain/Entities/DatingEvent.cs`.
- EducationLevelLookup: Education Level Lookup domain concept represented by `src/Randevoo.Domain/Entities/EducationLevelLookup.cs`.
- EventChatBlock: Event Chat Block domain concept represented by `src/Randevoo.Domain/Entities/EventChatBlock.cs`.
- EventChatMessage: Event Chat Message domain concept represented by `src/Randevoo.Domain/Entities/EventChatMessage.cs`.
- EventConversation: Event Conversation domain concept represented by `src/Randevoo.Domain/Entities/EventConversation.cs`.
- EventDiscountCode: Event Discount Code domain concept represented by `src/Randevoo.Domain/Entities/EventDiscountCode.cs`.
- EventDiscountTypeLookup: Event Discount Type Lookup domain concept represented by `src/Randevoo.Domain/Entities/EventDiscountTypeLookup.cs`.
- EventFaq: Event Faq domain concept represented by `src/Randevoo.Domain/Entities/EventFaq.cs`.
- EventLike: Event Like domain concept represented by `src/Randevoo.Domain/Entities/EventLike.cs`.
- EventModeLookup: Event Mode Lookup domain concept represented by `src/Randevoo.Domain/Entities/EventModeLookup.cs`.
- EventParticipantSmsRequest: Event Participant Sms Request domain concept represented by `src/Randevoo.Domain/Entities/EventParticipantSmsRequest.cs`.
- EventPlannerProfile: Event Planner Profile domain concept represented by `src/Randevoo.Domain/Entities/EventPlannerProfile.cs`.
- EventReviewStatusLookup: Event Review Status Lookup domain concept represented by `src/Randevoo.Domain/Entities/EventReviewStatusLookup.cs`.
- EventSurveyRating: Event Survey Rating domain concept represented by `src/Randevoo.Domain/Entities/EventSurveyRating.cs`.
- EventSurveyResponse: Event Survey Response domain concept represented by `src/Randevoo.Domain/Entities/EventSurveyResponse.cs`.
- EventTag: Event Tag domain concept represented by `src/Randevoo.Domain/Entities/EventTag.cs`.
- EventTicket: Event Ticket domain concept represented by `src/Randevoo.Domain/Entities/EventTicket.cs`.
- EventType: Event Type domain concept represented by `src/Randevoo.Domain/Entities/EventType.cs`.
- GenderLookup: Gender Lookup domain concept represented by `src/Randevoo.Domain/Entities/GenderLookup.cs`.
- Interest: Interest domain concept represented by `src/Randevoo.Domain/Entities/Interest.cs`.
- ManualPaymentReceipt: Manual Payment Receipt domain concept represented by `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs`.
- ModerationReport: Moderation Report domain concept represented by `src/Randevoo.Domain/Entities/ModerationReport.cs`.
- OnlineEventPlatform: Online Event Platform domain concept represented by `src/Randevoo.Domain/Entities/OnlineEventPlatform.cs`.
- OnlinePayment: Online Payment domain concept represented by `src/Randevoo.Domain/Entities/OnlinePayment.cs`.
- PermissionAction: Permission Action domain concept represented by `src/Randevoo.Domain/Entities/PermissionAction.cs`.
- PlannerBankAccount: Planner Bank Account domain concept represented by `src/Randevoo.Domain/Entities/PlannerBankAccount.cs`.
- PlannerWithdrawalRequest: Planner Withdrawal Request domain concept represented by `src/Randevoo.Domain/Entities/PlannerWithdrawalRequest.cs`.
- RefreshToken: Refresh Token domain concept represented by `src/Randevoo.Domain/Entities/RefreshToken.cs`.
- RoleOperationPermission: Role Operation Permission domain concept represented by `src/Randevoo.Domain/Entities/RoleOperationPermission.cs`.
- SmsQueueItem: Sms Queue Item domain concept represented by `src/Randevoo.Domain/Entities/SmsQueueItem.cs`.
- SupportTicket: Support Ticket domain concept represented by `src/Randevoo.Domain/Entities/SupportTicket.cs`.
- SupportTicketAssignmentCursor: Support Ticket Assignment Cursor domain concept represented by `src/Randevoo.Domain/Entities/SupportTicketAssignmentCursor.cs`.
- SupportTicketAttachment: Support Ticket Attachment domain concept represented by `src/Randevoo.Domain/Entities/SupportTicketAttachment.cs`.
- SupportTicketCategoryLookup: Support Ticket Category Lookup domain concept represented by `src/Randevoo.Domain/Entities/SupportTicketCategoryLookup.cs`.
- SupportTicketHistoryEntry: Support Ticket History Entry domain concept represented by `src/Randevoo.Domain/Entities/SupportTicketHistoryEntry.cs`.
- SupportTicketMessage: Support Ticket Message domain concept represented by `src/Randevoo.Domain/Entities/SupportTicketMessage.cs`.
- SupportTicketRecipientTypeLookup: Support Ticket Recipient Type Lookup domain concept represented by `src/Randevoo.Domain/Entities/SupportTicketRecipientTypeLookup.cs`.
- SupportTicketStatusLookup: Support Ticket Status Lookup domain concept represented by `src/Randevoo.Domain/Entities/SupportTicketStatusLookup.cs`.
- Tag: Tag domain concept represented by `src/Randevoo.Domain/Entities/Tag.cs`.
- TicketOrder: Ticket Order domain concept represented by `src/Randevoo.Domain/Entities/TicketOrder.cs`.
- User: User domain concept represented by `src/Randevoo.Domain/Entities/User.cs`.
- UserOperationPermissionOverride: User Operation Permission Override domain concept represented by `src/Randevoo.Domain/Entities/UserOperationPermissionOverride.cs`.
- UserProfile: User Profile domain concept represented by `src/Randevoo.Domain/Entities/UserProfile.cs`.
- UserProfileImage: User Profile Image domain concept represented by `src/Randevoo.Domain/Entities/UserProfileImage.cs`.
- UserRoleLookup: User Role Lookup domain concept represented by `src/Randevoo.Domain/Entities/UserRoleLookup.cs`.
- ZodiacSignLookup: Zodiac Sign Lookup domain concept represented by `src/Randevoo.Domain/Entities/ZodiacSignLookup.cs`.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
