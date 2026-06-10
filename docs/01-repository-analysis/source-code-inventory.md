# Source Code Inventory

## Purpose
Summarize source files by area.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `.github/copilot-instructions.md`
- `.github/instructions/clean-architecture.instructions.md`
- `.github/instructions/cqrs.instructions.md`
- `.github/instructions/csharp.instructions.md`
- `.github/instructions/domain-events.instructions.md`
- `.github/instructions/ef.instructions.md`
- `.github/instructions/logging.instructions.md`
- `.github/instructions/naming.instructions.md`
- `.github/instructions/repository.instructions.md`
- `.github/instructions/testing.instructions.md`
- `.github/instructions/webapi.instructions.md`
- `.github/skills/create-cqrs-feature/SKILL.md`
- `.github/skills/create-cqrs-feature/template.cs`
- `.github/skills/create-integration-test/SKILL.md`
- `.github/skills/create-rag-pipeline/SKILL.md`
- `.github/skills/create-webapi-endpoint/SKILL.md`
- `.github/skills/optimize-ef-query/SKILL.md`
- `.github/skills/review-security/SKILL.md`
- `.github/skills/semantic-kernel-plugin/SKILL.md`
- `README.md`

| Area | Count | Notes |
| --- | ---: | --- |
| C# files | 560 | Domain/Application/Infrastructure/WebApi/AdminPanel/tests |
| Razor pages/views | 63 | AdminPanel UI screens and shared partials |
| JSON config | 7 | appsettings and launch settings |
| Migrations | 39 | EF Core schema history |

## Representative files
- `.github/copilot-instructions.md`
- `.github/instructions/clean-architecture.instructions.md`
- `.github/instructions/cqrs.instructions.md`
- `.github/instructions/csharp.instructions.md`
- `.github/instructions/domain-events.instructions.md`
- `.github/instructions/ef.instructions.md`
- `.github/instructions/logging.instructions.md`
- `.github/instructions/naming.instructions.md`
- `.github/instructions/repository.instructions.md`
- `.github/instructions/testing.instructions.md`
- `.github/instructions/webapi.instructions.md`
- `.github/skills/create-cqrs-feature/SKILL.md`
- `.github/skills/create-cqrs-feature/template.cs`
- `.github/skills/create-integration-test/SKILL.md`
- `.github/skills/create-rag-pipeline/SKILL.md`
- `.github/skills/create-webapi-endpoint/SKILL.md`
- `.github/skills/optimize-ef-query/SKILL.md`
- `.github/skills/review-security/SKILL.md`
- `.github/skills/semantic-kernel-plugin/SKILL.md`
- `README.md`
- `Randevoo.sln`
- `src/Randevoo.AdminPanel/Models/Auth/AdminRole.cs`
- `src/Randevoo.AdminPanel/Models/Auth/LoginRequest.cs`
- `src/Randevoo.AdminPanel/Models/Auth/MockUser.cs`
- `src/Randevoo.AdminPanel/Models/Buyers/TicketOrderListModels.cs`
- `src/Randevoo.AdminPanel/Models/Common/AppLanguage.cs`
- `src/Randevoo.AdminPanel/Models/Common/CityOption.cs`
- `src/Randevoo.AdminPanel/Models/Common/CountryOption.cs`
- `src/Randevoo.AdminPanel/Models/Common/DashboardDateRange.cs`
- `src/Randevoo.AdminPanel/Models/Common/DashboardFilterViewModel.cs`
- `src/Randevoo.AdminPanel/Models/Common/DashboardStats.cs`
- `src/Randevoo.AdminPanel/Models/Common/EducationLevelOption.cs`
- `src/Randevoo.AdminPanel/Models/Common/GenderOption.cs`
- `src/Randevoo.AdminPanel/Models/Common/Policies.cs`
- `src/Randevoo.AdminPanel/Models/Common/SystemLookupOption.cs`
- `src/Randevoo.AdminPanel/Models/Common/ZodiacSignOption.cs`
- `src/Randevoo.AdminPanel/Models/Dashboard/AdminAnalyticsModels.cs`
- `src/Randevoo.AdminPanel/Models/DiscountCodes/EventDiscountCodeAdminItem.cs`
- `src/Randevoo.AdminPanel/Models/DiscountCodes/EventDiscountCodeEditorInput.cs`
- `src/Randevoo.AdminPanel/Models/DiscountCodes/EventDiscountCodeUsageItem.cs`
- `src/Randevoo.AdminPanel/Models/Events/DatingEvent.cs`
- `src/Randevoo.AdminPanel/Models/Events/EmergencyRefundInput.cs`
- `src/Randevoo.AdminPanel/Models/Events/EventChangeLogEntry.cs`
- `src/Randevoo.AdminPanel/Models/Events/EventDraftInput.cs`
- `src/Randevoo.AdminPanel/Models/Events/EventDraftState.cs`
- `src/Randevoo.AdminPanel/Models/Events/EventFaqInput.cs`
- `src/Randevoo.AdminPanel/Models/Events/EventImageCarouselModel.cs`
- `src/Randevoo.AdminPanel/Models/Events/EventListFilter.cs`
- `src/Randevoo.AdminPanel/Models/Events/EventListResult.cs`
- `src/Randevoo.AdminPanel/Models/Events/EventListScope.cs`
- `src/Randevoo.AdminPanel/Models/Events/EventModeOption.cs`
- `src/Randevoo.AdminPanel/Models/Events/EventOperationalStatus.cs`
- `src/Randevoo.AdminPanel/Models/Events/EventReviewStatus.cs`
- `src/Randevoo.AdminPanel/Models/Events/EventSmsRequest.cs`
- `src/Randevoo.AdminPanel/Models/Events/EventSmsRequestStatus.cs`
- `src/Randevoo.AdminPanel/Models/Events/EventTicketBuyerItem.cs`
- `src/Randevoo.AdminPanel/Models/Events/EventType.cs`
- `src/Randevoo.AdminPanel/Models/Events/EventTypeAdminItem.cs`
- `src/Randevoo.AdminPanel/Models/Events/EventTypeEditorInput.cs`
- `src/Randevoo.AdminPanel/Models/Events/EventTypeOption.cs`
- `src/Randevoo.AdminPanel/Models/Events/OnlineEventPlatformOption.cs`
- `src/Randevoo.AdminPanel/Models/Events/PendingEventChangeItem.cs`
- `src/Randevoo.AdminPanel/Models/Events/TagAdminItem.cs`
- `src/Randevoo.AdminPanel/Models/Events/TagEditorInput.cs`
- `src/Randevoo.AdminPanel/Models/Events/TagOption.cs`
- `src/Randevoo.AdminPanel/Models/Finance/AdminEventTicketTransactionGroup.cs`
- `src/Randevoo.AdminPanel/Models/Finance/AdminTicketTransactionItem.cs`
- `src/Randevoo.AdminPanel/Models/Finance/ManualPaymentReceiptItem.cs`
- `src/Randevoo.AdminPanel/Models/Finance/ManualPaymentReceiptReviewInput.cs`
- `src/Randevoo.AdminPanel/Models/Finance/PlannerBankAccountInput.cs`
- `src/Randevoo.AdminPanel/Models/Finance/PlannerBankAccountItem.cs`
- `src/Randevoo.AdminPanel/Models/Finance/PlannerCommissionEventSummary.cs`
- `src/Randevoo.AdminPanel/Models/Finance/PlannerCommissionTransactionItem.cs`
- `src/Randevoo.AdminPanel/Models/Finance/PlannerFinanceDashboard.cs`
- `src/Randevoo.AdminPanel/Models/Finance/PlannerWithdrawalRequestItem.cs`
- `src/Randevoo.AdminPanel/Models/Finance/UserFinanceOverview.cs`
- `src/Randevoo.AdminPanel/Models/Finance/UserFinanceTransactionItem.cs`
- `src/Randevoo.AdminPanel/Models/Finance/UserOnlinePaymentItem.cs`
- `src/Randevoo.AdminPanel/Models/Finance/WithdrawalRequestInput.cs`
- `src/Randevoo.AdminPanel/Models/Finance/WithdrawalReviewInput.cs`

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
