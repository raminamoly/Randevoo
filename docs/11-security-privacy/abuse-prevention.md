# Abuse Prevention

## Purpose
Document abuse prevention mechanisms.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.Domain/Entities/ModerationReport.cs`
- `src/Randevoo.Domain/Entities/EventChatBlock.cs`

Current mechanisms include moderation reports, chat blocks, support tickets, audit logs, and admin review endpoints/pages.

## Gaps or uncertainties
- Rate limiting, spam controls, image moderation, and automated abuse detection were not confirmed.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
