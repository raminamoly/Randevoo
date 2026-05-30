# Admin User Flow

```mermaid
flowchart TD
    Start([Open app]) --> EnterMobile[Enter mobile number]
    EnterMobile --> RequestCode[Request login code]
    RequestCode --> ReceiveCode[Receive SMS code]
    ReceiveCode --> VerifyCode[Submit code]
    VerifyCode --> JwtIssued[JWT issued with Admin role]

    JwtIssued --> AdminDashboard[Open admin dashboard]

    AdminDashboard --> ManageUsers[Manage users]
    ManageUsers --> ChangeRole[Change user role]
    ChangeRole --> EndUserRole[Set role to EndUser]
    ChangeRole --> PlannerRole[Set role to EventPlanner]
    ChangeRole --> AdminRole[Set role to Admin]

    AdminDashboard --> ManageBalances[Manage balances]
    ManageBalances --> SelectUserBalance[Select user balance]
    SelectUserBalance --> AdjustBalance[Credit or debit balance]
    AdjustBalance --> BalanceTransaction[Balance transaction recorded]

    AdminDashboard --> ManageEvents[Manage dating events]
    ManageEvents --> CreateEvent[Create event as admin]
    ManageEvents --> OpenEvent[Open event for sale]
    ManageEvents --> CloseEvent[Close event for sale]
    ManageEvents --> ChangeLocation[Change event location or address]
    ManageEvents --> ChangeCommission[Change event planner commission percent]
    ManageEvents --> SendSms[Send SMS to participants]
    ManageEvents --> CancelEvent[Cancel event]
    CancelEvent --> RefundTickets[Refund participant tickets]
    ManageEvents --> ManageParticipants[View and remove event participants]
    ManageParticipants --> EmergencyRefund[Refund removed participant]
    AdminDashboard --> MonitorChats[Monitor chat and survey data when needed]

    AdminDashboard --> MonitorSystem[Monitor platform]
    MonitorSystem --> ReviewUsers[Review users and profiles]
    MonitorSystem --> ReviewEvents[Review events and ticket activity]
    MonitorSystem --> ReviewMoney[Review balances and transactions]
```

## Main Permissions

- Can login with mobile number and SMS code.
- Can change user roles.
- Can adjust user balances.
- Can create and manage dating events.
- Can change event commission percent.
- Can open, close, cancel, and refund events.
- Can send SMS messages to event participants.
- Can act across users and planners.
