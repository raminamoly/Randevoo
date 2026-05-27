---
applyTo: "src/**/*.cs"
---

# Clean Architecture Standards

## Layering Rules
- Domain must not reference Infrastructure, EF Core, ASP.NET Core, or external SDKs
- Application coordinates use cases and orchestration
- Infrastructure handles persistence, integrations, caching, messaging, and external services
- Presentation handles transport concerns only

## Domain Layer
- Domain contains entities, value objects, enums, specifications, and domain events
- Enforce invariants inside aggregate roots
- Avoid anemic domain models
- Keep domain behavior close to domain data
- Avoid setters that bypass business rules

## Application Layer
- Use Application layer for orchestration and workflows
- Use interfaces for repositories and infrastructure dependencies
- Keep handlers focused on one business capability
- Avoid infrastructure implementation details

## Infrastructure Layer
- Infrastructure implements abstractions from Application
- External services must be isolated behind interfaces
- Centralize third-party SDK access
- Add resiliency for external integrations

## Presentation Layer
- Controllers/endpoints should contain no business logic
- Map DTOs at boundaries
- Validate requests before application execution
- Return standardized responses and errors

## Cross-Cutting Concerns
- Use middleware/pipeline behaviors for:
  - Logging
  - Validation
  - Authorization
  - Performance metrics
  - Correlation IDs
- Keep cross-cutting concerns centralized
