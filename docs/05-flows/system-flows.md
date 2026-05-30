# System Flows

## FLOW-004 Ticket Purchase

```mermaid
sequenceDiagram
    actor User
    participant API
    participant Handler
    participant Event as DatingEvent
    participant BuyerBalance
    participant PlannerBalance
    participant Db

    User->>API: POST /api/dating-events/{id}/tickets
    API->>Handler: BuyDatingEventTicketCommand
    Handler->>Event: SellTicket(user, profile)
    Event-->>Handler: EventTicket
    Handler->>BuyerBalance: Debit ticket price
    Handler->>PlannerBalance: Credit planner income
    Handler->>Db: Save event and balances
    API-->>User: ticketId
```

## FLOW-005 Emergency Removal

```mermaid
sequenceDiagram
    actor Planner
    participant API
    participant Handler
    participant Ticket
    participant Balance
    participant Conversation
    participant Report
    participant Db

    Planner->>API: POST remove participant
    API->>Handler: RemoveEventParticipantCommand
    Handler->>Ticket: RemoveWithRefund()
    Handler->>Balance: Credit EmergencyRemovalRefund
    Handler->>Conversation: Disable participant conversations
    Handler->>Report: Create ActionTaken moderation report
    Handler->>Db: Save changes
    API-->>Planner: 204
```

## FLOW-006 Survey Updates Planner Quality

```mermaid
sequenceDiagram
    actor Participant
    participant API
    participant Handler
    participant Survey
    participant PlannerProfile
    participant Db

    Participant->>API: POST /api/event-surveys/events/{id}/me
    API->>Handler: SubmitEventSurveyCommand
    Handler->>Survey: Create or update ratings
    Handler->>Db: Save survey
    Handler->>PlannerProfile: UpdateMetrics()
    Handler->>Db: Save metrics
    API-->>Participant: EventSurveyDto
```
