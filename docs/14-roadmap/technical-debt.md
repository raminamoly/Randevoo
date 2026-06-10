# Technical Debt

## Purpose
List technical debt risks.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- Needs Verification: no direct source file found for this topic.

- Large DbContext/schema surface area increases migration risk.
- Payment/finance domain needs high-confidence tests.
- AdminPanel pages should consistently expose empty/error states and anti-forgery protection.
- Notification and payment abstractions need production adapters.
- Authorization matrix should be executable tests, not only policies in code.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
