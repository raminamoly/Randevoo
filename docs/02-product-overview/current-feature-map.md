# Current Feature Map

## Purpose
Map implemented and partial features to source evidence.

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

| Feature area | Evidence | Status |
| --- | --- | --- |
| Authentication | Auth endpoints, JWT service, refresh tokens | Implemented, needs production security review |
| Dating profiles | Profile endpoints/entities/admin pages | Implemented |
| Event management | Dating event endpoints/entities/admin pages | Implemented/active |
| Ticketing/payment | EventTicket, TicketOrder, OnlinePayment, ManualPaymentReceipt, balance entities | Implemented model and flows; gateway integration needs verification |
| Matching/likes/chat | EventLike, EventConversation, EventChatMessage, hub/endpoints | Partial event-based matching/conversation model |
| Moderation | ModerationReport endpoints/admin pages | Implemented basics, policy depth needs verification |
| Notifications | SMS queue, console email/SMS senders | Partial; production providers missing |
| Support | Support ticket entities/endpoints/admin pages | Implemented |

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
