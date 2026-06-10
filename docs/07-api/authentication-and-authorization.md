# Authentication And Authorization

## Purpose
Document API auth model and policies.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.WebApi/Program.cs`
- `src/Randevoo.WebApi/Endpoints/AuthEndpoints.cs`

- JWT bearer authentication is configured in WebApi.
- Auth endpoints provide mobile code request/verify, refresh token, logout, and email confirmation flows.
- Policy names used by endpoints include AdminOnly, EventPlannerOnly, EndUserOnly, SupportOrAdmin, and authenticated defaults.

```mermaid
sequenceDiagram
  participant User
  participant API
  participant CodeSender
  participant TokenService
  User->>API: request mobile code
  API->>CodeSender: send/record code
  User->>API: verify code
  API->>TokenService: issue JWT + refresh token
  API-->>User: tokens
```

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
