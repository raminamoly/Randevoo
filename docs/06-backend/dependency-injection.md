# Dependency Injection

## Purpose
Document DI setup and composition roots.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.WebApi/Program.cs`
- `src/Randevoo.AdminPanel/Program.cs`
- `src/Randevoo.Infrastructure/DependencyInjection.cs`

Composition occurs in WebApi and AdminPanel Program files, with Infrastructure exposing a dependency injection extension. Auth, authorization policies, DbContext/repositories, API clients, middleware, and Razor Pages/Minimal APIs are registered there.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
