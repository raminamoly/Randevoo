# WebApi Layer

## Purpose
Document endpoints, middleware, hubs, and HTTP composition.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.WebApi/Program.cs`
- `src/Randevoo.WebApi/Endpoints/AuthEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/AuthEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/AuthEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/AuthEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/AuthEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/AuthEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/BalanceEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/BalanceEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/BalanceEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs`

## Endpoints
| Method | Route | Auth | Handler | Source |
| --- | --- | --- | --- | --- |
| POST | `/api/auth/mobile/request-code` | Authenticated | RequestMobileCodeAsync | `src/Randevoo.WebApi/Endpoints/AuthEndpoints.cs:20` |
| POST | `/api/auth/mobile/verify-code` | Authenticated | VerifyMobileCodeAsync | `src/Randevoo.WebApi/Endpoints/AuthEndpoints.cs:23` |
| POST | `/api/auth/refresh-token` | Authenticated | RefreshTokenAsync | `src/Randevoo.WebApi/Endpoints/AuthEndpoints.cs:26` |
| POST | `/api/auth/logout` | Authenticated | LogoutAsync | `src/Randevoo.WebApi/Endpoints/AuthEndpoints.cs:29` |
| POST | `/api/auth/email/request-confirmation` | Authenticated | RequestEmailConfirmationAsync | `src/Randevoo.WebApi/Endpoints/AuthEndpoints.cs:32` |
| GET | `/api/auth/email/confirm` | Authenticated | ConfirmEmailAsync | `src/Randevoo.WebApi/Endpoints/AuthEndpoints.cs:36` |
| GET | `/api/balances/me` | AdminOnly | GetMineAsync | `src/Randevoo.WebApi/Endpoints/BalanceEndpoints.cs:17` |
| GET | `/api/balances/{userId:long}` | AdminOnly | GetByUserIdAsync | `src/Randevoo.WebApi/Endpoints/BalanceEndpoints.cs:18` |
| POST | `/api/balances/{userId:long}/adjust` | AdminOnly | AdjustAsync | `src/Randevoo.WebApi/Endpoints/BalanceEndpoints.cs:19` |
| GET | `/api/dating-events/open` | EventPlannerOnly | ListOpenAsync | `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:26` |
| POST | `/api/dating-events/` | EventPlannerOnly | CreateAsync | `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:27` |
| POST | `/api/dating-events/{eventId:long}/open` | EventPlannerOnly | OpenAsync | `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:28` |
| POST | `/api/dating-events/{eventId:long}/close` | EventPlannerOnly | CloseAsync | `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:29` |
| POST | `/api/dating-events/{eventId:long}/cancel` | EventPlannerOnly | CancelAsync | `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:30` |
| PUT | `/api/dating-events/{eventId:long}/location` | EventPlannerOnly | ChangeLocationAsync | `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:31` |
| PUT | `/api/dating-events/{eventId:long}/commission` | AdminOnly | SetCommissionAsync | `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:32` |
| POST | `/api/dating-events/{eventId:long}/tickets` | EndUserOnly | BuyTicketAsync | `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:33` |
| POST | `/api/dating-events/{eventId:long}/send-sms` | EventPlannerOnly | SendSmsAsync | `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:34` |
| POST | `/api/dating-events/sms-requests/{requestId:long}/approve` | AdminOnly | ApproveSmsRequestAsync | `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:35` |
| POST | `/api/dating-events/sms-requests/{requestId:long}/reject` | AdminOnly | RejectSmsRequestAsync | `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:36` |
| POST | `/api/dating-profiles/` | Authenticated | CreateProfileAsync | `src/Randevoo.WebApi/Endpoints/DatingProfileEndpoints.cs:20` |
| GET | `/api/dating-profiles/{profileId:long}` | Authenticated | GetProfileByIdAsync | `src/Randevoo.WebApi/Endpoints/DatingProfileEndpoints.cs:21` |
| GET | `/api/dating-profiles/by-user/{userId:long}` | Authenticated | GetProfileByUserIdAsync | `src/Randevoo.WebApi/Endpoints/DatingProfileEndpoints.cs:22` |
| PUT | `/api/dating-profiles/{profileId:long}` | Authenticated | UpdateProfileAsync | `src/Randevoo.WebApi/Endpoints/DatingProfileEndpoints.cs:23` |
| DELETE | `/api/dating-profiles/{profileId:long}` | Authenticated | DeleteProfileAsync | `src/Randevoo.WebApi/Endpoints/DatingProfileEndpoints.cs:24` |
| GET | `/api/event-chats/me/conversations` | EndUserOnly | ListMineAsync | `src/Randevoo.WebApi/Endpoints/EventChatEndpoints.cs:22` |
| POST | `/api/event-chats/events/{eventId:long}/conversations` | EndUserOnly | StartConversationAsync | `src/Randevoo.WebApi/Endpoints/EventChatEndpoints.cs:23` |
| POST | `/api/event-chats/events/{eventId:long}/likes/reject` | EndUserOnly | RejectLikeAsync | `src/Randevoo.WebApi/Endpoints/EventChatEndpoints.cs:24` |
| POST | `/api/event-chats/conversations/{conversationId:long}/messages` | EndUserOnly | SendMessageAsync | `src/Randevoo.WebApi/Endpoints/EventChatEndpoints.cs:25` |
| POST | `/api/event-chats/conversations/{conversationId:long}/blocks` | EndUserOnly | BlockUserAsync | `src/Randevoo.WebApi/Endpoints/EventChatEndpoints.cs:26` |
| GET | `/api/event-participants/me/archive` | Authenticated | ListMyArchiveAsync | `src/Randevoo.WebApi/Endpoints/EventParticipantEndpoints.cs:19` |
| GET | `/api/event-participants/events/{eventId:long}/profiles` | EventPlannerOnly | ListVisibleProfilesAsync | `src/Randevoo.WebApi/Endpoints/EventParticipantEndpoints.cs:20` |
| GET | `/api/event-participants/events/{eventId:long}/participants` | EventPlannerOnly | ListParticipantsAsync | `src/Randevoo.WebApi/Endpoints/EventParticipantEndpoints.cs:21` |
| POST | `/api/event-participants/events/{eventId:long}/participants/{participantUserId:long}/remove` | EventPlannerOnly | RemoveParticipantAsync | `src/Randevoo.WebApi/Endpoints/EventParticipantEndpoints.cs:22` |
| PUT | `/api/event-planner-profile/me` | Authenticated | UpsertMineAsync | `src/Randevoo.WebApi/Endpoints/EventPlannerProfileEndpoints.cs:16` |
| GET | `/api/event-surveys/events/{eventId:long}/me` | EndUserOnly | GetMineAsync | `src/Randevoo.WebApi/Endpoints/EventSurveyEndpoints.cs:17` |
| POST | `/api/event-surveys/events/{eventId:long}/me` | EndUserOnly | SubmitAsync | `src/Randevoo.WebApi/Endpoints/EventSurveyEndpoints.cs:18` |
| GET | `/api/event-types/` | AdminOnly | ListAsync | `src/Randevoo.WebApi/Endpoints/EventTypeEndpoints.cs:15` |
| POST | `/api/event-types/` | AdminOnly | UpsertAsync | `src/Randevoo.WebApi/Endpoints/EventTypeEndpoints.cs:16` |
| PUT | `/api/event-types/{id:long}` | AdminOnly | UpdateAsync | `src/Randevoo.WebApi/Endpoints/EventTypeEndpoints.cs:17` |
| POST | `/api/moderation-reports/` | AdminOnly | CreateAsync | `src/Randevoo.WebApi/Endpoints/ModerationEndpoints.cs:19` |
| GET | `/api/moderation-reports/` | AdminOnly | ListMineAsync | `src/Randevoo.WebApi/Endpoints/ModerationEndpoints.cs:20` |
| GET | `/api/moderation-reports/admin` | AdminOnly | ListAdminAsync | `src/Randevoo.WebApi/Endpoints/ModerationEndpoints.cs:21` |
| PUT | `/api/moderation-reports/{reportId:long}/review` | AdminOnly | ReviewAsync | `src/Randevoo.WebApi/Endpoints/ModerationEndpoints.cs:22` |
| GET | `/api/privacy/me/export` | Authenticated | ExportMeAsync | `src/Randevoo.WebApi/Endpoints/PrivacyEndpoints.cs:17` |
| DELETE | `/api/privacy/me` | Authenticated | DeleteMeAsync | `src/Randevoo.WebApi/Endpoints/PrivacyEndpoints.cs:18` |
| POST | `/api/support-tickets/` | SupportOrAdmin | CreateAsync | `src/Randevoo.WebApi/Endpoints/SupportTicketEndpoints.cs:24` |
| GET | `/api/support-tickets/` | SupportOrAdmin | ListMineAsync | `src/Randevoo.WebApi/Endpoints/SupportTicketEndpoints.cs:25` |
| GET | `/api/support-tickets/staff` | SupportOrAdmin | ListStaffAsync | `src/Randevoo.WebApi/Endpoints/SupportTicketEndpoints.cs:26` |
| GET | `/api/support-tickets/{ticketId:long}` | Authenticated | GetAsync | `src/Randevoo.WebApi/Endpoints/SupportTicketEndpoints.cs:27` |
| POST | `/api/support-tickets/{ticketId:long}/replies` | SupportOrAdmin | ReplyAsync | `src/Randevoo.WebApi/Endpoints/SupportTicketEndpoints.cs:28` |
| PUT | `/api/support-tickets/{ticketId:long}/status` | SupportOrAdmin | ChangeStatusAsync | `src/Randevoo.WebApi/Endpoints/SupportTicketEndpoints.cs:29` |
| PUT | `/api/support-tickets/{ticketId:long}/assignee` | AdminOnly | ReassignAsync | `src/Randevoo.WebApi/Endpoints/SupportTicketEndpoints.cs:30` |
| PUT | `/api/admin/users/{userId:long}/role` | AdminOnly | ChangeRoleAsync | `src/Randevoo.WebApi/Endpoints/UserAdminEndpoints.cs:16` |

## Hubs
- EventChatHub: authorized (`src/Randevoo.WebApi/Hubs/EventChatHub.cs`)

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
