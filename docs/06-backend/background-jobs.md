# Background Jobs

## Purpose
Identify scheduled/hosted/background processing.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.Application`
- `src/Randevoo.Infrastructure`

No dedicated hosted service or background job scheduler was confirmed. SMS queue and notification abstractions exist, but delivery appears synchronous or console-backed in current infrastructure.

## Gaps or uncertainties
- Add hosted processing documentation if a queue worker is introduced.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
