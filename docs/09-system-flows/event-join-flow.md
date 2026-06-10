# Event Join Flow

## Purpose
Document event join flow from current code evidence.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.WebApi/Endpoints/DatingEventEndpoints.cs`
- `src/Randevoo.Application/Features/DatingEvents/Commands/BuyDatingEventTicket/BuyDatingEventTicketHandler.cs`

```mermaid
sequenceDiagram
  actor User
  participant API as DatingEventEndpoints
  participant Handler as BuyDatingEventTicketHandler
  participant Payment as Payment/Balance records
  participant Db
  User->>API: POST /api/dating-events/{id}/tickets
  API->>Handler: BuyDatingEventTicketCommand
  Handler->>Payment: create order/payment/balance records
  Handler->>Db: persist ticket/order state
  Db-->>Handler: saved
  Handler-->>API: purchase result
  API-->>User: ticket/order response
```

## Gaps or uncertainties
- No additional gaps beyond the repository evidence listed here.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
