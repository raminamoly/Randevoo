# Security Gaps

## Purpose
List security/privacy gaps found during extraction.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- Needs Verification: no direct source file found for this topic.

- Production SMS/email providers are not confirmed.
- Production payment gateway and webhook verification are not confirmed.
- Rate limiting and brute-force protection for auth code endpoints need verification.
- File upload validation/storage controls for photos/attachments need verification.
- Full privacy deletion/export coverage needs tests.
- AdminPanel CSRF/anti-forgery coverage should be verified per form.
- Audit retention, log scrubbing, and secret management policies are not fully documented in code.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
