# Error Handling

## Purpose
Document error handling conventions.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.WebApi/Middleware/GlobalExceptionMiddleware.cs`
- `src/Randevoo.WebApi/Endpoints/EndpointHelpers.cs`

Errors are centralized by GlobalExceptionMiddleware and endpoint helper code. Domain exceptions and validation failures should map to consistent HTTP status codes.

## Gaps or uncertainties
- Full error status catalog requires endpoint-by-endpoint runtime verification.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
