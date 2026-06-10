# C4 Context Diagram

## Purpose
System context diagram.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.WebApi`
- `src/Randevoo.AdminPanel`

```mermaid
flowchart TD
  User[End User] --> Randevoo[Randevoo Platform]
  Planner[Event Planner] --> AdminPanel[AdminPanel]
  Support[Support Staff] --> AdminPanel
  Admin[Admin] --> AdminPanel
  AdminPanel --> WebApi[Web API]
  Randevoo --> WebApi
  WebApi --> Database[(Database)]
  WebApi --> Sms[SMS/Email Provider - console/current]
  WebApi --> Payment[Payment Gateway - Needs Verification]
```

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
