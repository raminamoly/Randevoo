# UC-030 Event Types

Includes UC-029 and UC-030.

## Goal

Provide a lookup list of active event types and allow Admin management.

## Actor

Any caller for list; Admin for management.

## Main Flow

1. Caller lists active event types.
2. Admin creates event type.
3. Admin updates/deactivates event type.

## Business Rules

- Name length 2-100.
- Description max 500.
- List endpoint returns active types only.

## APIs

API-030, API-031, API-032.

## Entities

`EventType`.

## TODO

- `DatingEvent` does not reference `EventType.Id`; it still stores string `EventType`.
