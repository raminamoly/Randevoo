# Logging Strategy

## Current Stack

Randevoo uses Serilog from the WebApi host for structured technical logs.

- Console sink for local and container logs
- Rolling file sink in Development at `src/Randevoo.WebApi/logs/randevoo-webapi-.log`
- Optional Seq sink in Development at `http://localhost:5341`
- Persistent `AuditLogs` table for sensitive business, security, privacy, and money actions

## Log Levels

| Level | Use |
|---|---|
| Debug/Trace | Local troubleshooting only |
| Information | Normal application and business events |
| Warning | Suspicious, sensitive, recoverable, or user-impacting events |
| Error | Failed operations and unhandled exceptions |
| Critical | Service unavailable, data corruption, or severe security incidents |

## Request Logging

Request logging records:

- HTTP method
- Path
- Status code
- Elapsed time
- `X-Correlation-ID` / trace identifier
- User ID and role when authenticated
- Client IP

Request and response bodies are not logged by default.

## Never Log

- OTP/mobile login codes
- JWT access tokens or refresh tokens
- Email confirmation tokens or full confirmation links
- Passwords or secrets
- Connection strings
- Authorization headers or cookies
- Payment card data
- Full request/response bodies by default

## Audit Logging

Audit logs are separate from Serilog because they are business records, not only diagnostics. They are append-only during normal operation.

Currently audited actions include:

- Admin role changes
- Admin balance adjustments
- Event cancellation with refund summary
- Emergency participant removal and refund
- Admin moderation report reviews
- Privacy data export
- Account deletion/anonymization

Audit fields:

- Actor user ID
- Action
- Target type and ID
- Before/after summary JSON when useful
- Reason
- IP address
- Correlation ID
- Created timestamp

## Local Seq

Run Seq locally:

```powershell
docker run --name seq -d --restart unless-stopped -e ACCEPT_EULA=Y -p 5341:80 datalust/seq
```

Then open:

```text
http://localhost:5341
```

Useful searches:

```text
Application = 'Randevoo.WebApi'
CorrelationId = 'your-correlation-id'
UserId = 123
Action = 'BalanceAdjusted'
```

## Production Recommendations

- Provide `Jwt:Secret` and `ConnectionStrings:DefaultConnection` through environment configuration or a secret manager.
- Keep console logs enabled for container/platform collection.
- Send structured logs to Seq, Loki, Application Insights, or another managed log backend.
- Retain audit logs according to legal/privacy policy.
- Restrict read access to audit logs because they contain sensitive operational metadata.
- Alert on repeated failed logins, unusual admin balance adjustments, moderation spikes, and high 5xx rates.

## Troubleshooting

For user-facing errors, ask for the `traceId` returned in the error response or the `X-Correlation-ID` request header, then search logs by `CorrelationId`.

For admin/money/security disputes, search the `AuditLogs` table first, then use the correlation ID to inspect technical logs around the same request.
