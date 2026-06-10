# Business Rules

## Purpose
Capture business rules visible in domain handlers/entities.

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

- User mobile number and email uniqueness are configured in DbContext.
- Event lifecycle supports open, close, cancel, review/operational status concepts.
- Event planners and admins have separate policy gates in API/AdminPanel.
- Ticketing uses ticket orders, event tickets, online payments, manual receipts, currencies, and balance transactions.
- Support tickets have categories, statuses, messages, attachments, history, assignment cursor, and recipient type lookups.
- Moderation reports track reporter, reported user/conversation/event context, reason, status, and review data.

## Gaps or uncertainties
- Some invariants are implemented in handlers rather than aggregate methods; verify per feature before centralizing rules.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
