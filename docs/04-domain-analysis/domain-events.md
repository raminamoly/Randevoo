# Domain Events

## Purpose
Document domain event infrastructure and discovered event classes.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.Domain/Events`
- `src/Randevoo.Domain/Common/Events`

Domain event abstractions exist under `Domain/Common/Events`, with user and profile event classes under `Domain/Events`.

## Event files
- `src/Randevoo.Domain/Common/Events/DomainEvent.cs`
- `src/Randevoo.Domain/Common/Events/IDomainEvent.cs`
- `src/Randevoo.Domain/Events/BaseEntityEvents.cs`
- `src/Randevoo.Domain/Events/UserEvents.cs`
- `src/Randevoo.Domain/Events/UserProfileEvents.cs`

## Gaps or uncertainties
- No complete event dispatcher pipeline was confirmed during extraction.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
