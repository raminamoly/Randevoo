# Business Invariants

## Purpose
List important invariants visible from schema and handlers.

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

- Unique mobile number and email for users.
- Unique role/action permission combinations.
- Unique event conversation and event like relationships where configured.
- Restrict deletes for financial/payment links to preserve accounting integrity.
- Cascade deletes for ownership-contained records such as profile and profile images where configured.
- Event capacity, gender ticket pricing, discounts, payment collection method, and status transitions are central event invariants.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
