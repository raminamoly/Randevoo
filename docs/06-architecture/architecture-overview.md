# Architecture Overview

## Clean Architecture Mapping

```mermaid
flowchart TD
    Client[HTTP Client / Scalar] --> WebApi
    WebApi --> Application
    Application --> Domain
    Infrastructure --> Application
    Infrastructure --> Domain
    WebApi --> Infrastructure
    Infrastructure --> Sql[(SQL Server)]
```

## Layer Responsibilities

| Layer | Responsibility |
|---|---|
| Domain | Business entities, value objects, rules, domain events, repository contracts |
| Application | CQRS command/query handlers, DTO mapping, workflow orchestration |
| Infrastructure | EF Core, repositories, unit of work, JWT service, SMS/email implementations |
| WebApi | HTTP routing, auth policies, minimal API request/response boundary |
| Tests | Unit and integration test coverage |

## Request Flow

```mermaid
sequenceDiagram
    actor Client
    participant Endpoint as Minimal API Endpoint
    participant Sender as MediatR ISender
    participant Handler as Command/Query Handler
    participant Repo as Repository
    participant Domain
    participant Db as RandevooDbContext

    Client->>Endpoint: HTTP request
    Endpoint->>Endpoint: Read JWT claims / route/body
    Endpoint->>Sender: Send command/query
    Sender->>Handler: Dispatch
    Handler->>Repo: Load aggregates
    Repo->>Db: EF Core query
    Handler->>Domain: Execute behavior/rules
    Handler->>Repo: Add/update aggregates
    Handler->>Db: SaveChanges via UnitOfWork
    Handler-->>Endpoint: DTO/result
    Endpoint-->>Client: HTTP response
```

## Dependency Direction

- Application depends on Domain.
- Infrastructure depends on Application and Domain.
- WebApi depends on all production layers to compose services and map endpoints.
- Domain does not depend on Infrastructure or WebApi.

## Authentication and Authorization

- JWT bearer auth is configured in `Program.cs`.
- JWT includes `ClaimTypes.NameIdentifier`, `mobile_number`, and role claim.
- Policies:
  - `EndUserOnly`: `EndUser`, `Admin`
  - `EventPlannerOnly`: `EventPlanner`, `Admin`
  - `AdminOnly`: `Admin`

## Integrations

- SQL Server is configured through `AddRandevooInfrastructure(connectionString)`.
- `ConsoleSmsSender` and `ConsoleEmailSender` are placeholder notification implementations.
- Scalar is mapped only in Development.

## TODO / Assumption Required

- No centralized validation pipeline is configured; validation is mostly domain guards and handler checks.
- No domain event dispatch pipeline exists.
- No production notification provider exists.
