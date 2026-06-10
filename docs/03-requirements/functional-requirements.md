# Functional Requirements

## Purpose
Extract implemented functional requirements from endpoints, handlers, and UI.

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

- The system shall support POST /api/auth/mobile/request-code via RequestMobileCodeAsync (Authenticated). Source: `src/Randevoo.WebApi/Endpoints/AuthEndpoints.cs:20`.
- The system shall support POST /api/auth/mobile/verify-code via VerifyMobileCodeAsync (Authenticated). Source: `src/Randevoo.WebApi/Endpoints/AuthEndpoints.cs:23`.
- The system shall support POST /api/auth/refresh-token via RefreshTokenAsync (Authenticated). Source: `src/Randevoo.WebApi/Endpoints/AuthEndpoints.cs:26`.
- The system shall support POST /api/auth/logout via LogoutAsync (Authenticated). Source: `src/Randevoo.WebApi/Endpoints/AuthEndpoints.cs:29`.
- The system shall support POST /api/auth/email/request-confirmation via RequestEmailConfirmationAsync (Authenticated). Source: `src/Randevoo.WebApi/Endpoints/AuthEndpoints.cs:32`.
- The system shall support GET /api/auth/email/confirm via ConfirmEmailAsync (Authenticated). Source: `src/Randevoo.WebApi/Endpoints/AuthEndpoints.cs:36`.
- The system shall support GET /api/balances/me via GetMineAsync (AdminOnly). Source: `src/Randevoo.WebApi/Endpoints/BalanceEndpoints.cs:17`.
- The system shall support GET /api/balances/{userId:long} via GetByUserIdAsync (AdminOnly). Source: `src/Randevoo.WebApi/Endpoints/BalanceEndpoints.cs:18`.
- The system shall support POST /api/balances/{userId:long}/adjust via AdjustAsync (AdminOnly). Source: `src/Randevoo.WebApi/Endpoints/BalanceEndpoints.cs:19`.
- The system shall support GET /api/dating-events/open via ListOpenAsync (EventPlannerOnly). Source: `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:26`.
- The system shall support POST /api/dating-events/ via CreateAsync (EventPlannerOnly). Source: `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:27`.
- The system shall support POST /api/dating-events/{eventId:long}/open via OpenAsync (EventPlannerOnly). Source: `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:28`.
- The system shall support POST /api/dating-events/{eventId:long}/close via CloseAsync (EventPlannerOnly). Source: `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:29`.
- The system shall support POST /api/dating-events/{eventId:long}/cancel via CancelAsync (EventPlannerOnly). Source: `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:30`.
- The system shall support PUT /api/dating-events/{eventId:long}/location via ChangeLocationAsync (EventPlannerOnly). Source: `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:31`.
- The system shall support PUT /api/dating-events/{eventId:long}/commission via SetCommissionAsync (AdminOnly). Source: `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:32`.
- The system shall support POST /api/dating-events/{eventId:long}/tickets via BuyTicketAsync (EndUserOnly). Source: `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:33`.
- The system shall support POST /api/dating-events/{eventId:long}/send-sms via SendSmsAsync (EventPlannerOnly). Source: `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:34`.
- The system shall support POST /api/dating-events/sms-requests/{requestId:long}/approve via ApproveSmsRequestAsync (AdminOnly). Source: `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:35`.
- The system shall support POST /api/dating-events/sms-requests/{requestId:long}/reject via RejectSmsRequestAsync (AdminOnly). Source: `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs:36`.
- The system shall support POST /api/dating-profiles/ via CreateProfileAsync (Authenticated). Source: `src/Randevoo.WebApi/Endpoints/DatingProfileEndpoints.cs:20`.
- The system shall support GET /api/dating-profiles/{profileId:long} via GetProfileByIdAsync (Authenticated). Source: `src/Randevoo.WebApi/Endpoints/DatingProfileEndpoints.cs:21`.
- The system shall support GET /api/dating-profiles/by-user/{userId:long} via GetProfileByUserIdAsync (Authenticated). Source: `src/Randevoo.WebApi/Endpoints/DatingProfileEndpoints.cs:22`.
- The system shall support PUT /api/dating-profiles/{profileId:long} via UpdateProfileAsync (Authenticated). Source: `src/Randevoo.WebApi/Endpoints/DatingProfileEndpoints.cs:23`.
- The system shall support DELETE /api/dating-profiles/{profileId:long} via DeleteProfileAsync (Authenticated). Source: `src/Randevoo.WebApi/Endpoints/DatingProfileEndpoints.cs:24`.
- The system shall support GET /api/event-chats/me/conversations via ListMineAsync (EndUserOnly). Source: `src/Randevoo.WebApi/Endpoints/EventChatEndpoints.cs:22`.
- The system shall support POST /api/event-chats/events/{eventId:long}/conversations via StartConversationAsync (EndUserOnly). Source: `src/Randevoo.WebApi/Endpoints/EventChatEndpoints.cs:23`.
- The system shall support POST /api/event-chats/events/{eventId:long}/likes/reject via RejectLikeAsync (EndUserOnly). Source: `src/Randevoo.WebApi/Endpoints/EventChatEndpoints.cs:24`.
- The system shall support POST /api/event-chats/conversations/{conversationId:long}/messages via SendMessageAsync (EndUserOnly). Source: `src/Randevoo.WebApi/Endpoints/EventChatEndpoints.cs:25`.
- The system shall support POST /api/event-chats/conversations/{conversationId:long}/blocks via BlockUserAsync (EndUserOnly). Source: `src/Randevoo.WebApi/Endpoints/EventChatEndpoints.cs:26`.
- The system shall support GET /api/event-participants/me/archive via ListMyArchiveAsync (Authenticated). Source: `src/Randevoo.WebApi/Endpoints/EventParticipantEndpoints.cs:19`.
- The system shall support GET /api/event-participants/events/{eventId:long}/profiles via ListVisibleProfilesAsync (EventPlannerOnly). Source: `src/Randevoo.WebApi/Endpoints/EventParticipantEndpoints.cs:20`.
- The system shall support GET /api/event-participants/events/{eventId:long}/participants via ListParticipantsAsync (EventPlannerOnly). Source: `src/Randevoo.WebApi/Endpoints/EventParticipantEndpoints.cs:21`.
- The system shall support POST /api/event-participants/events/{eventId:long}/participants/{participantUserId:long}/remove via RemoveParticipantAsync (EventPlannerOnly). Source: `src/Randevoo.WebApi/Endpoints/EventParticipantEndpoints.cs:22`.
- The system shall support PUT /api/event-planner-profile/me via UpsertMineAsync (Authenticated). Source: `src/Randevoo.WebApi/Endpoints/EventPlannerProfileEndpoints.cs:16`.
- The system shall support GET /api/event-surveys/events/{eventId:long}/me via GetMineAsync (EndUserOnly). Source: `src/Randevoo.WebApi/Endpoints/EventSurveyEndpoints.cs:17`.
- The system shall support POST /api/event-surveys/events/{eventId:long}/me via SubmitAsync (EndUserOnly). Source: `src/Randevoo.WebApi/Endpoints/EventSurveyEndpoints.cs:18`.
- The system shall support GET /api/event-types/ via ListAsync (AdminOnly). Source: `src/Randevoo.WebApi/Endpoints/EventTypeEndpoints.cs:15`.
- The system shall support POST /api/event-types/ via UpsertAsync (AdminOnly). Source: `src/Randevoo.WebApi/Endpoints/EventTypeEndpoints.cs:16`.
- The system shall support PUT /api/event-types/{id:long} via UpdateAsync (AdminOnly). Source: `src/Randevoo.WebApi/Endpoints/EventTypeEndpoints.cs:17`.
- The system shall support POST /api/moderation-reports/ via CreateAsync (AdminOnly). Source: `src/Randevoo.WebApi/Endpoints/ModerationEndpoints.cs:19`.
- The system shall support GET /api/moderation-reports/ via ListMineAsync (AdminOnly). Source: `src/Randevoo.WebApi/Endpoints/ModerationEndpoints.cs:20`.
- The system shall support GET /api/moderation-reports/admin via ListAdminAsync (AdminOnly). Source: `src/Randevoo.WebApi/Endpoints/ModerationEndpoints.cs:21`.
- The system shall support PUT /api/moderation-reports/{reportId:long}/review via ReviewAsync (AdminOnly). Source: `src/Randevoo.WebApi/Endpoints/ModerationEndpoints.cs:22`.
- The system shall support GET /api/privacy/me/export via ExportMeAsync (Authenticated). Source: `src/Randevoo.WebApi/Endpoints/PrivacyEndpoints.cs:17`.
- The system shall support DELETE /api/privacy/me via DeleteMeAsync (Authenticated). Source: `src/Randevoo.WebApi/Endpoints/PrivacyEndpoints.cs:18`.
- The system shall support POST /api/support-tickets/ via CreateAsync (SupportOrAdmin). Source: `src/Randevoo.WebApi/Endpoints/SupportTicketEndpoints.cs:24`.
- The system shall support GET /api/support-tickets/ via ListMineAsync (SupportOrAdmin). Source: `src/Randevoo.WebApi/Endpoints/SupportTicketEndpoints.cs:25`.
- The system shall support GET /api/support-tickets/staff via ListStaffAsync (SupportOrAdmin). Source: `src/Randevoo.WebApi/Endpoints/SupportTicketEndpoints.cs:26`.
- The system shall support GET /api/support-tickets/{ticketId:long} via GetAsync (Authenticated). Source: `src/Randevoo.WebApi/Endpoints/SupportTicketEndpoints.cs:27`.
- The system shall support POST /api/support-tickets/{ticketId:long}/replies via ReplyAsync (SupportOrAdmin). Source: `src/Randevoo.WebApi/Endpoints/SupportTicketEndpoints.cs:28`.
- The system shall support PUT /api/support-tickets/{ticketId:long}/status via ChangeStatusAsync (SupportOrAdmin). Source: `src/Randevoo.WebApi/Endpoints/SupportTicketEndpoints.cs:29`.
- The system shall support PUT /api/support-tickets/{ticketId:long}/assignee via ReassignAsync (AdminOnly). Source: `src/Randevoo.WebApi/Endpoints/SupportTicketEndpoints.cs:30`.
- The system shall support PUT /api/admin/users/{userId:long}/role via ChangeRoleAsync (AdminOnly). Source: `src/Randevoo.WebApi/Endpoints/UserAdminEndpoints.cs:16`.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
