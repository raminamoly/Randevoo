# ADR 003 Authentication Strategy

## Purpose
Record an architecture decision inferred from the current implementation.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- Needs Verification: no direct source file found for this topic.

Decision: use mobile/email verification, JWT bearer tokens for API, refresh tokens, and cookie auth for AdminPanel. Status: implemented; production hardening requires review.

## Consequences
- Developers should preserve the decision until product/architecture owners approve a new ADR.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
