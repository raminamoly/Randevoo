# Backend Overview

## Purpose
Summarize backend layers and responsibilities.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.WebApi/Program.cs`
- `src/Randevoo.Application`
- `src/Randevoo.Infrastructure`

- Domain: entities, value objects, enums, domain events, repository interfaces.
- Application: commands, queries, handlers, DTOs, application service ports.
- Infrastructure: EF Core, repositories, JWT, audit logging, SMS/email implementations, privacy data reader.
- WebApi: HTTP endpoints, middleware, SignalR hub, authentication/authorization.

```mermaid
sequenceDiagram
  participant Client
  participant WebApi
  participant Endpoint
  participant Handler
  participant Repository
  participant Db as Database
  Client->>WebApi: HTTP request / JWT
  WebApi->>Endpoint: route + policy
  Endpoint->>Handler: command/query
  Handler->>Repository: domain persistence
  Repository->>Db: EF Core
  Db-->>Repository: data
  Repository-->>Handler: entity/result
  Handler-->>Endpoint: DTO/result
  Endpoint-->>Client: JSON response
```
## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
