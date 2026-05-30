# EndUser Flow

```mermaid
flowchart TD
    Start([Open app]) --> EnterMobile[Enter mobile number]
    EnterMobile --> RequestCode[Request login code]
    RequestCode --> ReceiveCode[Receive SMS code]
    ReceiveCode --> VerifyCode[Submit code]
    VerifyCode --> JwtIssued[JWT issued]

    JwtIssued --> HasProfile{Dating profile completed?}
    HasProfile -- No --> CreateProfile[Create dating profile]
    CreateProfile --> BrowseEvents
    HasProfile -- Yes --> BrowseEvents[Browse open dating events]

    BrowseEvents --> SelectEvent[Select event]
    SelectEvent --> CheckRules{Meets event rules?}
    CheckRules -- No --> CannotBuy[Show validation error]
    CheckRules -- Yes --> CheckBalance{Enough balance?}

    CheckBalance -- No --> BalancePage[Open balance page]
    BalancePage --> AddFunds[Balance adjusted by admin/payment flow]
    AddFunds --> SelectEvent

    CheckBalance -- Yes --> BuyTicket[Buy ticket]
    BuyTicket --> DeductBalance[Deduct ticket price from balance]
    DeductBalance --> TicketCreated[Ticket created]

    TicketCreated --> EventDay[Attend event]
    TicketCreated --> Archive[See event in archive]
    EventDay --> Profiles[See other participant profiles after event start]
    Profiles --> ChatLimit[Start chats up to event limit]
    ChatLimit --> Conversation[Send messages in conversations]
    Conversation --> BlockUser[Block another participant if needed]
    EventDay --> AfterEvent[After event]
    AfterEvent --> Survey[Fill 5-factor event survey]

    JwtIssued --> EmailOptional{Wants email confirmation?}
    EmailOptional -- Yes --> EnterEmail[Enter email]
    EnterEmail --> SendEmailLink[Send confirmation link]
    SendEmailLink --> ConfirmEmail[Open confirmation link]
    ConfirmEmail --> EmailConfirmed[Email confirmed]
    EmailOptional -- No --> HasProfile
```

## Main Permissions

- Can login with mobile number and SMS code.
- Can complete and update dating profile.
- Can view own balance.
- Can buy tickets for open events when profile, age, gender capacity, and balance rules pass.
- Cannot create, open, close, cancel, or manage dating events.
