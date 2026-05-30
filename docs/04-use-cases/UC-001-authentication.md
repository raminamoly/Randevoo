# UC-001 Authentication

Includes UC-001 through UC-004.

## Goal

Allow users to login without passwords using a mobile code, receive JWT/refresh tokens, rotate sessions, logout, and optionally confirm email.

## Actor

Anonymous user, authenticated user, user with confirmation link.

## Preconditions

- SMS sender is registered.
- Email sender is registered for email confirmation.
- JWT and refresh-token settings exist or fallback development values are used.

## Main Flow

1. User requests mobile login code.
2. System finds or creates `User`.
3. System checks SMS request limits, hashes code, stores hash/expiry, sends SMS.
4. User submits mobile number and code.
5. System verifies hash and expiry.
6. System clears login code and returns JWT plus refresh token.
7. Client can rotate the refresh token to receive a new JWT and refresh token.
8. Client can logout by revoking the refresh token.
9. Authenticated user can request email confirmation.
10. User opens confirmation link and email becomes confirmed.

## Alternative Flows

- Invalid mobile number returns business-rule error.
- Wrong/expired login code returns business-rule error.
- Too many SMS requests returns business-rule error.
- Too many wrong code attempts temporarily locks mobile login.
- Reused/revoked/expired refresh token returns business-rule error.
- Email confirmation without JWT returns unauthorized.
- Invalid/expired email token returns business-rule error.

## Business Rules

- No password is stored.
- This is not an anonymous app; protected product features require authenticated users.
- Mobile number allows digits and optional leading plus.
- SMS login code is one-time use and expires after 5 minutes.
- JWT access token expires after 15 minutes by default.
- Refresh token expires after 30 days by default and rotates on every use.
- Confirmation email is lower-cased.

## APIs

API-001, API-002, API-003, API-004, API-041, API-042.

## Entities

`User`, `RefreshToken`.
