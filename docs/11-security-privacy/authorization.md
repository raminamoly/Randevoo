# Authorization

## Purpose
Document policies and role access.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.WebApi/Program.cs`
- `src/Randevoo.AdminPanel/Program.cs`
- `src/Randevoo.Domain/Constants/OperationPermissionCatalog.cs`
- `src/Randevoo.Infrastructure/Data/OperationPermissionCatalogSync.cs`

Policies found include AdminOnly, EventPlannerOnly, EndUserOnly, SupportOrAdmin, AdminOrPlanner, and AdminPlannerOrSupport. Endpoint/page catalogs list source-level policy usage.

## Dynamic admin operation permissions
Static policies still gate entry into protected pages and folders. Fine-grained admin actions are modeled separately through `PermissionActions`, `RoleOperationPermissions`, and optional `UserOperationPermissionOverrides`.

The catalog sync process keeps `PermissionActions` aligned with `OperationPermissionCatalog` and creates missing role rows for Admin, EventPlanner, and PlatformSupportTeam. Existing role decisions are preserved during sync. Removed catalog actions are marked inactive/deprecated instead of being hard-deleted.

Security rule: `/Settings/OperationPermissions` is AdminOnly regardless of database permissions. Do not expose this page to planners, support users, or end users through dynamic permission rows.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
