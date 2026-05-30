# UC-011 Dating Event Management

Includes UC-010 through UC-016 and UC-018.

## Goal

Create and manage dating events.

## Actor

EventPlanner or Admin. Listing open events is anonymous.

## Preconditions

- EventPlanner must have planner profile unless actor is Admin.
- Actor must own event unless actor is Admin for management actions.

## Main Flow

1. List open events.
2. EventPlanner creates event.
3. Event starts closed for sale.
4. Planner opens or closes sale.
5. Planner can change location/address.
6. Planner can send SMS to participants.
7. Planner can cancel event and refund tickets.
8. Admin can change commission percent.

## Business Rules

- Event end must be after start.
- Planner user must be EventPlanner or Admin.
- Cancelled events cannot be reopened.
- Commission is 0-100.
- Ticket price is 0.01-1,000,000.

## APIs

API-011 through API-017, API-019.

## Entities

`DatingEvent`, `EventTicket`, `User`, `EventPlannerProfile`, `Location`, `AgeRange`.
