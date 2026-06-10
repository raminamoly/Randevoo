# Payment Flow

## Purpose
Document payment flow from current code evidence.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.Domain/Entities/TicketOrder.cs`
- `src/Randevoo.Domain/Entities/OnlinePayment.cs`
- `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs`

```mermaid
sequenceDiagram
  actor User
  participant API
  participant Handler
  participant Finance as Payment/Receipt/Balance entities
  participant Admin as AdminPanel review pages
  User->>API: buy ticket or submit manual receipt
  API->>Handler: validate price/discount/currency
  Handler->>Finance: create TicketOrder/OnlinePayment/ManualPaymentReceipt
  Admin->>Finance: review receipt or inspect transactions
  Finance-->>User: payment/order state
```

## Gaps or uncertainties
- External payment gateway implementation needs verification.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
