# Non Functional Requirements

## Purpose
Document current non-functional evidence and gaps.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.AdminPanel/Properties/launchSettings.json`
- `src/Randevoo.AdminPanel/appsettings.Development.json`
- `src/Randevoo.AdminPanel/appsettings.json`
- `src/Randevoo.WebApi/Properties/launchSettings.json`
- `src/Randevoo.WebApi/appsettings.Development.json`
- `src/Randevoo.WebApi/appsettings.Production.example.json`
- `src/Randevoo.WebApi/appsettings.json`

- Security: JWT bearer auth in WebApi and cookie auth in AdminPanel.
- Maintainability: layered projects and feature folders.
- Observability: audit log entity, activity/correlation middleware, logging configuration.
- Persistence reliability: EF Core migrations and repository abstractions.
- Privacy: privacy export/delete endpoints exist.

## Gaps or uncertainties
- No load/performance budget found.
- No disaster recovery or backup strategy found.
- No production monitoring stack found.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
