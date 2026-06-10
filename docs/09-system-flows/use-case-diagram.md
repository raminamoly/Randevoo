# Use Case Diagram

## Purpose
Represent major actors and implemented use cases.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.WebApi/Endpoints`
- `src/Randevoo.AdminPanel/Pages`

```mermaid
flowchart LR
  Guest((Guest))
  User((End User))
  Planner((Event Planner))
  Support((Support))
  Admin((Admin))
  Guest --> Browse[Browse open events]
  User --> Auth[Authenticate]
  User --> Profile[Manage dating profile]
  User --> Buy[Buy/join event]
  User --> Chat[Like/chat after event]
  User --> Survey[Submit survey]
  User --> Report[Create moderation report]
  User --> Ticket[Create support ticket]
  Planner --> PlannerProfile[Manage planner profile]
  Planner --> Events[Create/manage events]
  Planner --> Participants[View participants/buyers]
  Support --> SupportQueue[Handle support tickets]
  Admin --> Users[Manage users and roles]
  Admin --> Finance[Manage finance/settings/moderation]
```
## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
