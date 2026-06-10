# API Tests

## Purpose
Document API test state.

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

Needs Verification: no dedicated API test project naming was detected. Add WebApplicationFactory-based tests for endpoint contracts, auth policies, and error responses.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
