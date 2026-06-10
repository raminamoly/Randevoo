# Privacy Model

## Purpose
Document privacy capabilities and concerns.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.WebApi/Endpoints/PrivacyEndpoints.cs`
- `src/Randevoo.Infrastructure/Services/PrivacyDataReader.cs`

Privacy export and delete endpoints exist under /api/privacy. The privacy model should be reviewed for completeness across profile photos, chat messages, support attachments, audit logs, and financial retention requirements.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
