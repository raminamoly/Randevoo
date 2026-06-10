# C4 Component Diagram

## Purpose
Backend component view.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.WebApi`
- `src/Randevoo.Application`
- `src/Randevoo.Infrastructure`

```mermaid
flowchart TD
  Endpoints[Minimal API endpoint groups] --> Handlers[Application commands/queries/handlers]
  Endpoints --> Middleware[Correlation, activity, exception middleware]
  Endpoints --> Hub[EventChatHub]
  Handlers --> Repositories[Repository interfaces]
  Repositories --> EfRepos[Infrastructure repositories]
  EfRepos --> DbContext[RandevooDbContext]
  Handlers --> Services[Token, audit, privacy, notification ports]
  Services --> InfraServices[Infrastructure service implementations]
```

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
