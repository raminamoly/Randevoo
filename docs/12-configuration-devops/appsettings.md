# Appsettings

## Purpose
Document appsettings files without copying secret values.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.AdminPanel/appsettings.Development.json`
- `src/Randevoo.AdminPanel/appsettings.json`
- `src/Randevoo.WebApi/appsettings.Development.json`
- `src/Randevoo.WebApi/appsettings.Production.example.json`
- `src/Randevoo.WebApi/appsettings.json`

Appsettings files exist for WebApi and AdminPanel. Review keys locally, but do not publish real secrets. Connection strings, JWT settings, logging, CORS, and external provider settings must be environment-specific.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
