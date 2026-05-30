# UC-031 Balances

Includes UC-031 through UC-033.

## Goal

Track user balances and transaction history.

## Actor

Authenticated user, Admin.

## Main Flow

1. User views own balance.
2. Admin views any user balance.
3. Admin adjusts a user balance.
4. Ticket purchase debits buyer and credits planner income.
5. Event cancellation or emergency removal creates refund transactions.

## Business Rules

- Balance account is created lazily.
- Amount must be positive.
- Debits cannot make balance negative.
- Emergency removal refund has distinct type `EmergencyRemovalRefund`.

## APIs

API-033, API-034, API-035.

## Entities

`BalanceAccount`, `BalanceTransaction`, `User`, `DatingEvent`, `EventTicket`.
