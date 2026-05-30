# Safety and Moderation Flow

```mermaid
flowchart TD
    Start([Safety issue happens]) --> Source{Where did it happen?}
    Source -- Event participant context --> EventReport[EndUser creates event report]
    Source -- Chat context --> ChatReport[EndUser creates chat report]
    Source -- Emergency at event --> PlannerRemoval[EventPlanner removes participant]

    EventReport --> Pending[Report status: Pending]
    ChatReport --> Pending
    PlannerRemoval --> Removed[Ticket marked removed and refunded]
    Removed --> DisableConversations[Disable removed user's event conversations]
    Removed --> AutoReport[Create ActionTaken moderation report]

    Pending --> AdminQueue[Admin reviews report queue]
    AutoReport --> AdminQueue

    AdminQueue --> ReviewDecision{Admin decision}
    ReviewDecision -- Reviewed --> Reviewed[Mark Reviewed with note]
    ReviewDecision -- Dismissed --> Dismissed[Mark Dismissed with note]
    ReviewDecision -- Action taken --> ActionTaken[Mark ActionTaken with note]

    Reviewed --> Audit[Review metadata saved]
    Dismissed --> Audit
    ActionTaken --> Audit

    Audit --> Done([Moderation trail complete])
```

## Privacy Rules

- EndUser public profile and chat responses do not expose `MobileNumber` or `Email`.
- EventPlanner participant-management responses can expose participant `MobileNumber` for emergency calls.
- Admin moderation and management responses can expose operational data when needed.
- `Email` stays hidden from participant/profile responses by default.

## Implemented Moderation Rules

- Users cannot report themselves.
- Event reports require both users to belong to the event.
- Chat reports require the reporter to belong to the conversation.
- Admins can list pending/reviewed/dismissed/action-taken reports.
- Admin review stores status, note, reviewer id, and review time.
- Emergency removal records refund/removal reason and creates an action-taken moderation report.
