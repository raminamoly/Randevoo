# UC-009 Event Planner Profile

## Goal

Allow authenticated users to create/update planner profile and become EventPlanner.

## Actor

Authenticated user.

## Preconditions

- User is authenticated.

## Main Flow

1. User submits title, optional picture URL, and resume.
2. System creates or updates `EventPlannerProfile`.
3. New profile changes role to `EventPlanner` unless user is `Admin`.
4. Response includes planner profile and metrics.

## Business Rules

- Title length 2-100.
- Picture URL max 500.
- Resume length 10-4000.
- Metrics are updated by survey submission.

## APIs

API-010.

## Entities

`User`, `EventPlannerProfile`, `EventSurveyResponse`.
