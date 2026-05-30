# Documentation Coverage Report

## APIs Documented

42 API entries documented in [API contracts](06-architecture/api-contracts.md):

- Authentication: 6
- Dating profiles: 5
- Event planner profile: 1
- Dating events: 9
- Event participants: 4
- Event chats: 4
- Event surveys: 2
- Event types: 3
- Balances: 3
- Moderation: 4
- Admin users: 1

## Entities Documented

16 entities documented in [Domain entities](03-domain/entities.md):

- User
- UserProfile
- Interest
- EventPlannerProfile
- DatingEvent
- EventTicket
- BalanceAccount
- BalanceTransaction
- EventConversation
- EventChatMessage
- EventChatBlock
- EventSurveyResponse
- EventSurveyRating
- EventType
- ModerationReport
- RefreshToken

Value objects documented:

- Location
- Coordinates
- AgeRange
- Height

## Use Cases Discovered

38 use case IDs mapped in [Use case map](04-use-cases/use-case-map.md), grouped into 11 focused use-case files.

## Tests Documented

- 2 unit test classes documented.
- 3 integration test classes documented.
- 11 integration test scenario IDs documented.
- No implemented E2E tests found.

## Missing / Unclear Areas

- Dating profile endpoints are currently anonymous and do not enforce ownership.
- `EventType` lookup exists but `DatingEvent` still stores a string event type.
- Domain events are collected but not dispatched.
- SMS/email implementations are console-only.
- No background jobs, scheduled tasks, SignalR hubs, message broker, or external APIs were found.
- No E2E tests exist.

## Generation Notes

Docs were generated from:

- `.csproj` package references
- `Program.cs`
- Minimal API endpoint files
- Domain entity files
- Repository interfaces/implementations
- `RandevooDbContext`
- Unit and integration tests
