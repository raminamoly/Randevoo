# UC-005 Dating Profile Management

Includes UC-005 through UC-008.

## Goal

Create, view, update, and soft-delete EndUser dating profiles.

## Actor

EndUser.

## Preconditions

- User exists for create.

## Main Flow

1. Create profile with display name, birth date, gender, location, and height.
2. View profile by profile id or user id.
3. Update profile display data, location, education, smoking, gender, and height.
4. Delete profile through soft delete.

## Alternative Flows

- Missing user returns not found.
- Duplicate display name returns business-rule error.
- Deleted profile is hidden by query filter.

## Business Rules

- Minimum age is 18.
- Display name is 2-50 chars and unique.
- User can only have one profile.

## APIs

API-005 through API-009.

## Entities

`User`, `UserProfile`, `Interest`, `Location`, `Height`.

## TODO

- Add endpoint authorization and ownership checks.
- Add API support for adding/removing interests.
