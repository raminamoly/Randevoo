# Post-Ticket Participant, Chat, and Survey Flow

```mermaid
flowchart TD
    TicketBought[EndUser buys event ticket] --> Archive[Event appears in user's archive]
    TicketBought --> WaitStart{Event start time reached?}

    WaitStart -- No --> HiddenProfiles[Participant profiles are hidden]
    WaitStart -- Yes --> ShowProfiles[User can view other valid participant profiles]

    ShowProfiles --> StartChat[Start conversation with participant]
    StartChat --> ChatLimit{Within event chat limit?}
    ChatLimit -- No --> LimitError[Show chat limit error]
    ChatLimit -- Yes --> Conversation[Conversation created]

    Conversation --> SendMessage[Send chat messages]
    Conversation --> BlockUser[Block participant]
    BlockUser --> Blocked[Blocked users cannot message each other]

    TicketBought --> PlannerView[EventPlanner views participants]
    PlannerView --> FullData[Planner sees ticket and profile data]
    PlannerView --> EmergencyRemove[Planner removes participant in emergency]
    EmergencyRemove --> Refund[Ticket marked refunded and removed]
    Refund --> NoAccess[Removed user loses profile and chat access for event]

    TicketBought --> WaitEnd{Event ended?}
    WaitEnd -- No --> SurveyHidden[Survey cannot be submitted yet]
    WaitEnd -- Yes --> Survey[Submit 5-factor survey]
    Survey --> Ratings[Store extensible rating rows]
```

## Implemented Rules

- EndUsers can view their event archive after buying tickets.
- EndUsers can see other participant profiles only after the event start time.
- Removed or refunded tickets cannot access participant profiles, event chat, or survey submission.
- Chat conversations are event-scoped.
- Chat connections are limited by `DatingEvent.NumberOfChatAllowed`.
- Users can block each other inside a conversation.
- Blocked users cannot continue sending messages to each other.
- Surveys are available after event end time.
- Surveys require all 5 current rating factors:
  - Overall experience
  - Event organization
  - Venue and location
  - Participant quality
  - Safety and comfort
- EventPlanners can view participants for their own events.
- EventPlanners can remove a participant in an emergency, refund the ticket, and record the reason.
- Admins can manage all events and participants.
