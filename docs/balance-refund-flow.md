# Balance and Refund Flow

```mermaid
flowchart TD
    Start([Balance account]) --> AdminAdjust[Admin adjustment]
    Start --> TicketBuy[Ticket purchase]
    Start --> EventCancel[Event cancellation refund]
    Start --> EmergencyRemove[Emergency participant removal refund]

    AdminAdjust --> AdminTransaction[AdminAdjustment transaction]
    TicketBuy --> Debit[Debit EndUser balance]
    Debit --> PurchaseTransaction[TicketPurchase transaction]
    Debit --> PlannerIncome[Credit planner income]
    PlannerIncome --> PlannerTransaction[EventPlannerIncome transaction]

    EventCancel --> RefundTicket[Mark tickets refunded]
    RefundTicket --> TicketRefund[TicketRefund transaction]

    EmergencyRemove --> RemoveTicket[Mark ticket removed and refunded]
    RemoveTicket --> DisableAccess[Disable event access and conversations]
    RemoveTicket --> EmergencyRefund[EmergencyRemovalRefund transaction]

    AdminTransaction --> History[Balance history]
    PurchaseTransaction --> History
    PlannerTransaction --> History
    TicketRefund --> History
    EmergencyRefund --> History

    History --> UserView[User views own balance history]
    History --> AdminView[Admin views any user balance history]
```

## Transaction Fields

- `Amount`
- `Type`
- `Description`
- `DatingEventId`
- `ReferenceType`
- `ReferenceId`
- `CreatedByUserId`
- `CreatedAt`

## Rules

- Balance cannot go negative.
- Every refund creates a transaction.
- Emergency removal refunds use `EmergencyRemovalRefund`, separate from normal `TicketRefund`.
- Admins can view any user balance history.
- EndUsers and EventPlanners can view their own balance history.
