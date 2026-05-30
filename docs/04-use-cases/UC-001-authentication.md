# UC-001 Authentication

Includes UC-001 through UC-004.

## Goal

Allow users to login without passwords using a mobile code and optionally confirm email.

## Actor

Anonymous user, authenticated user, user with confirmation link.

## Preconditions

- SMS sender is registered.
- Email sender is registered for email confirmation.
- JWT settings exist or fallback development values are used.

## Main Flow

1. User requests mobile login code.
2. System finds or creates `User`.
3. System hashes code, stores hash/expiry, sends SMS.
4. User submits mobile number and code.
5. System verifies hash and expiry.
6. System clears login code and returns JWT.
7. Authenticated user can request email confirmation.
8. User opens confirmation link and email becomes confirmed.

## Alternative Flows

- Invalid mobile number returns business-rule error.
- Wrong/expired login code returns business-rule error.
- Email confirmation without JWT returns unauthorized.
- Invalid/expired email token returns business-rule error.

## Business Rules

- No password is stored.
- Mobile number allows digits and optional leading plus.
- Confirmation email is lower-cased.

## APIs

API-001, API-002, API-003, API-004.

## Entities

`User`.
