# Event Creation Flow

## Purpose
Document event creation flow from current code evidence.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.Application/Features/DatingEvents/Commands/CreateDatingEvent/CreateDatingEventHandler.cs`
- `src/Randevoo.AdminPanel/Pages/Events/Edit.cshtml`

```mermaid
sequenceDiagram
  actor Planner
  participant AdminPanel
  participant API
  participant Handler
  participant Db
  Planner->>AdminPanel: Fill event form
  AdminPanel->>API: Create dating event
  API->>Handler: CreateDatingEventCommand
  Handler->>Db: Store event, tickets, FAQs/tags as applicable
  Db-->>Handler: saved event
  Handler-->>API: DTO
  API-->>AdminPanel: result
```

## Gaps or uncertainties
- No additional gaps beyond the repository evidence listed here.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
