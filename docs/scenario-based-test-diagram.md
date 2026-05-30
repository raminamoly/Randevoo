# Scenario-Based Test Diagram

```mermaid
flowchart TD
    Start([Integration test starts]) --> MobileLogin[Login users with mobile code]

    MobileLogin --> PlannerProfile[Planner creates EventPlanner profile]
    PlannerProfile --> PlannerRelogin[Planner logs in again for EventPlanner JWT]
    PlannerRelogin --> CreateEvent[Planner creates DatingEvent]
    CreateEvent --> OpenEvent[Planner opens event for ticket sale]

    OpenEvent --> CreateUsers[Create EndUsers]
    CreateUsers --> FundBalances[Admin adjusts EndUser balances]
    FundBalances --> CreateProfiles[EndUsers create dating profiles]
    CreateProfiles --> BuyTickets[EndUsers buy tickets]

    BuyTickets --> ArchiveAssert[Assert events appear in user archive]
    BuyTickets --> EventStarted{Event start time reached?}
    EventStarted -- No --> ProfilesForbidden[Assert participant profiles are blocked]
    EventStarted -- Yes --> ProfilesVisible[Assert participant profiles are visible]

    ProfilesVisible --> StartChat[Start event conversation]
    StartChat --> SendMessage[Send chat message]
    SendMessage --> BlockUser[Block other participant]
    BlockUser --> BlockedMessage[Assert blocked message is rejected]

    StartChat --> ChatLimit[Try to exceed chat limit]
    ChatLimit --> ChatLimitAssert[Assert chat limit error]

    BuyTickets --> EventEnded{Event end time reached?}
    EventEnded -- No --> SurveyForbidden[Assert survey is blocked]
    EventEnded -- Yes --> SubmitSurvey[Submit 5-factor survey]
    SubmitSurvey --> SurveyAssert[Assert all 5 ratings saved]

    BuyTickets --> PlannerParticipants[Planner lists event participants]
    PlannerParticipants --> ParticipantAssert[Assert ticket and profile data visible]
    PlannerParticipants --> RemoveParticipant[Planner removes participant in emergency]
    RemoveParticipant --> RefundAssert[Assert ticket refunded and removed]
    RefundAssert --> RemovedAccessAssert[Assert removed user loses event access]

    RemovedAccessAssert --> Done([Scenario test complete])
```

## Main Test Scenarios

- Passwordless mobile login creates a valid JWT.
- EventPlanner profile upgrades the user role.
- EndUsers need profile and balance before buying tickets.
- Participant profile visibility starts only after event start time.
- Chat is limited by event settings.
- Blocked chat users cannot message each other.
- Surveys require all 5 factors and only work after event end time.
- Emergency participant removal refunds the ticket and removes access.
