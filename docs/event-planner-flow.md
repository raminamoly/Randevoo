# EventPlanner Flow

```mermaid
flowchart TD
    Start([Open app]) --> EnterMobile[Enter mobile number]
    EnterMobile --> RequestCode[Request login code]
    RequestCode --> ReceiveCode[Receive SMS code]
    ReceiveCode --> VerifyCode[Submit code]
    VerifyCode --> JwtIssued[JWT issued]

    JwtIssued --> HasPlannerProfile{Event planner profile completed?}
    HasPlannerProfile -- No --> CreatePlannerProfile[Create event planner profile]
    CreatePlannerProfile --> RoleUpdated[User role becomes EventPlanner]
    RoleUpdated --> ReLogin[Login again to get JWT with EventPlanner role]
    ReLogin --> PlannerDashboard[Open planner dashboard]
    HasPlannerProfile -- Yes --> PlannerDashboard

    PlannerDashboard --> BalancePage[View balance page]
    PlannerDashboard --> CreateEvent[Create dating event]

    CreateEvent --> EventDraft[Event created closed for sale]
    EventDraft --> UpdateLocation[Change address or location]
    EventDraft --> OpenSale[Open event for ticket sale]

    OpenSale --> UsersBuy[EndUsers buy tickets]
    UsersBuy --> CapacityTracking[Male and female capacity tracked]
    CapacityTracking --> ViewParticipants[View participant ticket and profile data]
    ViewParticipants --> EmergencyRemove[Remove participant in emergency]
    EmergencyRemove --> RefundTicket[Refund removed participant ticket]
    ViewParticipants --> SendSms[Send SMS to participants]

    OpenSale --> CloseSale[Close event for sale]
    OpenSale --> CancelEvent[Cancel event]
    CancelEvent --> RefundTickets[Refund participant tickets]

    CloseSale --> EventDay[Run event]
    SendSms --> EventDay
    EventDay --> AfterEvent[After event]
    AfterEvent --> ChatLimit[Participants can start limited chats]

    PlannerDashboard --> AdminChanges{Admin action needed?}
    AdminChanges -- Commission change --> AdminCommission[Admin updates event commission percent]
    AdminChanges -- Role change --> AdminRole[Admin changes user role]
```

## Main Permissions

- Can login with mobile number and SMS code.
- Can create and update EventPlanner profile.
- Can create dating events after planner profile exists.
- Can open, close, cancel, and change location for owned events.
- Can send SMS messages to participants of owned events.
- Can view own balance.
- Cannot change event commission percent unless Admin.
- Cannot manage other planners' events unless Admin.
