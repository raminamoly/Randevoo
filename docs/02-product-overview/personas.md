# Personas

## Purpose
Define personas implied by roles, endpoints, and UI.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.Domain/Enums/UserRole.cs`
- `src/Randevoo.AdminPanel/Program.cs`

- Guest: can access public pages and open event listings where allowed.
- End user / participant: authenticates, manages dating profile, buys tickets, joins event-related interactions, submits surveys/reports/support tickets.
- Event planner: manages planner profile, events, participants, SMS requests, received receipts, and planner finance views.
- Support staff: handles support tickets and selected operational views.
- Admin: manages users, settings, event types, finance, moderation, operation permissions, and global dashboards.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
