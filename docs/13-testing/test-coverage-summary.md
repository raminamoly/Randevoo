# Test Coverage Summary

## Purpose
Summarize coverage by area from test names.

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

Existing tests cover selected domain/application/infrastructure scenarios. Coverage appears strongest where explicit unit/integration tests exist and weaker for UI, full API authorization, payment gateway behavior, privacy deletion/export, moderation abuse flows, and AdminPanel visual workflows.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
