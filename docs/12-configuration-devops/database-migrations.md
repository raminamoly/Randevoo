# Database Migrations

## Purpose
Document EF migration workflow.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.Infrastructure/Data/Migrations/20260529191443_UserProfileCqrsLayer.cs`
- `src/Randevoo.Infrastructure/Data/Migrations/20260529194807_PasswordlessMobileAuth.cs`
- `src/Randevoo.Infrastructure/Data/Migrations/20260530051637_RolesBalancesAndDatingEvents.cs`
- `src/Randevoo.Infrastructure/Data/Migrations/20260530060221_EventParticipantsChatsAndSurveys.cs`
- `src/Randevoo.Infrastructure/Data/Migrations/20260530064558_SafetyModerationEventTypesPlannerQuality.cs`
- `src/Randevoo.Infrastructure/Data/Migrations/20260530071721_RefreshTokensAndAuthHardening.cs`
- `src/Randevoo.Infrastructure/Data/Migrations/20260530100619_DatingEventEventTypeForeignKey.cs`
- `src/Randevoo.Infrastructure/Data/Migrations/20260602180315_AddAuditLogs.cs`
- `src/Randevoo.Infrastructure/Data/Migrations/20260603091205_AddDatingEventTags.cs`
- `src/Randevoo.Infrastructure/Data/Migrations/20260603111215_AddEventParticipantSmsApprovalQueue.cs`
- `src/Randevoo.Infrastructure/Data/Migrations/20260603165909_AddEventSmsSchedulingAndAdminEdits.cs`
- `src/Randevoo.Infrastructure/Data/Migrations/20260603172344_AddDatingEventEducationRestriction.cs`
- `src/Randevoo.Infrastructure/Data/Migrations/20260603180239_AddPlannerProfileApprovalWorkflow.cs`
- `src/Randevoo.Infrastructure/Data/Migrations/20260603221054_AddPlannerFinanceWithdrawals.cs`
- `src/Randevoo.Infrastructure/Data/Migrations/20260604055738_AddCountryCityLookups.cs`
- `src/Randevoo.Infrastructure/Data/Migrations/20260604060924_NormalizeProfileEventLookupTables.cs`
- `src/Randevoo.Infrastructure/Data/Migrations/20260604064035_NormalizeLocationTagsAndRialCurrency.cs`
- `src/Randevoo.Infrastructure/Data/Migrations/20260604073648_AddUserProfileImagesAndSampleProfiles.cs`
- `src/Randevoo.Infrastructure/Data/Migrations/20260604113541_AddOnlinePaymentsAndPlannerBankAccounts.cs`
- `src/Randevoo.Infrastructure/Data/Migrations/20260604115405_AddEventDeliveryModesAndFaqs.cs`

```powershell
dotnet ef database update --project src/Randevoo.Infrastructure --startup-project src/Randevoo.WebApi
dotnet ef migrations add <Name> --project src/Randevoo.Infrastructure --startup-project src/Randevoo.WebApi
```

Current migrations: 39.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
