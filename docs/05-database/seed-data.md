# Seed Data

## Purpose
Document database initialization behavior.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.Infrastructure/Data/RandevooDatabaseInitializer.cs`

Runtime sample data seeding is not part of the application startup. The database initializer only applies migrations or creates the database for non-relational providers, then ensures the initial admin user exists.

Operational data such as events, planners, participants, tickets, payments, and support tickets must be read from the database and must not be recreated from application seed code.

## Gaps or uncertainties
- Existing development databases may still contain older sample rows. Removing those rows is a separate database cleanup task.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
