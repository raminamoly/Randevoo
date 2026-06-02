# Randevoo Handoff

## Current Checkpoint

- Branch: `master`
- Latest pushed commit: `3f2c712`
- Repository: `https://github.com/raminamoly/Randevoo`
- Local workspace: `C:\_D\Randevoo`

## Verification

Last verified locally:

```powershell
dotnet test Randevoo.sln --no-restore
git diff --check
```

Notes:

- `dotnet test Randevoo.sln` may try to restore from NuGet. In restricted-network sessions, use `--no-restore` after packages are already restored.
- `git diff --check` reports only normal LF to CRLF warnings on Windows.
- SQL Server/Testcontainers relational smoke test is opt-in with `RUN_SQLSERVER_TESTCONTAINERS=true`.

## Architecture

The solution follows Clean Architecture / CQRS:

- `src/Randevoo.Domain`: entities, value objects, enums, domain events, repository contracts.
- `src/Randevoo.Application`: MediatR commands/queries, DTOs, interfaces.
- `src/Randevoo.Infrastructure`: EF Core, SQL Server, repositories, unit of work, JWT, notifications, privacy data reader.
- `src/Randevoo.WebApi`: minimal API endpoints, auth policies, Scalar/OpenAPI, SignalR hub.
- `tests/Randevoo.Tests.Unit`: domain tests.
- `tests/Randevoo.Tests.Integration`: API tests and optional SQL Server/Testcontainers smoke test.

## Recent Hardening

Implemented and pushed:

- Passwordless mobile auth with JWT plus rotating refresh tokens.
- SMS request throttling and failed-code lockout.
- Production fail-fast for missing SQL connection string and JWT secret.
- Authenticated dating profile APIs with owner/admin access checks.
- `/api/v1/...` alias for versioned API access.
- `DatingEvent.EventTypeId` foreign key to `EventType`.
- Chat-only SignalR hub at `/hubs/event-chat`.
- Privacy export and account deletion APIs.
- Domain event dispatch bridge through MediatR notifications.
- Filtering/cursor parameters for open events and moderation lists.
- Optional SQL Server/Testcontainers relational smoke test.
- Documentation and README updated.

## Important API Notes

Authentication:

- Mobile code request: `POST /api/auth/mobile/request-code`
- Mobile code verify: `POST /api/auth/mobile/verify-code`
- Refresh token: `POST /api/auth/refresh-token`
- Logout: `POST /api/auth/logout`
- Email confirmation request: `POST /api/auth/email/request-confirmation`
- Email confirmation: `GET /api/auth/email/confirm`

Dating profiles:

- All dating profile endpoints require authentication.
- Create uses the authenticated user id from JWT, not a request body `UserId`.
- Read/update/delete require owner or Admin.

Privacy:

- Export: `GET /api/privacy/me/export`
- Delete account: `DELETE /api/privacy/me`

Chat:

- HTTP chat endpoints still perform command handling.
- SignalR hub broadcasts `eventConversationUpdated` to both conversation users after a message is sent.

## Database

Current migrations include:

- `PasswordlessMobileAuth`
- `RolesBalancesAndDatingEvents`
- `EventParticipantsChatsAndSurveys`
- `SafetyModerationEventTypesPlannerQuality`
- `RefreshTokensAndAuthHardening`
- `DatingEventEventTypeForeignKey`

Development connection string is in:

```text
src/Randevoo.WebApi/appsettings.Development.json
```

Production must provide:

- `ConnectionStrings:DefaultConnection`
- `Jwt:Secret`

## Known Gaps / Next Work

High-value next steps:

- Replace console SMS/email senders with real providers.
- Add concrete domain event handlers where side effects are needed.
- Expand SQL Server/Testcontainers tests beyond the unique mobile-number smoke test.
- Add real API versioning package if version negotiation/deprecation is needed.
- Add E2E tests.
- Add SignalR client integration tests when chat frontend exists.
- Decide if account deletion should hard-delete related profile data or retain anonymized audit history.
- Add admin privacy/export support if required by operations.

## Coding Guidance

- Keep CQRS boundaries: commands mutate, queries read.
- Keep domain rules inside domain entities where possible.
- Keep endpoint handlers thin and delegate behavior to MediatR.
- Prefer adding focused tests for security/authorization regressions.
- Do not reintroduce production fallback secrets or machine-specific connection strings in `Program.cs`.
