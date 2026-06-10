# Endpoints Catalog

## Purpose
Detailed API endpoint catalog from Minimal API route declarations.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
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
- `src/Randevoo.WebApi/Endpoints/DatingProfileEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/DatingProfileEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/DatingProfileEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/DatingProfileEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/DatingProfileEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/EventChatEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/EventChatEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/EventChatEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/EventChatEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/EventChatEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/EventParticipantEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/EventParticipantEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/EventParticipantEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/EventParticipantEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/EventPlannerProfileEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/EventSurveyEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/EventSurveyEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/EventTypeEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/EventTypeEndpoints.cs`
- `src/Randevoo.WebApi/Endpoints/EventTypeEndpoints.cs`

| Method | Route | Purpose | Request model | Response model | Auth | Handler | Related entities | Source |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| POST | `/api/auth/mobile/request-code` | Request Mobile Code Async | Needs Verification in handler parameters | Needs Verification in endpoint result | Authenticated | RequestMobileCodeAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/AuthEndpoints.cs:20` |
| POST | `/api/auth/mobile/verify-code` | Verify Mobile Code Async | Needs Verification in handler parameters | Needs Verification in endpoint result | Authenticated | VerifyMobileCodeAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/AuthEndpoints.cs:23` |
| POST | `/api/auth/refresh-token` | Refresh Token Async | Needs Verification in handler parameters | Needs Verification in endpoint result | Authenticated | RefreshTokenAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/AuthEndpoints.cs:26` |
| POST | `/api/auth/logout` | Logout Async | Needs Verification in handler parameters | Needs Verification in endpoint result | Authenticated | LogoutAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/AuthEndpoints.cs:29` |
| POST | `/api/auth/email/request-confirmation` | Request Email Confirmation Async | Needs Verification in handler parameters | Needs Verification in endpoint result | Authenticated | RequestEmailConfirmationAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/AuthEndpoints.cs:32` |
| GET | `/api/auth/email/confirm` | Confirm Email Async | Needs Verification in handler parameters | Needs Verification in endpoint result | Authenticated | ConfirmEmailAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/AuthEndpoints.cs:36` |
| GET | `/api/balances/me` | Get Mine Async | Needs Verification in handler parameters | Needs Verification in endpoint result | AdminOnly | GetMineAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/BalanceEndpoints.cs:17` |
| GET | `/api/balances/{userId:long}` | Get By User Id Async | Needs Verification in handler parameters | Needs Verification in endpoint result | AdminOnly | GetByUserIdAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/BalanceEndpoints.cs:18` |
| POST | `/api/balances/{userId:long}/adjust` | Adjust Async | Needs Verification in handler parameters | Needs Verification in endpoint result | AdminOnly | AdjustAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/BalanceEndpoints.cs:19` |
| GET | `/api/dating-events/open` | List Open Async | Needs Verification in handler parameters | Needs Verification in endpoint result | EventPlannerOnly | ListOpenAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:26` |
| POST | `/api/dating-events/` | Create Async | Needs Verification in handler parameters | Needs Verification in endpoint result | EventPlannerOnly | CreateAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:27` |
| POST | `/api/dating-events/{eventId:long}/open` | Open Async | Needs Verification in handler parameters | Needs Verification in endpoint result | EventPlannerOnly | OpenAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:28` |
| POST | `/api/dating-events/{eventId:long}/close` | Close Async | Needs Verification in handler parameters | Needs Verification in endpoint result | EventPlannerOnly | CloseAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:29` |
| POST | `/api/dating-events/{eventId:long}/cancel` | Cancel Async | Needs Verification in handler parameters | Needs Verification in endpoint result | EventPlannerOnly | CancelAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:30` |
| PUT | `/api/dating-events/{eventId:long}/location` | Change Location Async | Needs Verification in handler parameters | Needs Verification in endpoint result | EventPlannerOnly | ChangeLocationAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:31` |
| PUT | `/api/dating-events/{eventId:long}/commission` | Set Commission Async | Needs Verification in handler parameters | Needs Verification in endpoint result | AdminOnly | SetCommissionAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:32` |
| POST | `/api/dating-events/{eventId:long}/tickets` | Buy Ticket Async | Needs Verification in handler parameters | Needs Verification in endpoint result | EndUserOnly | BuyTicketAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:33` |
| POST | `/api/dating-events/{eventId:long}/send-sms` | Send Sms Async | Needs Verification in handler parameters | Needs Verification in endpoint result | EventPlannerOnly | SendSmsAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:34` |
| POST | `/api/dating-events/sms-requests/{requestId:long}/approve` | Approve Sms Request Async | Needs Verification in handler parameters | Needs Verification in endpoint result | AdminOnly | ApproveSmsRequestAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:35` |
| POST | `/api/dating-events/sms-requests/{requestId:long}/reject` | Reject Sms Request Async | Needs Verification in handler parameters | Needs Verification in endpoint result | AdminOnly | RejectSmsRequestAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:36` |
| POST | `/api/dating-profiles/` | Create Profile Async | Needs Verification in handler parameters | Needs Verification in endpoint result | Authenticated | CreateProfileAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/DatingProfileEndpoints.cs:20` |
| GET | `/api/dating-profiles/{profileId:long}` | Get Profile By Id Async | Needs Verification in handler parameters | Needs Verification in endpoint result | Authenticated | GetProfileByIdAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/DatingProfileEndpoints.cs:21` |
| GET | `/api/dating-profiles/by-user/{userId:long}` | Get Profile By User Id Async | Needs Verification in handler parameters | Needs Verification in endpoint result | Authenticated | GetProfileByUserIdAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/DatingProfileEndpoints.cs:22` |
| PUT | `/api/dating-profiles/{profileId:long}` | Update Profile Async | Needs Verification in handler parameters | Needs Verification in endpoint result | Authenticated | UpdateProfileAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/DatingProfileEndpoints.cs:23` |
| DELETE | `/api/dating-profiles/{profileId:long}` | Delete Profile Async | Needs Verification in handler parameters | Needs Verification in endpoint result | Authenticated | DeleteProfileAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/DatingProfileEndpoints.cs:24` |
| GET | `/api/event-chats/me/conversations` | List Mine Async | Needs Verification in handler parameters | Needs Verification in endpoint result | EndUserOnly | ListMineAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/EventChatEndpoints.cs:22` |
| POST | `/api/event-chats/events/{eventId:long}/conversations` | Start Conversation Async | Needs Verification in handler parameters | Needs Verification in endpoint result | EndUserOnly | StartConversationAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/EventChatEndpoints.cs:23` |
| POST | `/api/event-chats/events/{eventId:long}/likes/reject` | Reject Like Async | Needs Verification in handler parameters | Needs Verification in endpoint result | EndUserOnly | RejectLikeAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/EventChatEndpoints.cs:24` |
| POST | `/api/event-chats/conversations/{conversationId:long}/messages` | Send Message Async | Needs Verification in handler parameters | Needs Verification in endpoint result | EndUserOnly | SendMessageAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/EventChatEndpoints.cs:25` |
| POST | `/api/event-chats/conversations/{conversationId:long}/blocks` | Block User Async | Needs Verification in handler parameters | Needs Verification in endpoint result | EndUserOnly | BlockUserAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/EventChatEndpoints.cs:26` |
| GET | `/api/event-participants/me/archive` | List My Archive Async | Needs Verification in handler parameters | Needs Verification in endpoint result | Authenticated | ListMyArchiveAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/EventParticipantEndpoints.cs:19` |
| GET | `/api/event-participants/events/{eventId:long}/profiles` | List Visible Profiles Async | Needs Verification in handler parameters | Needs Verification in endpoint result | EventPlannerOnly | ListVisibleProfilesAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/EventParticipantEndpoints.cs:20` |
| GET | `/api/event-participants/events/{eventId:long}/participants` | List Participants Async | Needs Verification in handler parameters | Needs Verification in endpoint result | EventPlannerOnly | ListParticipantsAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/EventParticipantEndpoints.cs:21` |
| POST | `/api/event-participants/events/{eventId:long}/participants/{participantUserId:long}/remove` | Remove Participant Async | Needs Verification in handler parameters | Needs Verification in endpoint result | EventPlannerOnly | RemoveParticipantAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/EventParticipantEndpoints.cs:22` |
| PUT | `/api/event-planner-profile/me` | Upsert Mine Async | Needs Verification in handler parameters | Needs Verification in endpoint result | Authenticated | UpsertMineAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/EventPlannerProfileEndpoints.cs:16` |
| GET | `/api/event-surveys/events/{eventId:long}/me` | Get Mine Async | Needs Verification in handler parameters | Needs Verification in endpoint result | EndUserOnly | GetMineAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/EventSurveyEndpoints.cs:17` |
| POST | `/api/event-surveys/events/{eventId:long}/me` | Submit Async | Needs Verification in handler parameters | Needs Verification in endpoint result | EndUserOnly | SubmitAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/EventSurveyEndpoints.cs:18` |
| GET | `/api/event-types/` | List Async | Needs Verification in handler parameters | Needs Verification in endpoint result | AdminOnly | ListAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/EventTypeEndpoints.cs:15` |
| POST | `/api/event-types/` | Upsert Async | Needs Verification in handler parameters | Needs Verification in endpoint result | AdminOnly | UpsertAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/EventTypeEndpoints.cs:16` |
| PUT | `/api/event-types/{id:long}` | Update Async | Needs Verification in handler parameters | Needs Verification in endpoint result | AdminOnly | UpdateAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/EventTypeEndpoints.cs:17` |
| POST | `/api/moderation-reports/` | Create Async | Needs Verification in handler parameters | Needs Verification in endpoint result | AdminOnly | CreateAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/ModerationEndpoints.cs:19` |
| GET | `/api/moderation-reports/` | List Mine Async | Needs Verification in handler parameters | Needs Verification in endpoint result | AdminOnly | ListMineAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/ModerationEndpoints.cs:20` |
| GET | `/api/moderation-reports/admin` | List Admin Async | Needs Verification in handler parameters | Needs Verification in endpoint result | AdminOnly | ListAdminAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/ModerationEndpoints.cs:21` |
| PUT | `/api/moderation-reports/{reportId:long}/review` | Review Async | Needs Verification in handler parameters | Needs Verification in endpoint result | AdminOnly | ReviewAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/ModerationEndpoints.cs:22` |
| GET | `/api/privacy/me/export` | Export Me Async | Needs Verification in handler parameters | Needs Verification in endpoint result | Authenticated | ExportMeAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/PrivacyEndpoints.cs:17` |
| DELETE | `/api/privacy/me` | Delete Me Async | Needs Verification in handler parameters | Needs Verification in endpoint result | Authenticated | DeleteMeAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/PrivacyEndpoints.cs:18` |
| POST | `/api/support-tickets/` | Create Async | Needs Verification in handler parameters | Needs Verification in endpoint result | SupportOrAdmin | CreateAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/SupportTicketEndpoints.cs:24` |
| GET | `/api/support-tickets/` | List Mine Async | Needs Verification in handler parameters | Needs Verification in endpoint result | SupportOrAdmin | ListMineAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/SupportTicketEndpoints.cs:25` |
| GET | `/api/support-tickets/staff` | List Staff Async | Needs Verification in handler parameters | Needs Verification in endpoint result | SupportOrAdmin | ListStaffAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/SupportTicketEndpoints.cs:26` |
| GET | `/api/support-tickets/{ticketId:long}` | Get Async | Needs Verification in handler parameters | Needs Verification in endpoint result | Authenticated | GetAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/SupportTicketEndpoints.cs:27` |
| POST | `/api/support-tickets/{ticketId:long}/replies` | Reply Async | Needs Verification in handler parameters | Needs Verification in endpoint result | SupportOrAdmin | ReplyAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/SupportTicketEndpoints.cs:28` |
| PUT | `/api/support-tickets/{ticketId:long}/status` | Change Status Async | Needs Verification in handler parameters | Needs Verification in endpoint result | SupportOrAdmin | ChangeStatusAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/SupportTicketEndpoints.cs:29` |
| PUT | `/api/support-tickets/{ticketId:long}/assignee` | Reassign Async | Needs Verification in handler parameters | Needs Verification in endpoint result | AdminOnly | ReassignAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/SupportTicketEndpoints.cs:30` |
| PUT | `/api/admin/users/{userId:long}/role` | Change Role Async | Needs Verification in handler parameters | Needs Verification in endpoint result | AdminOnly | ChangeRoleAsync | Infer from endpoint group | `src/Randevoo.WebApi/Endpoints/UserAdminEndpoints.cs:16` |

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
