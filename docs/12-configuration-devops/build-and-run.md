# Build And Run

## Purpose
Document build/run workflow.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `Randevoo.sln`

Use `dotnet build Randevoo.sln` for compilation and `dotnet test Randevoo.sln` for tests. Run WebApi and AdminPanel as separate ASP.NET Core projects during local development.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
