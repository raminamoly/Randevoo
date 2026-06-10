# Profile Flow

## Purpose
Document profile flow from current code evidence.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.WebApi/Endpoints/DatingProfileEndpoints.cs`
- `src/Randevoo.Domain/Entities/UserProfile.cs`

```mermaid
sequenceDiagram
  actor User
  participant API as DatingProfileEndpoints
  participant Handler
  participant Repo as UserProfileRepository
  User->>API: Create/update profile
  API->>Handler: command/query
  Handler->>Repo: persist profile, lookups, images
  Repo-->>Handler: profile
  Handler-->>API: DTO
  API-->>User: profile response
```

## Gaps or uncertainties
- No additional gaps beyond the repository evidence listed here.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
