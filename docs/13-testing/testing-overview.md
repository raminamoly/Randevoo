# Testing Overview

## Purpose
Summarize test projects and current coverage.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `tests/Randevoo.Tests.Integration/AuthApiTests.cs`
- `tests/Randevoo.Tests.Integration/DatingEventApiTests.cs`
- `tests/Randevoo.Tests.Integration/DatingProfileApiTests.cs`
- `tests/Randevoo.Tests.Integration/ObservabilityTests.cs`
- `tests/Randevoo.Tests.Integration/SqlServerRelationalTests.cs`
- `tests/Randevoo.Tests.Integration/SupportTicketRepositoryTests.cs`
- `tests/Randevoo.Tests.Unit/Builder/UserBuilder.cs`
- `tests/Randevoo.Tests.Unit/DatingEventTests.cs`
- `tests/Randevoo.Tests.Unit/SupportTicketTests.cs`
- `tests/Randevoo.Tests.Unit/UserProfileTests.cs`
- `tests/Randevoo.Tests.Unit/UserTests.cs`

Detected 11 test files with 70 Fact/Theory attributes.

| Test class | Facts/Theories | Methods | Source |
| --- | ---: | --- | --- |
| AuthApiTests | 6 | MobileLogin_ThenEmailConfirmation_CompletesPasswordlessAuthFlow, EmailConfirmationRequest_WithoutJwt_ReturnsUnauthorized, RefreshToken_RotatesToken_AndOldTokenCannotBeReused, Logout_RevokesRefreshToken, MobileLoginRequest_WhenRequestedTooOften_ReturnsBadRequest, MobileLoginVerify_WhenWrongCodeRepeatedly_LocksLogin, SendLoginCodeAsync, SendMessageAsync | `tests/Randevoo.Tests.Integration/AuthApiTests.cs` |
| DatingEventApiTests | 10 | EventPlannerCanCreateEvent_AndEndUserCanBuyTicket, CreateEvent_UsesOneSharedTicketCurrency, EndUserCannotCreateDatingEvent, TicketPurchaseFails_WhenBuyerEducationDoesNotMeetEventRestriction, TicketPurchase_AppliesDiscountCode_AndChargesDiscountedAmount, TicketPurchase_ForOrganizerManualTransfer_DebitsPlannerForPlatformCommission, PlannerSmsRequestRequiresAdminApprovalBeforeQueueingMessages, EventParticipantsCanUseArchiveProfilesChatAndSurvey_AndPlannerCanRemoveWithRefund | `tests/Randevoo.Tests.Integration/DatingEventApiTests.cs` |
| DatingProfileApiTests | 6 | CreateAndGetDatingProfile_ReturnsCreatedProfile, CreateDatingProfile_WithMissingUser_ReturnsNotFound, UpdateAndDeleteDatingProfile_ChangesProfileThenHidesDeletedProfile, CreateDatingProfile_WithoutJwt_ReturnsUnauthorized, UpdateDatingProfile_ForAnotherUser_ReturnsForbidden, AdminCanReadUserDatingProfile | `tests/Randevoo.Tests.Integration/DatingProfileApiTests.cs` |
| ObservabilityTests | 2 | GlobalExceptionMiddleware_ReturnsSafeProblem_WithCorrelationId, AdminRoleChange_CreatesAuditLog_WithCorrelationId | `tests/Randevoo.Tests.Integration/ObservabilityTests.cs` |
| SqlServerRelationalTests | 1 | SqlServer_EnforcesUniqueMobileNumber | `tests/Randevoo.Tests.Integration/SqlServerRelationalTests.cs` |
| SupportTicketRepositoryTests | 2 | GetNextRoundRobinAssigneeAsync_AssignsActiveSupportUsersInSequence, ListAsync_AppliesSupportAndPlannerRecipientScopes | `tests/Randevoo.Tests.Integration/SupportTicketRepositoryTests.cs` |
| UserBuilder | 0 | Needs Verification | `tests/Randevoo.Tests.Unit/Builder/UserBuilder.cs` |
| DatingEventTests | 14 | Needs Verification | `tests/Randevoo.Tests.Unit/DatingEventTests.cs` |
| SupportTicketTests | 8 | Needs Verification | `tests/Randevoo.Tests.Unit/SupportTicketTests.cs` |
| UserProfileTests | 10 | Needs Verification | `tests/Randevoo.Tests.Unit/UserProfileTests.cs` |
| UserTests | 11 | Needs Verification | `tests/Randevoo.Tests.Unit/UserTests.cs` |

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
