---
applyTo: "src/**/*.cs"
---

# Domain Events Standards

## General
- Use domain events for meaningful business events
- Domain events represent facts that already happened
- Keep events immutable
- Use past-tense naming

## Examples
- UserRegisteredEvent
- InvoicePaidEvent
- SubscriptionExpiredEvent

## Rules
- Raise events from aggregate roots
- Do not trigger infrastructure concerns directly from entities
- Avoid synchronous side effects for expensive operations
- Keep event payloads minimal and focused

## Handlers
- Event handlers should:
  - Be isolated
  - Be idempotent
  - Handle retries safely
- Avoid long-running logic inside handlers

## Integration
- Distinguish between:
  - Domain Events
  - Integration Events
- Integration events are infrastructure concerns
