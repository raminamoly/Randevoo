# Permissions Matrix

## Purpose
Document role/action matrix from API policies and AdminPanel authorization.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.WebApi/Program.cs`
- `src/Randevoo.AdminPanel/Program.cs`
- `src/Randevoo.Domain/Constants/OperationPermissionCatalog.cs`
- `src/Randevoo.Infrastructure/Data/OperationPermissionCatalogSync.cs`
- `src/Randevoo.AdminPanel/Pages/Settings/OperationPermissions.cshtml`

| Role/Policy | Allowed actions | Forbidden/limited actions | Evidence |
| --- | --- | --- | --- |
| Anonymous/Public | Public endpoints such as open event listing, auth code request/verify, public planner/event pages | Authenticated account operations | Auth endpoints and AllowAnonymous pages |
| EndUserOnly | Buy event tickets, event chat, event surveys | Planner/admin management | WebApi endpoint policies |
| EventPlannerOnly | Create/manage own dating events, participant lists, SMS actions | Admin-only user/finance controls | WebApi endpoint policies |
| SupportOrAdmin | Support ticket staff lists/status updates, payment receipt review pages | Admin-only settings unless admin | WebApi/AdminPanel policies |
| AdminOnly | User roles, event type writes, finance adjustments, moderation review, operation permissions | None detected inside admin domain | WebApi/AdminPanel policies |
| AdminOrPlanner | Planner/event dashboards and received operational views | Admin-only settings/user management | AdminPanel policies |

## Admin operation permissions catalog
The admin panel has a database-backed operation permission catalog. `OperationPermissionCatalog` is the source of truth for fine-grained admin operations, including page access, grid actions, form submits, exports, sensitive data visibility, and sensitive business actions. Startup sync writes these definitions into `PermissionActions` and creates missing `RoleOperationPermissions` rows for Admin, EventPlanner, and PlatformSupportTeam.

EndUser is intentionally excluded from `/Settings/OperationPermissions` because this page manages admin-panel roles only. The page itself remains protected by the static `AdminOnly` policy; dynamic operation permissions refine what roles can do elsewhere and should not weaken the hard admin guard around permission management.

When adding a new admin page handler, grid button, dropdown operation, export, or sensitive field reveal, add a matching action to `OperationPermissionCatalog` with the relevant entity, group, page path, handler, UI surface, risk level, display order, and default role grants.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
