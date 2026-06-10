# Recommended Next Steps

## Purpose
Prioritized recommendations.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- Needs Verification: no direct source file found for this topic.

1. Add API contract tests for all Minimal API groups.
2. Add financial/payment integration tests around TicketOrder, OnlinePayment, ManualPaymentReceipt, BalanceTransaction.
3. Add auth rate limiting and verification-code abuse tests.
4. Add production SMS/email/payment provider documentation and implementations.
5. Add CI to run build, tests, and markdown/mermaid validation.
6. Add Playwright smoke tests for AdminPanel critical workflows.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
