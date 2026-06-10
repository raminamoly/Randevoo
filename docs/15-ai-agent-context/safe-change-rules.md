# Safe Change Rules

## Purpose
Rules to avoid damaging active user work.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- Needs Verification: no direct source file found for this topic.

- Check `git status` before edits.
- Stage only intended files.
- Do not revert unrelated changes.
- Keep migrations and snapshots consistent if schema changes are requested.
- For payment/auth/privacy/moderation changes, add targeted tests.
- Preserve docs links from `docs/00-index.md`.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
