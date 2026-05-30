# API Contracts

Base URL in development: `http://localhost:5031`

Authorization policies:

- `RequireAuthorization()`: any authenticated JWT.
- `EndUserOnly`: roles `EndUser` or `Admin`.
- `EventPlannerOnly`: roles `EventPlanner` or `Admin`.
- `AdminOnly`: role `Admin`.

## Authentication

| ID | Method | Route | Request | Response | Auth | Use Case |
|---|---|---|---|---|---|---|
| API-001 | POST | `/api/auth/mobile/request-code` | `RequestMobileCodeRequest { mobileNumber }` | `202 Accepted` | Anonymous | UC-001 |
| API-002 | POST | `/api/auth/mobile/verify-code` | `VerifyMobileCodeRequest { mobileNumber, code }` | `AuthResult { userId, mobileNumber, token }` | Anonymous | UC-002 |
| API-003 | POST | `/api/auth/email/request-confirmation` | `{ email }` | `202 Accepted` | Authenticated | UC-003 |
| API-004 | GET | `/api/auth/email/confirm?userId=&token=` | query | `{ message }` | Anonymous | UC-004 |

Validation/business rules: mobile format 8-20 chars, digits and optional leading plus; login code must match and not expire; email must pass domain email guard; confirmation token must match and not expire.

## Dating Profiles

| ID | Method | Route | Request | Response | Auth | Use Case |
|---|---|---|---|---|---|---|
| API-005 | POST | `/api/dating-profiles` | `CreateDatingProfileRequest` | `DatingProfileDto` | Anonymous in endpoint | UC-005 |
| API-006 | GET | `/api/dating-profiles/{profileId}` | path | `DatingProfileDto` | Anonymous | UC-006 |
| API-007 | GET | `/api/dating-profiles/by-user/{userId}` | path | `DatingProfileDto` | Anonymous | UC-006 |
| API-008 | PUT | `/api/dating-profiles/{profileId}` | `UpdateDatingProfileRequest` | `204` | Anonymous in endpoint | UC-007 |
| API-009 | DELETE | `/api/dating-profiles/{profileId}` | path | `204` | Anonymous in endpoint | UC-008 |

Validation/business rules: user must exist; one profile per user; display name 2-50 chars and unique; minimum age 18; location required; profile soft delete hides future reads.

TODO: dating-profile endpoints currently do not require JWT authorization or ownership checks.

## Event Planner Profiles

| ID | Method | Route | Request | Response | Auth | Use Case |
|---|---|---|---|---|---|---|
| API-010 | PUT | `/api/event-planner-profile/me` | `{ title, pictureUrl, resume }` | `EventPlannerProfileDto` | Authenticated | UC-009 |

Rules: creating profile upgrades user to EventPlanner unless Admin; title 2-100 chars; picture URL max 500; resume 10-4000.

## Dating Events

| ID | Method | Route | Request | Response | Auth | Use Case |
|---|---|---|---|---|---|---|
| API-011 | GET | `/api/dating-events/open?limit=` | query | `DatingEventDto[]` | Anonymous | UC-010 |
| API-012 | POST | `/api/dating-events` | `DatingEventInput` | `DatingEventDto` | EventPlannerOnly | UC-011 |
| API-013 | POST | `/api/dating-events/{eventId}/open` | path | `204` | EventPlannerOnly | UC-012 |
| API-014 | POST | `/api/dating-events/{eventId}/close` | path | `204` | EventPlannerOnly | UC-013 |
| API-015 | POST | `/api/dating-events/{eventId}/cancel` | path | `204` | EventPlannerOnly | UC-014 |
| API-016 | PUT | `/api/dating-events/{eventId}/location` | `{ country, city, region, latitude, longitude, address }` | `204` | EventPlannerOnly | UC-015 |
| API-017 | PUT | `/api/dating-events/{eventId}/commission` | `{ commissionPercent }` | `204` | AdminOnly | UC-016 |
| API-018 | POST | `/api/dating-events/{eventId}/tickets` | none | `{ ticketId }` | EndUserOnly | UC-017 |
| API-019 | POST | `/api/dating-events/{eventId}/send-sms` | `{ message }` | `202` | EventPlannerOnly | UC-018 |

Rules: planner must own event unless Admin; creating event requires planner profile unless Admin; event time end > start; ticket purchase requires profile, open event, valid age/capacity, enough balance; cancellation refunds tickets.

## Event Participants

| ID | Method | Route | Request | Response | Auth | Use Case |
|---|---|---|---|---|---|---|
| API-020 | GET | `/api/event-participants/me/archive` | none | `EventArchiveItemDto[]` | Authenticated | UC-019 |
| API-021 | GET | `/api/event-participants/events/{eventId}/profiles` | path | `DatingProfileDto[]` | Authenticated | UC-020 |
| API-022 | GET | `/api/event-participants/events/{eventId}/participants` | path | `EventParticipantDto[]` | EventPlannerOnly | UC-021 |
| API-023 | POST | `/api/event-participants/events/{eventId}/participants/{participantUserId}/remove` | `{ reason }` | `204` | EventPlannerOnly | UC-022 |

Rules: visible profiles require valid ticket and event start time; removed/refunded tickets lose access; planner participant DTO includes `MobileNumber` for emergency calls; emergency removal refunds and disables conversations.

## Event Chats

| ID | Method | Route | Request | Response | Auth | Use Case |
|---|---|---|---|---|---|---|
| API-024 | GET | `/api/event-chats/me/conversations` | none | `EventConversationDto[]` | EndUserOnly | UC-023 |
| API-025 | POST | `/api/event-chats/events/{eventId}/conversations` | `{ participantUserId }` | `EventConversationDto` | EndUserOnly | UC-024 |
| API-026 | POST | `/api/event-chats/conversations/{conversationId}/messages` | `{ body }` | `EventConversationDto` | EndUserOnly | UC-025 |
| API-027 | POST | `/api/event-chats/conversations/{conversationId}/blocks` | `{ blockedUserId }` | `204` | EndUserOnly | UC-026 |

Rules: chat starts after event start; both users need valid tickets; starter cannot exceed `NumberOfChatAllowed`; blocked/disabled conversations cannot send.

## Event Surveys

| ID | Method | Route | Request | Response | Auth | Use Case |
|---|---|---|---|---|---|---|
| API-028 | GET | `/api/event-surveys/events/{eventId}/me` | path | `EventSurveyDto` or `404` | EndUserOnly | UC-027 |
| API-029 | POST | `/api/event-surveys/events/{eventId}/me` | `{ ratings, comment }` | `EventSurveyDto` | EndUserOnly | UC-028 |

Rules: event must have ended; user needs valid ticket; all 5 factors required; score 1-5; planner metrics update after save.

## Event Types

| ID | Method | Route | Request | Response | Auth | Use Case |
|---|---|---|---|---|---|---|
| API-030 | GET | `/api/event-types` | none | `EventTypeDto[]` | Anonymous | UC-029 |
| API-031 | POST | `/api/event-types` | `{ name, description, isActive }` | `EventTypeDto` | AdminOnly | UC-030 |
| API-032 | PUT | `/api/event-types/{id}` | `{ name, description, isActive }` | `EventTypeDto` | AdminOnly | UC-030 |

Rules: active list only; name 2-100 chars; description max 500.

## Balances

| ID | Method | Route | Request | Response | Auth | Use Case |
|---|---|---|---|---|---|---|
| API-033 | GET | `/api/balances/me` | none | `BalanceDto` | Authenticated | UC-031 |
| API-034 | GET | `/api/balances/{userId}` | path | `BalanceDto` | AdminOnly | UC-032 |
| API-035 | POST | `/api/balances/{userId}/adjust` | `{ amount, description }` | `BalanceDto` | AdminOnly | UC-033 |

Rules: balance account created lazily; amount must be positive; debit cannot go negative.

## Moderation

| ID | Method | Route | Request | Response | Auth | Use Case |
|---|---|---|---|---|---|---|
| API-036 | POST | `/api/moderation-reports` | `{ reportedUserId, datingEventId?, eventConversationId?, reason, description }` | `ModerationReportDto` | Authenticated | UC-034 |
| API-037 | GET | `/api/moderation-reports` | none | own `ModerationReportDto[]` | Authenticated | UC-035 |
| API-038 | GET | `/api/moderation-reports/admin?status=` | query | `ModerationReportDto[]` | AdminOnly | UC-036 |
| API-039 | PUT | `/api/moderation-reports/{reportId}/review` | `{ status, note }` | `ModerationReportDto` | AdminOnly | UC-037 |

Rules: reporter cannot report self; event reports require both users in event; conversation reports require reporter in conversation; admin review status cannot be Pending.

## Admin Users

| ID | Method | Route | Request | Response | Auth | Use Case |
|---|---|---|---|---|---|---|
| API-040 | PUT | `/api/admin/users/{userId}/role` | `{ role }` | `204` | AdminOnly | UC-038 |

Rules: user must exist; role enum must be valid by model binding.
