# Validation And Error Handling

## Purpose
Document validation and error response approach.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.WebApi/Middleware/GlobalExceptionMiddleware.cs`
- `src/Randevoo.Domain/Common/GuardAgainst.cs`

Global exception middleware is present. Domain guard helpers and feature handlers provide validation/business rule checks. Minimal API endpoints return typed results and errors according to each handler path.

## Gaps or uncertainties
- A complete standardized problem-details contract should be verified in middleware and endpoint helpers.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
