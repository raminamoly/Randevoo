# UX Findings

## Purpose
Document UI strengths, gaps, and risks from static inspection.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.AdminPanel/Pages/Account/Forbidden.cshtml`
- `src/Randevoo.AdminPanel/Pages/Account/Login.cshtml`
- `src/Randevoo.AdminPanel/Pages/Account/Logout.cshtml`
- `src/Randevoo.AdminPanel/Pages/Buyers/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Dashboard/Events.cshtml`
- `src/Randevoo.AdminPanel/Pages/Dashboard/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Dashboard/Money.cshtml`
- `src/Randevoo.AdminPanel/Pages/Dashboard/My.cshtml`
- `src/Randevoo.AdminPanel/Pages/Dashboard/Sales.cshtml`
- `src/Randevoo.AdminPanel/Pages/Dashboard/Users.cshtml`
- `src/Randevoo.AdminPanel/Pages/DiscountCodes/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Error.cshtml`
- `src/Randevoo.AdminPanel/Pages/EventTypes/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Buyers.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Conversation.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Conversations.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Details.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Edit.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Faqs.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/My.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/Sms.cshtml`
- `src/Randevoo.AdminPanel/Pages/Events/SurveyRatings.cshtml`
- `src/Randevoo.AdminPanel/Pages/Finance/Index.cshtml`
- `src/Randevoo.AdminPanel/Pages/Finance/My.cshtml`
- `src/Randevoo.AdminPanel/Pages/Finance/PaymentReceipts.cshtml`
- `src/Randevoo.AdminPanel/Pages/Finance/ReceivedReceipts.cshtml`
- `src/Randevoo.AdminPanel/Pages/Finance/TicketTransactions.cshtml`
- `src/Randevoo.AdminPanel/Pages/Finance/User.cshtml`
- `src/Randevoo.AdminPanel/Pages/Finance/Withdrawals.cshtml`

## Strengths
- Operational pages are grouped by Dashboard, Events, Finance, Support, Planner, Settings, Buyers, Participants, Users, and Profiles.
- Policy annotations and folder conventions make access intent visible.
- Tables/forms align with admin workflows.

## Issues and missing states
- Needs Verification: loading/empty/error states are not guaranteed across all pages.
- Needs Verification: mobile responsiveness must be checked visually.
- Needs Verification: destructive actions should have confirmation and audit messaging.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
