# Use Case Map

| ID | Use Case | Actor | APIs |
|---|---|---|---|
| UC-001 | Request mobile login code | Anonymous | API-001 |
| UC-002 | Verify mobile login code | Anonymous | API-002 |
| UC-003 | Request email confirmation | Authenticated user | API-003 |
| UC-004 | Confirm email | User with link | API-004 |
| UC-005 | Create dating profile | EndUser | API-005 |
| UC-006 | View dating profile | Any caller | API-006, API-007 |
| UC-007 | Update dating profile | EndUser | API-008 |
| UC-008 | Delete dating profile | EndUser | API-009 |
| UC-009 | Upsert event planner profile | Authenticated user | API-010 |
| UC-010 | List open dating events | Any caller | API-011 |
| UC-011 | Create dating event | EventPlanner/Admin | API-012 |
| UC-012 | Open event sale | EventPlanner/Admin | API-013 |
| UC-013 | Close event sale | EventPlanner/Admin | API-014 |
| UC-014 | Cancel event and refund | EventPlanner/Admin | API-015 |
| UC-015 | Change event location | EventPlanner/Admin | API-016 |
| UC-016 | Change event commission | Admin | API-017 |
| UC-017 | Buy event ticket | EndUser/Admin | API-018 |
| UC-018 | Send SMS to participants | EventPlanner/Admin | API-019 |
| UC-019 | View own event archive | Authenticated user | API-020 |
| UC-020 | View participant profiles | Authenticated participant | API-021 |
| UC-021 | View event participants | EventPlanner/Admin | API-022 |
| UC-022 | Emergency remove participant | EventPlanner/Admin | API-023 |
| UC-023 | List my event conversations | EndUser/Admin | API-024 |
| UC-024 | Start event conversation | EndUser/Admin | API-025 |
| UC-025 | Send event chat message | EndUser/Admin | API-026 |
| UC-026 | Block event chat user | EndUser/Admin | API-027 |
| UC-027 | Get my event survey | EndUser/Admin | API-028 |
| UC-028 | Submit event survey | EndUser/Admin | API-029 |
| UC-029 | List event types | Any caller | API-030 |
| UC-030 | Manage event types | Admin | API-031, API-032 |
| UC-031 | View own balance | Authenticated user | API-033 |
| UC-032 | View user balance | Admin | API-034 |
| UC-033 | Adjust user balance | Admin | API-035 |
| UC-034 | Create moderation report | Authenticated user | API-036 |
| UC-035 | List my reports | Authenticated user | API-037 |
| UC-036 | List reports as admin | Admin | API-038 |
| UC-037 | Review moderation report | Admin | API-039 |
| UC-038 | Change user role | Admin | API-040 |

Detailed files:

- [UC-001-authentication.md](UC-001-authentication.md)
- [UC-005-dating-profile.md](UC-005-dating-profile.md)
- [UC-009-event-planner-profile.md](UC-009-event-planner-profile.md)
- [UC-011-dating-events.md](UC-011-dating-events.md)
- [UC-017-ticketing-participants.md](UC-017-ticketing-participants.md)
- [UC-024-event-chat.md](UC-024-event-chat.md)
- [UC-028-event-survey.md](UC-028-event-survey.md)
- [UC-030-event-types.md](UC-030-event-types.md)
- [UC-031-balances.md](UC-031-balances.md)
- [UC-034-moderation.md](UC-034-moderation.md)
- [UC-038-admin-users.md](UC-038-admin-users.md)
