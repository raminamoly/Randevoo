# UC-017 Ticketing and Participants

Includes UC-017 through UC-022.

## Goal

Support ticket purchase, archive, participant visibility, participant management, and emergency removal.

## Actor

EndUser, EventPlanner, Admin.

## Preconditions

- Event must exist.
- Ticket purchase requires open event, user profile, and sufficient balance.
- Participant visibility requires event start time.

## Main Flow

1. EndUser buys ticket.
2. System validates profile, age range, gender capacity, and balance.
3. System debits buyer and credits planner income.
4. User sees event in archive.
5. After event start, participant can view other valid participant profiles.
6. Planner/Admin can view participants with ticket/profile data and mobile number for emergency calls.
7. Planner/Admin can remove participant in emergency.
8. System marks ticket removed/refunded, credits refund, disables conversations, and creates moderation audit report.

## Business Rules

- One ticket per user/event.
- Gender stored on ticket as purchase-time snapshot.
- Removed/refunded tickets lose participant profile/chat/survey access.
- Emergency removal requires reason.

## APIs

API-018, API-020 through API-023.

## Entities

`DatingEvent`, `EventTicket`, `UserProfile`, `BalanceAccount`, `BalanceTransaction`, `EventConversation`, `ModerationReport`.
