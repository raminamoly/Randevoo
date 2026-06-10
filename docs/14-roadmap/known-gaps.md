# Known Gaps

## Purpose
Consolidate missing, partial, or uncertain areas.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- Needs Verification: no direct source file found for this topic.

- Matching is represented by event likes/conversations; no standalone Match entity was detected.
- Production payment gateway and webhook verification need confirmation.
- Production SMS/email providers and background notification workers need confirmation.
- CI/CD workflow was not confirmed.
- Docker support was not detected.
- API request/response examples require DTO-level verification before external publication.
- Full privacy export/delete coverage needs verification and tests.
- UI empty/loading/error/mobile states need visual verification.
- Rate limiting and abuse prevention beyond reports/blocks/support needs verification.
- Documentation was extracted from a dirty worktree, so some documented features may be uncommitted local changes.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
