# Seed Data

## Purpose
Document sample and lookup seed data.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.Infrastructure/Data/RandevooSampleDataSeeder.cs`

Seed data is managed by `RandevooSampleDataSeeder`. It appears to populate lookup/reference data and sample operational records for development/demo use.

## Gaps or uncertainties
- Review seeder before production deployment to ensure sample users, payment records, or operational data are not inserted in production.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
