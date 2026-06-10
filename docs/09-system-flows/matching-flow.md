# Matching Flow

## Purpose
Document matching flow from current code evidence.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.WebApi/Endpoints/EventChatEndpoints.cs`
- `src/Randevoo.Domain/Entities/EventLike.cs`
- `src/Randevoo.Domain/Entities/EventConversation.cs`

```mermaid
sequenceDiagram
  actor User
  participant API as EventChatEndpoints
  participant Likes as EventLikeRepository
  participant Conv as EventConversationRepository
  User->>API: Start conversation / reject like
  API->>Likes: create/update like state
  API->>Conv: create conversation when rules allow
  Conv-->>API: conversation DTO
  API-->>User: conversation/list result
```

## Gaps or uncertainties
- No standalone Match entity was detected; event likes/conversations appear to provide the matching-adjacent model.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
