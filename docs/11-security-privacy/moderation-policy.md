# Moderation Policy

## Purpose
Document implemented moderation workflow.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.WebApi/Endpoints/ModerationEndpoints.cs`
- `src/Randevoo.Domain/Entities/ModerationReport.cs`

Users can create/list their moderation reports; admins can list and review reports. Report status/reason enums define current lifecycle vocabulary.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
