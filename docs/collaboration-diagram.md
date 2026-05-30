# Collaboration Diagram

```mermaid
sequenceDiagram
    autonumber
    actor EndUser
    actor EventPlanner
    actor Admin
    participant WebApi
    participant MediatR
    participant CommandHandler
    participant Domain
    participant Repository
    participant Db as SQL Database
    participant Notification as SMS/Email Sender

    EndUser->>WebApi: POST /api/auth/mobile/request-code
    WebApi->>MediatR: RequestMobileLoginCodeCommand
    MediatR->>CommandHandler: Handle command
    CommandHandler->>Repository: Find or create User
    Repository->>Db: Save mobile login code hash
    CommandHandler->>Notification: Send login code

    EndUser->>WebApi: POST /api/auth/mobile/verify-code
    WebApi->>MediatR: VerifyMobileLoginCodeCommand
    MediatR->>CommandHandler: Handle command
    CommandHandler->>Domain: CompleteMobileLogin()
    CommandHandler->>Repository: Save User
    WebApi-->>EndUser: JWT

    EventPlanner->>WebApi: POST /api/dating-events
    WebApi->>MediatR: CreateDatingEventCommand
    MediatR->>CommandHandler: Handle command
    CommandHandler->>Repository: Load planner and profile
    CommandHandler->>Domain: new DatingEvent()
    CommandHandler->>Repository: Add DatingEvent
    Repository->>Db: Insert event
    WebApi-->>EventPlanner: Created event

    EndUser->>WebApi: POST /api/dating-events/{id}/tickets
    WebApi->>MediatR: BuyDatingEventTicketCommand
    MediatR->>CommandHandler: Handle command
    CommandHandler->>Repository: Load user, profile, balance, event
    CommandHandler->>Domain: SellTicket()
    CommandHandler->>Domain: Debit buyer balance
    CommandHandler->>Domain: Credit planner income
    Repository->>Db: Save ticket and balances
    WebApi-->>EndUser: Ticket id

    EndUser->>WebApi: POST /api/event-chats/events/{id}/conversations
    WebApi->>MediatR: StartEventConversationCommand
    MediatR->>CommandHandler: Handle command
    CommandHandler->>Repository: Validate tickets and chat count
    CommandHandler->>Domain: new EventConversation()
    Repository->>Db: Insert conversation
    WebApi-->>EndUser: Conversation

    EventPlanner->>WebApi: POST /api/event-participants/events/{id}/participants/{userId}/remove
    WebApi->>MediatR: RemoveEventParticipantCommand
    MediatR->>CommandHandler: Handle command
    CommandHandler->>Repository: Load event, ticket, balance
    CommandHandler->>Domain: RemoveWithRefund()
    CommandHandler->>Domain: Credit refund
    Repository->>Db: Save ticket and refund transaction
    WebApi-->>EventPlanner: NoContent

    Admin->>WebApi: PUT /api/admin/users/{id}/role
    WebApi->>MediatR: ChangeUserRoleCommand
    MediatR->>CommandHandler: Handle command
    CommandHandler->>Domain: ChangeUserRole()
    Repository->>Db: Save role
    WebApi-->>Admin: NoContent
```

## Collaboration Notes

- WebApi endpoints stay thin and delegate behavior to MediatR.
- Application handlers coordinate repositories and domain behavior.
- Domain entities enforce business rules.
- Infrastructure persists aggregates and sends notifications.
