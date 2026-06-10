# User Journeys

## Purpose
Document UI journeys supported by AdminPanel pages.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.AdminPanel/Pages/Account/Forbidden.cshtml`
- `src/Randevoo.AdminPanel/Pages/Account/Login.cshtml`
- `src/Randevoo.AdminPanel/Pages/Account/Logout.cshtml`
- `src/Randevoo.AdminPanel/Pages/Buyers/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Dashboard/Events.cshtml`
- `src/Randevoo.AdminPanel/Pages/Dashboard/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Dashboard/Money.cshtml`
- `src/Randevoo.AdminPanel/Pages/Dashboard/My.cshtml`
- `src/Randevoo.AdminPanel/Pages/Dashboard/Sales.cshtml`
- `src/Randevoo.AdminPanel/Pages/Dashboard/Users.cshtml`
- `src/Randevoo.AdminPanel/Pages/DiscountCodes/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Error.cshtml`
- `src/Randevoo.AdminPanel/Pages/EventTypes/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Buyers.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Conversation.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Conversations.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Details.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Edit.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Faqs.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/My.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Sms.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/SurveyRatings.cshtml`
- `src/Randevoo.AdminPanel/Pages/Finance/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Finance/My.cshtml`
- `src/Randevoo.AdminPanel/Pages/Finance/PaymentReceipts.cshtml`
- `src/Randevoo.AdminPanel/Pages/Finance/ReceivedReceipts.cshtml`
- `src/Randevoo.AdminPanel/Pages/Finance/TicketTransactions.cshtml`
- `src/Randevoo.AdminPanel/Pages/Finance/User.cshtml`
- `src/Randevoo.AdminPanel/Pages/Finance/Withdrawals.cshtml`

## Admin journey
Dashboard -> manage users/settings/event types/finance/moderation/support.

## Planner journey
Login -> planner profile -> event creation/editing -> participants/buyers/SMS -> received receipts -> finance.

## Support journey
Login -> support ticket queues -> ticket details/replies/status changes -> payment receipt review where authorized.

```mermaid
journey
  title AdminPanel operational journeys
  section Admin
    Login: 5: Admin
    Dashboard: 4: Admin
    Manage settings/users/events: 4: Admin
  section Planner
    Profile: 4: Planner
    Create event: 4: Planner
    Review participants and receipts: 3: Planner
  section Support
    Ticket queue: 4: Support
    Reply/update status: 4: Support
```

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
