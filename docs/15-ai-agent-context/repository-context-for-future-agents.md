# Repository Context For Future Agents

## Purpose
Compact context for future automated work.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `Randevoo.sln`

## Snapshot
- Projects: Randevoo.AdminPanel, Randevoo.Application, Randevoo.Domain, Randevoo.Infrastructure, Randevoo.WebApi, Randevoo.Tests.Integration, Randevoo.Tests.Unit.
- Entities: AuditLog, BalanceAccount, BalanceTransaction, BalanceTransactionTypeLookup, City, Country, CurrencyExchangeRate, CurrencyLookup, DatingEvent, EducationLevelLookup, EventChatBlock, EventChatMessage, EventConversation, EventDiscountCode, EventDiscountTypeLookup, EventFaq, EventLike, EventModeLookup, EventParticipantSmsRequest, EventPlannerProfile, EventReviewStatusLookup, EventSurveyRating, EventSurveyResponse, EventTag, EventTicket, EventType, GenderLookup, Interest, ManualPaymentReceipt, ModerationReport, OnlineEventPlatform, OnlinePayment, PermissionAction, PlannerBankAccount, PlannerWithdrawalRequest, RefreshToken, RoleOperationPermission, SmsQueueItem, SupportTicket, SupportTicketAssignmentCursor, SupportTicketAttachment, SupportTicketCategoryLookup, SupportTicketHistoryEntry, SupportTicketMessage, SupportTicketRecipientTypeLookup, SupportTicketStatusLookup, Tag, TicketOrder, User, UserOperationPermissionOverride, UserProfile, UserProfileImage, UserRoleLookup, ZodiacSignLookup.
- Endpoints: 54 detected.
- Admin pages: 63 detected.
- Migrations: 39 detected.

## High-risk areas
Authentication, authorization, payment/finance, privacy, moderation, file uploads, and migrations.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
