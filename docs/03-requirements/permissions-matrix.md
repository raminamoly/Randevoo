# Permissions Matrix

## Purpose
Document role/action matrix from API policies and AdminPanel authorization.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.WebApi/Program.cs`
- `src/Randevoo.AdminPanel/Program.cs`

| Role/Policy | Allowed actions | Forbidden/limited actions | Evidence |
| --- | --- | --- | --- |
| Anonymous/Public | Public endpoints such as open event listing, auth code request/verify, public planner/event pages | Authenticated account operations | Auth endpoints and AllowAnonymous pages |
| EndUserOnly | Buy event tickets, event chat, event surveys | Planner/admin management | WebApi endpoint policies |
| EventPlannerOnly | Create/manage own dating events, participant lists, SMS actions | Admin-only user/finance controls | WebApi endpoint policies |
| SupportOrAdmin | Support ticket staff lists/status updates, payment receipt review pages | Admin-only settings unless admin | WebApi/AdminPanel policies |
| AdminOnly | User roles, event type writes, finance adjustments, moderation review, operation permissions | None detected inside admin domain | WebApi/AdminPanel policies |
| AdminOrPlanner | Planner/event dashboards and received operational views | Admin-only settings/user management | AdminPanel policies |

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
