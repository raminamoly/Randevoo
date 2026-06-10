# Dependency Rules

## Purpose
Rules for maintaining architecture.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.AdminPanel/Randevoo.AdminPanel.csproj`
- `src/Randevoo.Application/Randevoo.Application.csproj`
- `src/Randevoo.Domain/Randevoo.Domain.csproj`
- `src/Randevoo.Infrastructure/Randevoo.Infrastructure.csproj`
- `src/Randevoo.WebApi/Randevoo.WebApi.csproj`
- `tests/Randevoo.Tests.Integration/Randevoo.Tests.Integration.csproj`
- `tests/Randevoo.Tests.Unit/Randevoo.Tests.Unit.csproj`

- Domain must not reference Infrastructure, WebApi, or AdminPanel.
- Application should depend on Domain and abstractions only.
- Infrastructure may implement Application/Domain interfaces.
- WebApi is the API composition root.
- AdminPanel should call WebApi/API clients rather than reaching into database persistence.
- Tests may reference layers appropriate to their scope.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
