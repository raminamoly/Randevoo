---
applyTo: "src/**/*.cs"
---

# Logging Standards

## Structured Logging
- Use structured logging with named properties
- Include correlation IDs where applicable
- Include contextual business information

## Security
- Never log:
  - Passwords
  - Tokens
  - Secrets
  - PII unless explicitly required

## Levels
- Trace: detailed diagnostics
- Debug: development troubleshooting
- Information: important business flow
- Warning: recoverable issues
- Error: failures requiring attention
- Critical: application/system failures

## Error Logging
- Log exceptions once at the appropriate boundary
- Include actionable context
- Avoid duplicate logging across layers

## Observability
- Log external service failures
- Log retry attempts
- Log performance bottlenecks
