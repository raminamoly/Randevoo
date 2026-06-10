# User Roles

## Purpose
Document roles discovered in code.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.Domain/Enums/UserRole.cs`
- `src/Randevoo.WebApi/Program.cs`
- `src/Randevoo.AdminPanel/Program.cs`

| Role | Evidence | Description |
| --- | --- | --- |
| Admin | Enum/policy/string search | Admin role detected; exact allowed actions are documented in the permissions matrix. |
| EventPlanner | Enum/policy/string search | EventPlanner role detected; exact allowed actions are documented in the permissions matrix. |
| Support | Enum/policy/string search | Support role detected; exact allowed actions are documented in the permissions matrix. |
| EndUser | Enum/policy/string search | EndUser role detected; exact allowed actions are documented in the permissions matrix. |
| PlatformSupportTeam | Enum/policy/string search | PlatformSupportTeam role detected; exact allowed actions are documented in the permissions matrix. |

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
