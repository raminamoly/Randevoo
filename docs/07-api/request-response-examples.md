# Request Response Examples

## Purpose
Provide examples where route intent is clear and mark others for verification.

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

## Mobile code request
```http
POST /api/auth/mobile/request-code
Content-Type: application/json

{ "mobileNumber": "+989000000000" }
```

## Open event list
```http
GET /api/dating-events/open
Authorization: Bearer <token-if-required-by-runtime>
```

## Support ticket creation
```http
POST /api/support-tickets
Authorization: Bearer <token>
Content-Type: application/json

{ "subject": "Example", "message": "Example issue" }
```

Needs Verification: exact DTO property names must be checked in command/request classes before publishing as external API documentation.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
