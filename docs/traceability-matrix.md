# Traceability Matrix

| Requirement | Use Case | API | Entity | Integration Test | E2E Test |
|---|---|---|---|---|---|
| FR-001 Passwordless mobile login | UC-001, UC-002 | API-001, API-002 | User | IT-001 | E2E-001 proposed |
| FR-002 Email confirmation | UC-003, UC-004 | API-003, API-004 | User | IT-001 | E2E-001 proposed |
| FR-003 Refresh-token sessions | UC-001 | API-041, API-042 | RefreshToken | IT-001 | E2E-001 proposed |
| FR-004 Dating profile management | UC-005 to UC-008 | API-005 to API-009 | UserProfile, Interest | IT-002 | E2E-001 proposed |
| FR-005 Planner profile management | UC-009 | API-010 | EventPlannerProfile, User | IT-003 | E2E-001 proposed |
| FR-006 Event browsing and management | UC-010 to UC-016 | API-011 to API-017 | DatingEvent | IT-003, IT-004 | E2E-001 proposed |
| FR-007 Ticket purchase | UC-017 | API-018 | EventTicket, BalanceAccount | IT-003 | E2E-001 proposed |
| FR-008 Participant SMS | UC-018 | API-019 | EventTicket, User | Not directly asserted | E2E-002 proposed |
| FR-009 Event archive | UC-019 | API-020 | EventTicket | IT-005 | E2E-001 proposed |
| FR-010 Participant profile visibility | UC-020 | API-021 | EventTicket, UserProfile | IT-005 | E2E-001 proposed |
| FR-011 Planner participant management | UC-021, UC-022 | API-022, API-023 | EventTicket, ModerationReport | IT-005 | E2E-002 proposed |
| FR-012 Event chat | UC-023 to UC-026 | API-024 to API-027 | EventConversation, EventChatMessage, EventChatBlock | IT-005 | E2E-001 proposed |
| FR-013 Event survey and planner quality | UC-027, UC-028 | API-028, API-029 | EventSurveyResponse, EventPlannerProfile | IT-005 | E2E-001 proposed |
| FR-014 Event type lookup/admin | UC-029, UC-030 | API-030 to API-032 | EventType | IT-006 | Not defined |
| FR-015 Balance history/admin adjustment | UC-031 to UC-033 | API-033 to API-035 | BalanceAccount, BalanceTransaction | IT-003, IT-005 | E2E-001 proposed |
| FR-016 Moderation reports | UC-034 to UC-037 | API-036 to API-039 | ModerationReport | IT-005 | E2E-002 proposed |
| FR-017 Admin user roles | UC-038 | API-040 | User | IT-007 | Not defined |

## Missing Areas

- No E2E tests implemented.
- No integration test directly verifies `SendSmsToParticipants` side effects.
- No integration test covers admin create/update event type.
- No integration test covers admin balance lookup endpoint.
- Expand SQL Server/Testcontainers coverage beyond the current unique-index smoke test.
