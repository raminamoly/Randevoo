# Security Overview

## Purpose
Summarize security model and risk areas.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.WebApi/Program.cs`
- `src/Randevoo.AdminPanel/Program.cs`

Security uses JWT bearer auth for API, cookie auth for AdminPanel, policy-based authorization, refresh tokens, verification codes, audit logging, moderation reports, and privacy export/delete endpoints. Dating-app sensitive data includes profile data, gender/preferences, photos, location/city, chat messages, reports, support content, and payment records.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
