# Authorization

## Purpose
Document policies and role access.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.WebApi/Program.cs`
- `src/Randevoo.AdminPanel/Program.cs`

Policies found include AdminOnly, EventPlannerOnly, EndUserOnly, SupportOrAdmin, AdminOrPlanner, and AdminPlannerOrSupport. Endpoint/page catalogs list source-level policy usage.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
