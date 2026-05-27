---
applyTo: "src/**/*.cs"
---

# CQRS Standards

## Commands
- Commands modify state only
- Commands should be immutable
- One command = one business intention
- Validate commands before execution
- Keep command handlers transactional
- Avoid returning entities directly from commands

## Queries
- Queries must never modify state
- Use optimized read models
- Prefer projection over loading full aggregates
- Use AsNoTracking for readonly queries
- Paginate large datasets

## Handlers
- One handler per command/query
- Keep handlers small and focused
- Delegate business rules to Domain layer
- Avoid fat handlers with orchestration logic

## MediatR
- Use MediatR pipeline behaviors for:
  - Validation
  - Logging
  - Performance timing
  - Transactions
  - Authorization
- Avoid business logic inside pipeline behaviors

## Transactions
- Keep transaction scope minimal
- Avoid distributed transactions when possible
- Ensure consistency boundaries are explicit
