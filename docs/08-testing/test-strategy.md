# Test Strategy

## Current Test Types

| Type | Project | Purpose |
|---|---|---|
| Unit | `Randevoo.Tests.Unit` | Domain entity behavior |
| Integration | `Randevoo.Tests.Integration` | Minimal API endpoints with WebApplicationFactory and EF InMemory |
| E2E | Not present | No browser/UI E2E tests found |

## Unit Test Coverage

- `UserTests`: mobile constructor, invalid mobile, login code lifecycle, SMS request throttling, failed-code lockout, email confirmation, profile creation, duplicate profile, deactivate, role change.
- `UserProfileTests`: constructor defaults, display name update, interest add/remove, duplicate/max interests, soft delete, age calculation, null user guard.

## Integration Test Coverage

- Auth passwordless login, refresh-token rotation, logout revocation, SMS throttling, lockout, and email confirmation.
- Dating profile authorization and ownership regression tests.
- Dating profile create/get/update/delete.
- Event planner profile creation.
- Dating event create/open/location/commission.
- Admin balance adjustment.
- Ticket purchase and buyer balance.
- EndUser cannot create event.
- Event archive, participant profiles, chat, block, chat limit.
- Survey submission and planner metrics.
- Planner participant list includes mobile number.
- Emergency removal and refund.
- Event type lookup.
- Admin role change.
- Optional SQL Server/Testcontainers relational constraint smoke test.

## Risk Areas

- Most integration tests still use EF InMemory for speed.
- SQL Server/Testcontainers coverage is opt-in with `RUN_SQLSERVER_TESTCONTAINERS=true`.
- No automated E2E/UI tests exist.
- Domain events are not asserted beyond some domain unit tests.

## TODO

- Expand SQL Server/Testcontainers coverage beyond the current unique-index smoke test.
- Add tests for admin event-type create/update.
- Add tests for moderation report list/review edge cases.
