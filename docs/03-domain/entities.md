# Domain Entities

## User

Purpose: account and authentication root for all roles.

Properties: `MobileNumber`, `Email`, `IsEmailConfirmed`, `PendingEmail`, mobile login code hash/expiry, SMS request window/count, failed login attempt count, lockout time, email token hash/expiry, `Role`, `IsActive`, optional `Profile`.

Relationships: one optional `UserProfile`; one optional `EventPlannerProfile`; one optional `BalanceAccount`; many `RefreshToken`; owns/participates in events, tickets, chats, reports, surveys by foreign keys.

Lifecycle: created by mobile login request; login code requested/verified with throttling and lockout; email confirmation requested/confirmed; role can change; can become event planner; can deactivate.

Aggregate ownership: aggregate root.

## RefreshToken

Purpose: persistent session token used to renew short-lived JWT access tokens.

Properties: `UserId`, hashed token, expiry time, optional revocation time, optional replacement token hash.

Relationships: belongs to `User`.

Lifecycle: created after successful mobile-code verification; rotated on refresh by revoking the old token and storing the replacement hash; revoked on logout; expires after configured lifetime.

Aggregate ownership: aggregate root.

## UserProfile

Purpose: public dating profile for an EndUser.

Properties: `UserId`, `DisplayName`, `Gender`, `DateOfBirth`, `Height`, `EducationLevel`, `Smoking`, `Location`, computed `Age`, interests.

Relationships: belongs to `User`; many-to-many with `Interest`.

Lifecycle: created once per user; can update display name, location, height, education level, gender, smoking, interests; supports soft delete.

Aggregate ownership: aggregate root, logically owned by `User`.

## Interest

Purpose: profile interest/tag with usage tracking.

Properties: `Name`, `Category`, `UsageCount`.

Relationships: many-to-many with `UserProfile`.

Lifecycle: created independently; usage increments/decrements when profiles add/remove it.

Aggregate ownership: standalone entity, not marked aggregate root.

## EventPlannerProfile

Purpose: planner-facing profile and quality metrics.

Properties: `UserId`, `Title`, `PictureUrl`, `Resume`, `AverageRating`, `TotalSurveyCount`, `HostedEventCount`, `CancelledEventCount`, `CompletedEventCount`.

Relationships: one-to-one with `User`.

Lifecycle: upserted by authenticated user; creation upgrades user to `EventPlanner` unless user is `Admin`; survey submission updates quality metrics.

Aggregate ownership: aggregate root.

## DatingEvent

Purpose: event listing and ticket-sale aggregate.

Properties: title, `Location`, address, start/end, `EventTypeId`, male/female age ranges, sale/cancel flags, planner user id, commission percent, gender capacities, chat limit, ticket price, images, HTML description.

Relationships: owned by event planner user; references `EventType`; has many `EventTicket`; related to conversations, surveys, reports by foreign keys.

Lifecycle: created closed for sale; opened/closed; location can change; commission can change by admin; can be cancelled and tickets refunded; sells tickets with capacity/age/profile checks.

Aggregate ownership: aggregate root.

## EventTicket

Purpose: purchase record and access token for a user/event.

Properties: `DatingEventId`, `UserId`, gender snapshot, price snapshot, refund flag, removal flag, removal reason, removed-by user, removed-at time, computed `IsValidForEventAccess`.

Relationships: belongs to `DatingEvent` and `User`.

Lifecycle: created by ticket purchase; may be refunded by event cancellation; may be removed/refunded in emergency.

Aggregate ownership: child of `DatingEvent`, persisted through repository for participant workflows.

## BalanceAccount

Purpose: user wallet/balance aggregate.

Properties: `UserId`, `Balance`, transactions.

Relationships: belongs to `User`; has many `BalanceTransaction`.

Lifecycle: created lazily for a user; credited/debited; enforces non-negative balance.

Aggregate ownership: aggregate root.

## BalanceTransaction

Purpose: immutable-ish balance ledger entry.

Properties: account id, user id, amount, transaction type, description, optional dating event id, reference type/id, optional created-by user id.

Relationships: child of `BalanceAccount`.

Lifecycle: created internally by account credit/debit.

Aggregate ownership: child of `BalanceAccount`.

## EventConversation

Purpose: event-scoped chat connection between two participants.

Properties: event id, starter user id, participant user id, disabled flag/reason/by/time, messages, blocks.

Relationships: belongs to `DatingEvent`; references two users; has messages and blocks.

Lifecycle: created after event start when both users have valid tickets and starter is within chat limit; can send messages; users can block each other; can be disabled by emergency participant removal.

Aggregate ownership: aggregate root.

## EventChatMessage

Purpose: message in an event conversation.

Properties: conversation id, sender user id, body.

Relationships: child of `EventConversation`; references sender user.

Lifecycle: created through `EventConversation.SendMessage`.

Aggregate ownership: child of `EventConversation`.

## EventChatBlock

Purpose: active block between conversation participants.

Properties: conversation id, blocker user id, blocked user id, active flag.

Relationships: child of `EventConversation`; references users.

Lifecycle: created through `EventConversation.Block`; no unblock use case currently exists.

Aggregate ownership: child of `EventConversation`.

## EventSurveyResponse

Purpose: post-event survey for calculating planner quality.

Properties: event id, user id, optional comment, ratings.

Relationships: belongs to `DatingEvent` and `User`; has many `EventSurveyRating`.

Lifecycle: submitted after event end by a valid non-refunded participant; can be updated by submitting again.

Aggregate ownership: aggregate root.

## EventSurveyRating

Purpose: score for one survey factor.

Properties: survey response id, `SurveyFactor`, score 1-5.

Relationships: child of `EventSurveyResponse`.

Lifecycle: created/replaced when survey ratings are updated.

Aggregate ownership: child of `EventSurveyResponse`.

## EventType

Purpose: lookup table for event type suggestions/admin management.

Properties: `Name`, optional `Description`, `IsActive`.

Relationships: referenced by `DatingEvent`.

Lifecycle: seeded by EF migration; admins can create/update/deactivate.

Aggregate ownership: aggregate root.

## ModerationReport

Purpose: report/audit record for user safety issues and emergency removals.

Properties: reporter user id, reported user id, optional event id, optional conversation id, reason, description, status, admin note, reviewer id, review time.

Relationships: references reporter, reported user, optional event, optional conversation, optional reviewer.

Lifecycle: created by users or emergency removal; admin can review, dismiss, or mark action taken.

Aggregate ownership: aggregate root.

## Value Objects

- `Location`: country, city, optional region, coordinates.
- `Coordinates`: latitude/longitude and distance calculation.
- `AgeRange`: min/max age and range check.
- `Height`: centimeters.

## TODO / Assumption Required

- `Interest` has no API endpoints in current implementation.
