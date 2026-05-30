# Aggregates

## Aggregate Roots

| Aggregate Root | Child Entities | Main Invariants |
|---|---|---|
| `User` | `UserProfile` through navigation | mobile number format, login code lifecycle, email confirmation lifecycle, one profile |
| `UserProfile` | interests | age >= 18, max 10 interests, no duplicate interest instance |
| `EventPlannerProfile` | none | title/resume length, non-negative metrics, rating 0-5 |
| `DatingEvent` | `EventTicket` | event end after start, planner role, capacity by gender, age ranges, sale/cancel state |
| `BalanceAccount` | `BalanceTransaction` | amount > 0, no negative balance |
| `EventConversation` | `EventChatMessage`, `EventChatBlock` | two different users, sender must be participant, disabled/blocked chats cannot send |
| `EventSurveyResponse` | `EventSurveyRating` | all 5 survey factors required, score 1-5 |
| `EventType` | none | name length, optional description length |
| `ModerationReport` | none | reporter cannot equal reported user, review cannot set status back to Pending |

## Cross-Aggregate Rules Enforced In Handlers

- Ticket purchase loads user, profile, event, buyer balance, and planner balance.
- Participant profiles require event start time and valid ticket.
- Chat start requires valid tickets for both users, event start time, and chat limit.
- Survey submission requires valid ticket and event end time.
- Emergency removal validates owner/admin, refunds ticket, disables conversations, creates moderation report.
- Moderation event reports validate both users belong to the event.

## Assumption Required

- Some rules are enforced in application handlers rather than domain aggregates because they require repository lookups across aggregates.
