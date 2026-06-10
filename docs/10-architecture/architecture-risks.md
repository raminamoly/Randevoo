# Architecture Risks

## Purpose
Highlight risks and suspicious areas.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.AdminPanel`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

- Large DbContext centralizes many configurations; schema changes need careful review.
- Payment/finance domain spans several entities and needs strong transactional tests.
- Notification providers are console-backed/currently partial.
- Matching appears event-like/conversation based rather than a standalone match aggregate; product language should stay precise.
- Dirty working tree during extraction means docs reflect current files, including uncommitted code changes.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
