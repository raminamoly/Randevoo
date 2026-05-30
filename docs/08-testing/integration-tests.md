# Integration Tests

## IT-001 Auth Flow

File: `tests/Randevoo.Tests.Integration/AuthApiTests.cs`

Maps to: UC-001, UC-002, UC-003, UC-004.

Coverage:

- Request mobile login code.
- Verify mobile code and receive JWT.
- Request email confirmation with JWT.
- Confirm email through link.
- Unauthorized email confirmation request returns 401.

## IT-002 Dating Profile Flow

File: `tests/Randevoo.Tests.Integration/DatingProfileApiTests.cs`

Maps to: UC-005, UC-006, UC-007, UC-008.

Coverage:

- Create and get profile.
- Missing user returns 404.
- Update profile.
- Delete profile and hide deleted profile.

## IT-003 Event Ticket Purchase Flow

File: `tests/Randevoo.Tests.Integration/DatingEventApiTests.cs`

Maps to: UC-009, UC-011, UC-012, UC-015, UC-016, UC-017, UC-031, UC-033.

Coverage:

- Planner creates profile and event.
- Event opens for sale.
- Location and commission update.
- Admin funds buyer balance.
- Buyer creates profile and buys ticket.
- Buyer balance decreases.

## IT-004 EndUser Event Authorization

File: `tests/Randevoo.Tests.Integration/DatingEventApiTests.cs`

Maps to: UC-011.

Coverage:

- EndUser cannot create dating event.

## IT-005 Participant, Chat, Survey, Moderation, Removal Flow

File: `tests/Randevoo.Tests.Integration/DatingEventApiTests.cs`

Maps to: UC-019 through UC-028, UC-034 through UC-037.

Coverage:

- Archive shows ticketed event.
- Participant profiles visible after start.
- Chat can start and message.
- User can report another participant.
- Admin can list/review reports.
- User can block chat participant.
- Blocked messages fail.
- Chat limit is enforced.
- Survey can be submitted after event end.
- Planner quality metrics update.
- Planner participant list includes mobile number.
- Emergency removal blocks access and creates refund transaction.

## IT-006 Event Types

File: `tests/Randevoo.Tests.Integration/DatingEventApiTests.cs`

Maps to: UC-029.

Coverage:

- Seeded active event types are visible.

## IT-007 Admin Role Change

File: `tests/Randevoo.Tests.Integration/DatingEventApiTests.cs`

Maps to: UC-038.

Coverage:

- Admin changes a user role to EventPlanner.
