# Domain Events

## Implemented Domain Event Types

| Event | Purpose | Raised By |
|---|---|---|
| `EntityCreatedEvent<T>` | Generic entity-created marker | `User`, `UserProfile`, `BalanceAccount`, `DatingEvent`, `EventConversation`, `EventPlannerProfile`, `EventSurveyResponse`, `EventType`, `ModerationReport` |
| `EntityUpdatedEvent<T>` | Generic field update marker | `User`, `UserProfile`, `EventPlannerProfile` |
| `EntitySoftDeletedEvent` | Generic soft-delete marker | `BaseEntity.SoftDelete()` |
| `EntityRestoredEvent` | Generic restore marker | `BaseEntity.Restore()` |
| `InterestAddedEvent` | User profile interest added | `UserProfile.AddInterest()` |
| `InterestRemovedEvent` | User profile interest removed | `UserProfile.RemoveInterest()` |

## Dispatching

Entities store domain events in `BaseEntity.DomainEvents`. No domain event dispatcher or outbox was found in the current implementation.

## TODO

- Decide whether domain events should be dispatched during `UnitOfWork.SaveChangesAsync`.
- Decide whether events should produce integration events or audit records.
