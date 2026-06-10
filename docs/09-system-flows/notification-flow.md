# Notification Flow

## Purpose
Document notification flow from current code evidence.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.Domain/Entities/SmsQueueItem.cs`
- `src/Randevoo.Infrastructure/Services/ConsoleSmsSender.cs`
- `src/Randevoo.Infrastructure/Services/ConsoleEmailSender.cs`

```mermaid
sequenceDiagram
  participant Handler
  participant SmsQueue
  participant SmsSender
  participant User
  Handler->>SmsQueue: create queue item or request
  Handler->>SmsSender: send message where implemented
  SmsSender-->>User: SMS/email/log output
```

## Gaps or uncertainties
- Production notification provider and background delivery worker were not confirmed.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
