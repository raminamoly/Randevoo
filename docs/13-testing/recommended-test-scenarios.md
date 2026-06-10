# Recommended Test Scenarios

## Purpose
List recommended test additions.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `tests/Randevoo.Tests.Integration/AuthApiTests.cs`
- `tests/Randevoo.Tests.Integration/DatingEventApiTests.cs`
- `tests/Randevoo.Tests.Integration/DatingProfileApiTests.cs`
- `tests/Randevoo.Tests.Integration/ObservabilityTests.cs`
- `tests/Randevoo.Tests.Integration/SqlServerRelationalTests.cs`
- `tests/Randevoo.Tests.Integration/SupportTicketRepositoryTests.cs`
- `tests/Randevoo.Tests.Unit/Builder/UserBuilder.cs`
- `tests/Randevoo.Tests.Unit/DatingEventTests.cs`
- `tests/Randevoo.Tests.Unit/SupportTicketTests.cs`
- `tests/Randevoo.Tests.Unit/UserProfileTests.cs`
- `tests/Randevoo.Tests.Unit/UserTests.cs`

- Domain: event lifecycle, ticket capacity/pricing, profile validation, support ticket state transitions.
- Application: every command/query handler success/failure path.
- API: auth, authorization policies, validation errors, problem details, pagination/filtering.
- Database: relationships, delete behavior, indexes, migrations, seed idempotency.
- Authentication: code expiry, brute force, refresh token rotation/revocation.
- User flows: profile creation, event creation, buy ticket, chat, survey, support, moderation.
- Payment: online/manual receipt status transitions, currency/exchange calculations, refunds.
- UI: AdminPanel forms/tables, empty/error states, permission gates.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
