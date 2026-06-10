# Logging Monitoring

## Purpose
Document logging/audit evidence.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.Domain/Entities/AuditLog.cs`
- `src/Randevoo.Infrastructure/Services/AuditLogger.cs`
- `src/Randevoo.WebApi/Middleware/ActivityLogMiddleware.cs`

Application logging, audit log entity/service, activity middleware, and correlation middleware exist. External monitoring/APM configuration needs verification.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
