# Randevoo Documentation

This folder is the source of truth for the current implementation. Documentation is based on the codebase as inspected, not planned future features.

## Start Here

- [System overview](system-overview.md)
- [Traceability matrix](traceability-matrix.md)

## Domain

- [Entities](03-domain/entities.md)
- [Aggregates](03-domain/aggregates.md)
- [Domain events](03-domain/domain-events.md)

## Use Cases

- [Use case map](04-use-cases/use-case-map.md)
- [Authentication](04-use-cases/UC-001-authentication.md)
- [Dating profile](04-use-cases/UC-005-dating-profile.md)
- [Event planner profile](04-use-cases/UC-009-event-planner-profile.md)
- [Dating events](04-use-cases/UC-011-dating-events.md)
- [Ticketing and participants](04-use-cases/UC-017-ticketing-participants.md)
- [Event chat](04-use-cases/UC-024-event-chat.md)
- [Event survey](04-use-cases/UC-028-event-survey.md)
- [Event types](04-use-cases/UC-030-event-types.md)
- [Balances](04-use-cases/UC-031-balances.md)
- [Moderation](04-use-cases/UC-034-moderation.md)
- [Admin users](04-use-cases/UC-038-admin-users.md)

## Flows

- [Participant flow](05-flows/participant-flow.md)
- [EventPlanner flow](05-flows/organizer-flow.md)
- [Admin flow](05-flows/admin-flow.md)
- [System flows](05-flows/system-flows.md)

## Architecture

- [Architecture overview](06-architecture/architecture-overview.md)
- [API contracts](06-architecture/api-contracts.md)
- [Database design](06-architecture/database-design.md)

## Observability

- [Logging strategy](observability/logging-strategy.md)

## Testing

- [Test strategy](08-testing/test-strategy.md)
- [Integration tests](08-testing/integration-tests.md)
- [E2E tests](08-testing/e2e-tests.md)

## Existing Diagram Docs

These older diagram-focused files are still useful:

- [Class diagram](class-diagram.md)
- [Component diagram](component-diagram.md)
- [Collaboration diagram](collaboration-diagram.md)
- [Use case diagram](use-case-diagram.md)
- [Scenario-based test diagram](scenario-based-test-diagram.md)
- [Safety moderation flow](safety-moderation-flow.md)
- [Balance refund flow](balance-refund-flow.md)
- [Diagram index](diagram-index.md)

## Known TODOs

- Add production SMS/email providers.
- Add E2E tests when a frontend exists.
- Expand SQL Server/Testcontainers integration tests beyond the current unique-index smoke test.
