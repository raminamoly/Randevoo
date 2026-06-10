# Documentation Extraction Report

## Purpose
Record extraction metadata, scope, and findings.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `Randevoo.sln`

## Date/time of extraction
2026-06-10 Asia/Tehran (local environment date).

## Repository name
Randevoo

## Branch name
codex/zodiac-admin-profiles

## Commit hash before documentation
2e2203175de62a6a0e50abd4a893e94089d54845

## Summary of analyzed areas
Solution/projects, Domain, Application, Infrastructure, WebApi endpoints/middleware/hubs, AdminPanel pages/API clients, EF Core DbContext/migrations/seed data, configuration files, tests, security/privacy, DevOps, and current gaps.

## Files/folders created
- docs/00-index.md
- docs/01-repository-analysis/repository-map.md
- docs/01-repository-analysis/solution-structure.md
- docs/01-repository-analysis/project-dependencies.md
- docs/01-repository-analysis/package-inventory.md
- docs/01-repository-analysis/source-code-inventory.md
- docs/02-product-overview/product-vision.md
- docs/02-product-overview/problem-solution.md
- docs/02-product-overview/personas.md
- docs/02-product-overview/user-roles.md
- docs/02-product-overview/glossary.md
- docs/02-product-overview/current-feature-map.md
- docs/03-requirements/functional-requirements.md
- docs/03-requirements/non-functional-requirements.md
- docs/03-requirements/user-stories.md
- docs/03-requirements/business-rules.md
- docs/03-requirements/permissions-matrix.md
- docs/04-domain-analysis/domain-overview.md
- docs/04-domain-analysis/entity-catalog.md
- docs/04-domain-analysis/aggregate-boundaries.md
- docs/04-domain-analysis/value-objects.md
- docs/04-domain-analysis/enums.md
- docs/04-domain-analysis/domain-events.md
- docs/04-domain-analysis/business-invariants.md
- docs/04-domain-analysis/state-machines.md
- docs/05-database/database-overview.md
- docs/05-database/dbcontext-analysis.md
- docs/05-database/tables-and-fields.md
- docs/05-database/relationships.md
- docs/05-database/indexes-and-constraints.md
- docs/05-database/migrations.md
- docs/05-database/seed-data.md
- docs/05-database/erd.md
- docs/06-backend/backend-overview.md
- docs/06-backend/application-layer.md
- docs/06-backend/domain-layer.md
- docs/06-backend/infrastructure-layer.md
- docs/06-backend/webapi-layer.md
- docs/06-backend/dependency-injection.md
- docs/06-backend/background-jobs.md
- docs/06-backend/validation-and-error-handling.md
- docs/07-api/api-overview.md
- docs/07-api/endpoints-catalog.md
- docs/07-api/request-response-examples.md
- docs/07-api/authentication-and-authorization.md
- docs/07-api/error-handling.md
- docs/08-ui-ux/ui-overview.md
- docs/08-ui-ux/sitemap.md
- docs/08-ui-ux/screens-catalog.md
- docs/08-ui-ux/components-catalog.md
- docs/08-ui-ux/forms-and-fields.md
- docs/08-ui-ux/user-journeys.md
- docs/08-ui-ux/admin-panel.md
- docs/08-ui-ux/ux-findings.md
- docs/09-system-flows/use-case-diagram.md
- docs/09-system-flows/registration-flow.md
- docs/09-system-flows/profile-flow.md
- docs/09-system-flows/event-discovery-flow.md
- docs/09-system-flows/event-creation-flow.md
- docs/09-system-flows/event-join-flow.md
- docs/09-system-flows/matching-flow.md
- docs/09-system-flows/messaging-flow.md
- docs/09-system-flows/payment-flow.md
- docs/09-system-flows/moderation-flow.md
- docs/09-system-flows/notification-flow.md
- docs/10-architecture/architecture-overview.md
- docs/10-architecture/c4-context-diagram.md
- docs/10-architecture/c4-container-diagram.md
- docs/10-architecture/c4-component-diagram.md
- docs/10-architecture/deployment-diagram.md
- docs/10-architecture/clean-architecture-boundaries.md
- docs/10-architecture/dependency-rules.md
- docs/10-architecture/architecture-risks.md
- docs/10-architecture/architecture-decisions/adr-001-project-structure.md
- docs/10-architecture/architecture-decisions/adr-002-database-strategy.md
- docs/10-architecture/architecture-decisions/adr-003-authentication-strategy.md
- docs/10-architecture/architecture-decisions/adr-004-event-matching-strategy.md
- docs/10-architecture/architecture-decisions/adr-005-ui-architecture.md
- docs/11-security-privacy/security-overview.md
- docs/11-security-privacy/authentication.md
- docs/11-security-privacy/authorization.md
- docs/11-security-privacy/sensitive-data.md
- docs/11-security-privacy/privacy-model.md
- docs/11-security-privacy/abuse-prevention.md
- docs/11-security-privacy/moderation-policy.md
- docs/11-security-privacy/security-gaps.md
- docs/12-configuration-devops/configuration-overview.md
- docs/12-configuration-devops/appsettings.md
- docs/12-configuration-devops/environment-variables.md
- docs/12-configuration-devops/local-development.md
- docs/12-configuration-devops/build-and-run.md
- docs/12-configuration-devops/database-migrations.md
- docs/12-configuration-devops/iis-deployment.md
- docs/12-configuration-devops/docker.md
- docs/12-configuration-devops/ci-cd.md
- docs/12-configuration-devops/logging-monitoring.md
- docs/13-testing/testing-overview.md
- docs/13-testing/existing-tests.md
- docs/13-testing/test-coverage-summary.md
- docs/13-testing/recommended-test-scenarios.md
- docs/13-testing/unit-tests.md
- docs/13-testing/integration-tests.md
- docs/13-testing/api-tests.md
- docs/13-testing/ui-tests.md
- docs/14-roadmap/current-state.md
- docs/14-roadmap/known-gaps.md
- docs/14-roadmap/technical-debt.md
- docs/14-roadmap/recommended-next-steps.md
- docs/14-roadmap/future-features.md
- docs/15-ai-agent-context/ai-coding-guidelines.md
- docs/15-ai-agent-context/safe-change-rules.md
- docs/15-ai-agent-context/repository-context-for-future-agents.md
- docs/15-ai-agent-context/documentation-extraction-report.md

## Main entities discovered
AuditLog, BalanceAccount, BalanceTransaction, BalanceTransactionTypeLookup, City, Country, CurrencyExchangeRate, CurrencyLookup, DatingEvent, EducationLevelLookup, EventChatBlock, EventChatMessage, EventConversation, EventDiscountCode, EventDiscountTypeLookup, EventFaq, EventLike, EventModeLookup, EventParticipantSmsRequest, EventPlannerProfile, EventReviewStatusLookup, EventSurveyRating, EventSurveyResponse, EventTag, EventTicket, EventType, GenderLookup, Interest, ManualPaymentReceipt, ModerationReport, OnlineEventPlatform, OnlinePayment, PermissionAction, PlannerBankAccount, PlannerWithdrawalRequest, RefreshToken, RoleOperationPermission, SmsQueueItem, SupportTicket, SupportTicketAssignmentCursor, SupportTicketAttachment, SupportTicketCategoryLookup, SupportTicketHistoryEntry, SupportTicketMessage, SupportTicketRecipientTypeLookup, SupportTicketStatusLookup, Tag, TicketOrder, User, UserOperationPermissionOverride, UserProfile, UserProfileImage, UserRoleLookup, ZodiacSignLookup

## Main tables discovered
Users, UserProfiles, UserProfileImages, Interests, EventPlannerProfiles, Countries, Cities, EducationLevels, Genders, ZodiacSigns, UserRoles, EventReviewStatuses, EventDiscountTypes, BalanceTransactionTypes, Currencies, CurrencyExchangeRates, BalanceAccounts, BalanceTransactions, OnlinePayments, ManualPaymentReceipts, PlannerWithdrawalRequests, PlannerBankAccounts, DatingEvents, TicketOrders, EventModes, OnlineEventPlatforms, EventFaqs, EventDiscountCodes, Tags, EventTags, EventTickets, EventLikes, EventConversations, EventChatMessages, EventChatBlocks, EventSurveyResponses, EventSurveyRatings, EventTypes, ModerationReports, SupportTickets, SupportTicketMessages, SupportTicketAttachments, SupportTicketHistoryEntries, SupportTicketAssignmentCursors, SupportTicketStatuses, SupportTicketCategories, SupportTicketRecipientTypes, EventParticipantSmsRequests, SmsQueueItems, RefreshTokens, AuditLogs, PermissionActions, RoleOperationPermissions, UserOperationPermissionOverrides

## Main user roles discovered
Admin, EventPlanner, Support, EndUser, PlatformSupportTeam

## Main UI screens discovered
/Account/Forbidden, /Account/Login, /Account/Logout, /Buyers, /Dashboard/Events, /Dashboard, /Dashboard/Money, /Dashboard/My, /Dashboard/Sales, /Dashboard/Users, /DiscountCodes, /Error, /EventTypes, /Events/Buyers, /Events/Conversation, /Events/Conversations, /Events/Details, /Events/Edit, /Events/Faqs, /Events, /Events/My, /Events/Sms, /Events/SurveyRatings, /Finance, /Finance/My, /Finance/PaymentReceipts, /Finance/ReceivedReceipts, /Finance/TicketTransactions, /Finance/User, /Finance/Withdrawals, /, /Logs, /Logs/SmsQueue, /Participants, /Planner/Approvals, /Planner/BankAccounts, /Planner/Details, /Planner, /Planner/Profile, /Planner/Review, /Privacy, /Public/Event, /Settings, /Settings/OperationPermissions, /Shared/_DashboardRangeFilter, /Shared/_EventImageSlider, /Shared/_Layout, /Shared/_SidebarNav, /Shared/_Topbar, /Shared/_ValidationScriptsPartial, /Support/Create, /Support/Details, /Support, /Support/My, /Support/Received, /Support/Tickets, /Tags, /UserProfiles/AdminEdit, /UserProfiles/Details, /UserProfiles, /Users, /_ViewImports, /_ViewStart

## Main APIs discovered
POST /api/auth/mobile/request-code, POST /api/auth/mobile/verify-code, POST /api/auth/refresh-token, POST /api/auth/logout, POST /api/auth/email/request-confirmation, GET /api/auth/email/confirm, GET /api/balances/me, GET /api/balances/{userId:long}, POST /api/balances/{userId:long}/adjust, GET /api/dating-events/open, POST /api/dating-events/, POST /api/dating-events/{eventId:long}/open, POST /api/dating-events/{eventId:long}/close, POST /api/dating-events/{eventId:long}/cancel, PUT /api/dating-events/{eventId:long}/location, PUT /api/dating-events/{eventId:long}/commission, POST /api/dating-events/{eventId:long}/tickets, POST /api/dating-events/{eventId:long}/send-sms, POST /api/dating-events/sms-requests/{requestId:long}/approve, POST /api/dating-events/sms-requests/{requestId:long}/reject, POST /api/dating-profiles/, GET /api/dating-profiles/{profileId:long}, GET /api/dating-profiles/by-user/{userId:long}, PUT /api/dating-profiles/{profileId:long}, DELETE /api/dating-profiles/{profileId:long}, GET /api/event-chats/me/conversations, POST /api/event-chats/events/{eventId:long}/conversations, POST /api/event-chats/events/{eventId:long}/likes/reject, POST /api/event-chats/conversations/{conversationId:long}/messages, POST /api/event-chats/conversations/{conversationId:long}/blocks, GET /api/event-participants/me/archive, GET /api/event-participants/events/{eventId:long}/profiles, GET /api/event-participants/events/{eventId:long}/participants, POST /api/event-participants/events/{eventId:long}/participants/{participantUserId:long}/remove, PUT /api/event-planner-profile/me, GET /api/event-surveys/events/{eventId:long}/me, POST /api/event-surveys/events/{eventId:long}/me, GET /api/event-types/, POST /api/event-types/, PUT /api/event-types/{id:long}, POST /api/moderation-reports/, GET /api/moderation-reports/, GET /api/moderation-reports/admin, PUT /api/moderation-reports/{reportId:long}/review, GET /api/privacy/me/export, DELETE /api/privacy/me, POST /api/support-tickets/, GET /api/support-tickets/, GET /api/support-tickets/staff, GET /api/support-tickets/{ticketId:long}, POST /api/support-tickets/{ticketId:long}/replies, PUT /api/support-tickets/{ticketId:long}/status, PUT /api/support-tickets/{ticketId:long}/assignee, PUT /api/admin/users/{userId:long}/role

## Main architecture style detected
Layered Clean Architecture-inspired .NET solution with DDD/CQRS/vertical-slice influences.

## Main risks discovered
Payment/finance correctness, privacy coverage, auth hardening, notification provider gaps, matching terminology, large DbContext migration risk, and dirty worktree state.

## Main gaps discovered
See `docs/14-roadmap/known-gaps.md` and `docs/11-security-privacy/security-gaps.md`.

## Recommended next actions
Run build/tests, add API/payment/auth/privacy tests, validate Mermaid rendering, and keep this documentation updated with future schema/API changes.

## Files or areas that could not be fully analyzed
Runtime behavior, real external provider integrations, production secrets/configuration, visual UI state across browsers, and exact API DTO examples require runtime/manual verification.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
