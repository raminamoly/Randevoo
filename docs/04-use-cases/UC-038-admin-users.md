# UC-038 Admin User Role Management

## Goal

Allow Admin to change user roles.

## Actor

Admin.

## Main Flow

1. Admin submits new `UserRole`.
2. System loads user.
3. System updates role and saves user.

## Business Rules

- User must exist.
- Roles are `EndUser`, `EventPlanner`, `Admin`.

## APIs

API-040.

## Entities

`User`.
