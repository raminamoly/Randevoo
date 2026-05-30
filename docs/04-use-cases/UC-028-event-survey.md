# UC-028 Event Survey

Includes UC-027 and UC-028.

## Goal

Collect 5-factor post-event survey for EventPlanner quality calculation.

## Actor

EndUser or Admin through `EndUserOnly` policy.

## Preconditions

- Event has ended.
- User has valid event ticket.

## Main Flow

1. User submits ratings and optional comment.
2. System validates all five factors and score range.
3. System creates or updates survey.
4. System recalculates EventPlanner profile metrics.
5. User can fetch own survey.

## Business Rules

- Survey is not used for matching.
- All five current factors are required.
- Score must be 1-5.
- Removed/refunded users cannot submit survey.

## APIs

API-028, API-029.

## Entities

`EventSurveyResponse`, `EventSurveyRating`, `DatingEvent`, `EventTicket`, `EventPlannerProfile`.
