# Tables And Fields

## Purpose
Detailed table/entity field catalog extracted from domain entities and DbContext declarations.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.Domain/Entities/AuditLog.cs`
- `src/Randevoo.Domain/Entities/BalanceAccount.cs`
- `src/Randevoo.Domain/Entities/BalanceTransaction.cs`
- `src/Randevoo.Domain/Entities/BalanceTransactionTypeLookup.cs`
- `src/Randevoo.Domain/Entities/City.cs`
- `src/Randevoo.Domain/Entities/Country.cs`
- `src/Randevoo.Domain/Entities/CurrencyExchangeRate.cs`
- `src/Randevoo.Domain/Entities/CurrencyLookup.cs`
- `src/Randevoo.Domain/Entities/DatingEvent.cs`
- `src/Randevoo.Domain/Entities/EducationLevelLookup.cs`
- `src/Randevoo.Domain/Entities/EventChatBlock.cs`
- `src/Randevoo.Domain/Entities/EventChatMessage.cs`
- `src/Randevoo.Domain/Entities/EventConversation.cs`
- `src/Randevoo.Domain/Entities/EventDiscountCode.cs`
- `src/Randevoo.Domain/Entities/EventDiscountTypeLookup.cs`
- `src/Randevoo.Domain/Entities/EventFaq.cs`
- `src/Randevoo.Domain/Entities/EventLike.cs`
- `src/Randevoo.Domain/Entities/EventModeLookup.cs`
- `src/Randevoo.Domain/Entities/EventParticipantSmsRequest.cs`
- `src/Randevoo.Domain/Entities/EventPlannerProfile.cs`
- `src/Randevoo.Domain/Entities/EventReviewStatusLookup.cs`
- `src/Randevoo.Domain/Entities/EventSurveyRating.cs`
- `src/Randevoo.Domain/Entities/EventSurveyResponse.cs`
- `src/Randevoo.Domain/Entities/EventTag.cs`
- `src/Randevoo.Domain/Entities/EventTicket.cs`
- `src/Randevoo.Domain/Entities/EventType.cs`
- `src/Randevoo.Domain/Entities/GenderLookup.cs`
- `src/Randevoo.Domain/Entities/Interest.cs`
- `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs`
- `src/Randevoo.Domain/Entities/ModerationReport.cs`
- `src/Randevoo.Domain/Entities/OnlineEventPlatform.cs`
- `src/Randevoo.Domain/Entities/OnlinePayment.cs`
- `src/Randevoo.Domain/Entities/PermissionAction.cs`
- `src/Randevoo.Domain/Entities/PlannerBankAccount.cs`
- `src/Randevoo.Domain/Entities/PlannerWithdrawalRequest.cs`
- `src/Randevoo.Domain/Entities/RefreshToken.cs`
- `src/Randevoo.Domain/Entities/RoleOperationPermission.cs`
- `src/Randevoo.Domain/Entities/SmsQueueItem.cs`
- `src/Randevoo.Domain/Entities/SupportTicket.cs`
- `src/Randevoo.Domain/Entities/SupportTicketAssignmentCursor.cs`
- `src/Randevoo.Domain/Entities/SupportTicketAttachment.cs`
- `src/Randevoo.Domain/Entities/SupportTicketCategoryLookup.cs`
- `src/Randevoo.Domain/Entities/SupportTicketHistoryEntry.cs`
- `src/Randevoo.Domain/Entities/SupportTicketMessage.cs`
- `src/Randevoo.Domain/Entities/SupportTicketRecipientTypeLookup.cs`
- `src/Randevoo.Domain/Entities/SupportTicketStatusLookup.cs`
- `src/Randevoo.Domain/Entities/Tag.cs`
- `src/Randevoo.Domain/Entities/TicketOrder.cs`
- `src/Randevoo.Domain/Entities/User.cs`
- `src/Randevoo.Domain/Entities/UserOperationPermissionOverride.cs`
- `src/Randevoo.Domain/Entities/UserProfile.cs`
- `src/Randevoo.Domain/Entities/UserProfileImage.cs`
- `src/Randevoo.Domain/Entities/UserRoleLookup.cs`
- `src/Randevoo.Domain/Entities/ZodiacSignLookup.cs`


## Table/Entity: AuditLog

Source files:
- `src/Randevoo.Domain/Entities/AuditLog.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Audit Log record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| ActorUserId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related ActorUser record. | `src/Randevoo.Domain/Entities/AuditLog.cs` |
| ActorDisplayName | string? | Needs Verification |  | See initializer/DbContext | nullable marker | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/AuditLog.cs` |
| ActorRole | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Role/authorization classification. | `src/Randevoo.Domain/Entities/AuditLog.cs` |
| Action | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Business data for AuditLog; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/AuditLog.cs` |
| LogType | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Business data for AuditLog; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/AuditLog.cs` |
| Module | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for AuditLog; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/AuditLog.cs` |
| Description | string? | Needs Verification |  | See initializer/DbContext | nullable marker | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/AuditLog.cs` |
| TargetType | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Business data for AuditLog; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/AuditLog.cs` |
| TargetId | string | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related Target record. | `src/Randevoo.Domain/Entities/AuditLog.cs` |
| BeforeJson | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for AuditLog; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/AuditLog.cs` |
| AfterJson | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for AuditLog; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/AuditLog.cs` |
| Reason | string? | Needs Verification |  | See initializer/DbContext | nullable marker | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/AuditLog.cs` |
| IpAddress | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for AuditLog; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/AuditLog.cs` |
| RequestPath | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for AuditLog; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/AuditLog.cs` |
| UserAgent | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for AuditLog; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/AuditLog.cs` |
| Status | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Lifecycle/status value used by business workflows. | `src/Randevoo.Domain/Entities/AuditLog.cs` |
| MetadataJson | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for AuditLog; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/AuditLog.cs` |
| CorrelationId | string? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related Correlation record. | `src/Randevoo.Domain/Entities/AuditLog.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| AuditLog.ActorUserId | Many-to-one candidate | ActorUser | ActorUserId | See DbContext | `src/Randevoo.Domain/Entities/AuditLog.cs` |
| AuditLog.TargetId | Many-to-one candidate | Target | TargetId | See DbContext | `src/Randevoo.Domain/Entities/AuditLog.cs` |
| AuditLog.CorrelationId | Many-to-one candidate | Correlation | CorrelationId | See DbContext | `src/Randevoo.Domain/Entities/AuditLog.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: BalanceAccount

Source files:
- `src/Randevoo.Domain/Entities/BalanceAccount.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Balance Account record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| UserId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related User record. | `src/Randevoo.Domain/Entities/BalanceAccount.cs` |
| User | User | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for BalanceAccount; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/BalanceAccount.cs` |
| Balance | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/BalanceAccount.cs` |
| ReportingCurrencyCode | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/BalanceAccount.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| BalanceAccount.UserId | Many-to-one candidate | User | UserId | See DbContext | `src/Randevoo.Domain/Entities/BalanceAccount.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: BalanceTransaction

Source files:
- `src/Randevoo.Domain/Entities/BalanceTransaction.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Balance Transaction record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| BalanceAccountId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related BalanceAccount record. | `src/Randevoo.Domain/Entities/BalanceTransaction.cs` |
| BalanceAccount | BalanceAccount | Likely required |  | See initializer/DbContext | See DbContext/migrations | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/BalanceTransaction.cs` |
| UserId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related User record. | `src/Randevoo.Domain/Entities/BalanceTransaction.cs` |
| Amount | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/BalanceTransaction.cs` |
| CurrencyCode | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/BalanceTransaction.cs` |
| ReportingCurrencyCode | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/BalanceTransaction.cs` |
| ReportingAmountIrr | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/BalanceTransaction.cs` |
| ExchangeRateToIrr | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/BalanceTransaction.cs` |
| ExchangeRateCapturedAtUtc | DateTime | Likely required |  | See initializer/DbContext | See DbContext/migrations | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/BalanceTransaction.cs` |
| ExchangeRateId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related ExchangeRate record. | `src/Randevoo.Domain/Entities/BalanceTransaction.cs` |
| ExchangeRate | CurrencyExchangeRate? | Needs Verification |  | See initializer/DbContext | nullable marker | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/BalanceTransaction.cs` |
| Type | BalanceTransactionType | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for BalanceTransaction; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/BalanceTransaction.cs` |
| Description | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/BalanceTransaction.cs` |
| DatingEventId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related DatingEvent record. | `src/Randevoo.Domain/Entities/BalanceTransaction.cs` |
| TicketOrderId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related TicketOrder record. | `src/Randevoo.Domain/Entities/BalanceTransaction.cs` |
| TicketOrder | TicketOrder? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for BalanceTransaction; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/BalanceTransaction.cs` |
| ReferenceType | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for BalanceTransaction; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/BalanceTransaction.cs` |
| ReferenceId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related Reference record. | `src/Randevoo.Domain/Entities/BalanceTransaction.cs` |
| CreatedByUserId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related CreatedByUser record. | `src/Randevoo.Domain/Entities/BalanceTransaction.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| BalanceTransaction.BalanceAccountId | Many-to-one candidate | BalanceAccount | BalanceAccountId | See DbContext | `src/Randevoo.Domain/Entities/BalanceTransaction.cs` |
| BalanceTransaction.UserId | Many-to-one candidate | User | UserId | See DbContext | `src/Randevoo.Domain/Entities/BalanceTransaction.cs` |
| BalanceTransaction.ExchangeRateId | Many-to-one candidate | ExchangeRate | ExchangeRateId | See DbContext | `src/Randevoo.Domain/Entities/BalanceTransaction.cs` |
| BalanceTransaction.DatingEventId | Many-to-one candidate | DatingEvent | DatingEventId | See DbContext | `src/Randevoo.Domain/Entities/BalanceTransaction.cs` |
| BalanceTransaction.TicketOrderId | Many-to-one candidate | TicketOrder | TicketOrderId | See DbContext | `src/Randevoo.Domain/Entities/BalanceTransaction.cs` |
| BalanceTransaction.ReferenceId | Many-to-one candidate | Reference | ReferenceId | See DbContext | `src/Randevoo.Domain/Entities/BalanceTransaction.cs` |
| BalanceTransaction.CreatedByUserId | Many-to-one candidate | CreatedByUser | CreatedByUserId | See DbContext | `src/Randevoo.Domain/Entities/BalanceTransaction.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: BalanceTransactionTypeLookup

Source files:
- `src/Randevoo.Domain/Entities/BalanceTransactionTypeLookup.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Balance Transaction Type Lookup record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| Name | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/BalanceTransactionTypeLookup.cs` |
| DisplayNameFa | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/BalanceTransactionTypeLookup.cs` |
| IsActive | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for BalanceTransactionTypeLookup; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/BalanceTransactionTypeLookup.cs` |
| DisplayOrder | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for BalanceTransactionTypeLookup; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/BalanceTransactionTypeLookup.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| Needs Verification | Needs Verification | Needs Verification | Needs Verification | See DbContext/migrations | `src/Randevoo.Domain/Entities/BalanceTransactionTypeLookup.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: City

Source files:
- `src/Randevoo.Domain/Entities/City.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
City record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| CountryId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related Country record. | `src/Randevoo.Domain/Entities/City.cs` |
| Country | Country | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for City; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/City.cs` |
| Name | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/City.cs` |
| IsActive | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for City; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/City.cs` |
| DisplayOrder | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for City; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/City.cs` |
| Latitude | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Business data for City; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/City.cs` |
| Longitude | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Business data for City; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/City.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| City.CountryId | Many-to-one candidate | Country | CountryId | See DbContext | `src/Randevoo.Domain/Entities/City.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: Country

Source files:
- `src/Randevoo.Domain/Entities/Country.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Country record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| Name | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/Country.cs` |
| Code | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/Country.cs` |
| IsActive | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for Country; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/Country.cs` |
| DisplayOrder | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for Country; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/Country.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| Needs Verification | Needs Verification | Needs Verification | Needs Verification | See DbContext/migrations | `src/Randevoo.Domain/Entities/Country.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: CurrencyExchangeRate

Source files:
- `src/Randevoo.Domain/Entities/CurrencyExchangeRate.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Currency Exchange Rate record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| FromCurrencyCode | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/CurrencyExchangeRate.cs` |
| ToCurrencyCode | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/CurrencyExchangeRate.cs` |
| Rate | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/CurrencyExchangeRate.cs` |
| EffectiveFromUtc | DateTime | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for CurrencyExchangeRate; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/CurrencyExchangeRate.cs` |
| EffectiveToUtc | DateTime? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for CurrencyExchangeRate; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/CurrencyExchangeRate.cs` |
| Source | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Business data for CurrencyExchangeRate; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/CurrencyExchangeRate.cs` |
| CreatedByUserId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related CreatedByUser record. | `src/Randevoo.Domain/Entities/CurrencyExchangeRate.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| CurrencyExchangeRate.CreatedByUserId | Many-to-one candidate | CreatedByUser | CreatedByUserId | See DbContext | `src/Randevoo.Domain/Entities/CurrencyExchangeRate.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: CurrencyLookup

Source files:
- `src/Randevoo.Domain/Entities/CurrencyLookup.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Currency Lookup record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| Code | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/CurrencyLookup.cs` |
| DisplayNameFa | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/CurrencyLookup.cs` |
| Symbol | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Business data for CurrencyLookup; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/CurrencyLookup.cs` |
| DecimalPlaces | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for CurrencyLookup; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/CurrencyLookup.cs` |
| IsActive | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for CurrencyLookup; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/CurrencyLookup.cs` |
| DisplayOrder | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for CurrencyLookup; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/CurrencyLookup.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| Needs Verification | Needs Verification | Needs Verification | Needs Verification | See DbContext/migrations | `src/Randevoo.Domain/Entities/CurrencyLookup.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: DatingEvent

Source files:
- `src/Randevoo.Domain/Entities/DatingEvent.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Dating Event record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| Title | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| Location | Location | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for DatingEvent; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| Address | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Business data for DatingEvent; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| DateTimeStart | DateTime | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for DatingEvent; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| DateTimeEnd | DateTime | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for DatingEvent; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| EventTypeId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related EventType record. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| EventType | EventType | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for DatingEvent; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| EventModeId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related EventMode record. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| EventMode | EventModeLookup | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for DatingEvent; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| OnlineEventPlatformId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related OnlineEventPlatform record. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| OnlineEventPlatform | OnlineEventPlatform? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for DatingEvent; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| OnlineJoinUrl | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for DatingEvent; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| OnlineAccessInstructions | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for DatingEvent; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| CountryId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related Country record. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| Country | Country? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for DatingEvent; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| CityId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related City record. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| City | City? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for DatingEvent; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| AgeRangeForMale | AgeRange | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for DatingEvent; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| AgeRangeForFemale | AgeRange | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for DatingEvent; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| IsOpenForSell | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for DatingEvent; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| IsCancelled | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for DatingEvent; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| ReviewStatus | EventReviewStatus | Likely required |  | See initializer/DbContext | See DbContext/migrations | Lifecycle/status value used by business workflows. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| EventPlannerUserId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related EventPlannerUser record. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| EventPlannerUser | User | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for DatingEvent; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| EventPlannerCommissionPercent | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| PaymentCollectionMethod | EventPaymentCollectionMethod | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for DatingEvent; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| OrganizerPaymentInstructions | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for DatingEvent; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| MaleCapacity | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for DatingEvent; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| FemaleCapacity | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for DatingEvent; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| NumberOfLikesAllowed | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for DatingEvent; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| CurrencyCode | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| MaleTicketPrice | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| MaleTicketCurrencyCode | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| FemaleTicketPrice | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| FemaleTicketCurrencyCode | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| EducationLevelRestriction | EventEducationLevelRestriction | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for DatingEvent; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| MinimumEducationLevelId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related MinimumEducationLevel record. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| MinimumEducationLevel | EducationLevelLookup? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for DatingEvent; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| EventImage1 | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for DatingEvent; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| EventImage2 | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for DatingEvent; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| EventImage3 | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for DatingEvent; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| EventDescriptionHtml | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/DatingEvent.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| DatingEvent.EventTypeId | Many-to-one candidate | EventType | EventTypeId | See DbContext | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| DatingEvent.EventModeId | Many-to-one candidate | EventMode | EventModeId | See DbContext | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| DatingEvent.OnlineEventPlatformId | Many-to-one candidate | OnlineEventPlatform | OnlineEventPlatformId | See DbContext | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| DatingEvent.CountryId | Many-to-one candidate | Country | CountryId | See DbContext | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| DatingEvent.CityId | Many-to-one candidate | City | CityId | See DbContext | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| DatingEvent.EventPlannerUserId | Many-to-one candidate | EventPlannerUser | EventPlannerUserId | See DbContext | `src/Randevoo.Domain/Entities/DatingEvent.cs` |
| DatingEvent.MinimumEducationLevelId | Many-to-one candidate | MinimumEducationLevel | MinimumEducationLevelId | See DbContext | `src/Randevoo.Domain/Entities/DatingEvent.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: EducationLevelLookup

Source files:
- `src/Randevoo.Domain/Entities/EducationLevelLookup.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Education Level Lookup record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| Title | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/EducationLevelLookup.cs` |
| Rank | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EducationLevelLookup; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EducationLevelLookup.cs` |
| IsActive | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EducationLevelLookup; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EducationLevelLookup.cs` |
| DisplayOrder | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EducationLevelLookup; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EducationLevelLookup.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| Needs Verification | Needs Verification | Needs Verification | Needs Verification | See DbContext/migrations | `src/Randevoo.Domain/Entities/EducationLevelLookup.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: EventChatBlock

Source files:
- `src/Randevoo.Domain/Entities/EventChatBlock.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Event Chat Block record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| EventConversationId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related EventConversation record. | `src/Randevoo.Domain/Entities/EventChatBlock.cs` |
| EventConversation | EventConversation | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventChatBlock; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventChatBlock.cs` |
| BlockerUserId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related BlockerUser record. | `src/Randevoo.Domain/Entities/EventChatBlock.cs` |
| BlockerUser | User | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventChatBlock; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventChatBlock.cs` |
| BlockedUserId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related BlockedUser record. | `src/Randevoo.Domain/Entities/EventChatBlock.cs` |
| BlockedUser | User | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventChatBlock; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventChatBlock.cs` |
| IsActive | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventChatBlock; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventChatBlock.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| EventChatBlock.EventConversationId | Many-to-one candidate | EventConversation | EventConversationId | See DbContext | `src/Randevoo.Domain/Entities/EventChatBlock.cs` |
| EventChatBlock.BlockerUserId | Many-to-one candidate | BlockerUser | BlockerUserId | See DbContext | `src/Randevoo.Domain/Entities/EventChatBlock.cs` |
| EventChatBlock.BlockedUserId | Many-to-one candidate | BlockedUser | BlockedUserId | See DbContext | `src/Randevoo.Domain/Entities/EventChatBlock.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: EventChatMessage

Source files:
- `src/Randevoo.Domain/Entities/EventChatMessage.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Event Chat Message record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| EventConversationId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related EventConversation record. | `src/Randevoo.Domain/Entities/EventChatMessage.cs` |
| EventConversation | EventConversation | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventChatMessage; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventChatMessage.cs` |
| SenderUserId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related SenderUser record. | `src/Randevoo.Domain/Entities/EventChatMessage.cs` |
| SenderUser | User | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventChatMessage; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventChatMessage.cs` |
| Body | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Business data for EventChatMessage; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventChatMessage.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| EventChatMessage.EventConversationId | Many-to-one candidate | EventConversation | EventConversationId | See DbContext | `src/Randevoo.Domain/Entities/EventChatMessage.cs` |
| EventChatMessage.SenderUserId | Many-to-one candidate | SenderUser | SenderUserId | See DbContext | `src/Randevoo.Domain/Entities/EventChatMessage.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: EventConversation

Source files:
- `src/Randevoo.Domain/Entities/EventConversation.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Event Conversation record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| DatingEventId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related DatingEvent record. | `src/Randevoo.Domain/Entities/EventConversation.cs` |
| DatingEvent | DatingEvent | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventConversation; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventConversation.cs` |
| StarterUserId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related StarterUser record. | `src/Randevoo.Domain/Entities/EventConversation.cs` |
| StarterUser | User | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventConversation; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventConversation.cs` |
| ParticipantUserId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related ParticipantUser record. | `src/Randevoo.Domain/Entities/EventConversation.cs` |
| ParticipantUser | User | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventConversation; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventConversation.cs` |
| IsDisabled | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventConversation; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventConversation.cs` |
| DisabledReason | string? | Needs Verification |  | See initializer/DbContext | nullable marker | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/EventConversation.cs` |
| DisabledByUserId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related DisabledByUser record. | `src/Randevoo.Domain/Entities/EventConversation.cs` |
| DisabledAt | DateTime? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for EventConversation; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventConversation.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| EventConversation.DatingEventId | Many-to-one candidate | DatingEvent | DatingEventId | See DbContext | `src/Randevoo.Domain/Entities/EventConversation.cs` |
| EventConversation.StarterUserId | Many-to-one candidate | StarterUser | StarterUserId | See DbContext | `src/Randevoo.Domain/Entities/EventConversation.cs` |
| EventConversation.ParticipantUserId | Many-to-one candidate | ParticipantUser | ParticipantUserId | See DbContext | `src/Randevoo.Domain/Entities/EventConversation.cs` |
| EventConversation.DisabledByUserId | Many-to-one candidate | DisabledByUser | DisabledByUserId | See DbContext | `src/Randevoo.Domain/Entities/EventConversation.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: EventDiscountCode

Source files:
- `src/Randevoo.Domain/Entities/EventDiscountCode.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Event Discount Code record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| DatingEventId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related DatingEvent record. | `src/Randevoo.Domain/Entities/EventDiscountCode.cs` |
| DatingEvent | DatingEvent? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for EventDiscountCode; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventDiscountCode.cs` |
| Code | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/EventDiscountCode.cs` |
| Title | string? | Needs Verification |  | See initializer/DbContext | nullable marker | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/EventDiscountCode.cs` |
| Description | string? | Needs Verification |  | See initializer/DbContext | nullable marker | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/EventDiscountCode.cs` |
| GenderScope | EventDiscountGenderScope | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventDiscountCode; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventDiscountCode.cs` |
| DiscountType | EventDiscountType | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventDiscountCode; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventDiscountCode.cs` |
| Value | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Business data for EventDiscountCode; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventDiscountCode.cs` |
| StartsAtUtc | DateTime | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventDiscountCode; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventDiscountCode.cs` |
| EndsAtUtc | DateTime | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventDiscountCode; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventDiscountCode.cs` |
| MaxUsageCount | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventDiscountCode; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventDiscountCode.cs` |
| UsedCount | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventDiscountCode; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventDiscountCode.cs` |
| IsActive | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventDiscountCode; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventDiscountCode.cs` |
| LastUsedAtUtc | DateTime? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for EventDiscountCode; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventDiscountCode.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| EventDiscountCode.DatingEventId | Many-to-one candidate | DatingEvent | DatingEventId | See DbContext | `src/Randevoo.Domain/Entities/EventDiscountCode.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: EventDiscountTypeLookup

Source files:
- `src/Randevoo.Domain/Entities/EventDiscountTypeLookup.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Event Discount Type Lookup record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| Name | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/EventDiscountTypeLookup.cs` |
| DisplayNameFa | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/EventDiscountTypeLookup.cs` |
| IsActive | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventDiscountTypeLookup; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventDiscountTypeLookup.cs` |
| DisplayOrder | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventDiscountTypeLookup; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventDiscountTypeLookup.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| Needs Verification | Needs Verification | Needs Verification | Needs Verification | See DbContext/migrations | `src/Randevoo.Domain/Entities/EventDiscountTypeLookup.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: EventFaq

Source files:
- `src/Randevoo.Domain/Entities/EventFaq.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Event Faq record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| DatingEventId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related DatingEvent record. | `src/Randevoo.Domain/Entities/EventFaq.cs` |
| DatingEvent | DatingEvent | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventFaq; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventFaq.cs` |
| Question | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Business data for EventFaq; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventFaq.cs` |
| Answer | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Business data for EventFaq; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventFaq.cs` |
| DisplayOrder | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventFaq; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventFaq.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| EventFaq.DatingEventId | Many-to-one candidate | DatingEvent | DatingEventId | See DbContext | `src/Randevoo.Domain/Entities/EventFaq.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: EventLike

Source files:
- `src/Randevoo.Domain/Entities/EventLike.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Event Like record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| DatingEventId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related DatingEvent record. | `src/Randevoo.Domain/Entities/EventLike.cs` |
| DatingEvent | DatingEvent | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventLike; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventLike.cs` |
| FromUserId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related FromUser record. | `src/Randevoo.Domain/Entities/EventLike.cs` |
| FromUser | User | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventLike; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventLike.cs` |
| ToUserId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related ToUser record. | `src/Randevoo.Domain/Entities/EventLike.cs` |
| ToUser | User | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventLike; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventLike.cs` |
| Status | EventLikeStatus | Likely required |  | See initializer/DbContext | See DbContext/migrations | Lifecycle/status value used by business workflows. | `src/Randevoo.Domain/Entities/EventLike.cs` |
| RespondedAtUtc | DateTime? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for EventLike; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventLike.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| EventLike.DatingEventId | Many-to-one candidate | DatingEvent | DatingEventId | See DbContext | `src/Randevoo.Domain/Entities/EventLike.cs` |
| EventLike.FromUserId | Many-to-one candidate | FromUser | FromUserId | See DbContext | `src/Randevoo.Domain/Entities/EventLike.cs` |
| EventLike.ToUserId | Many-to-one candidate | ToUser | ToUserId | See DbContext | `src/Randevoo.Domain/Entities/EventLike.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: EventModeLookup

Source files:
- `src/Randevoo.Domain/Entities/EventModeLookup.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Event Mode Lookup record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| Name | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/EventModeLookup.cs` |
| IsOnline | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventModeLookup; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventModeLookup.cs` |
| IsActive | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventModeLookup; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventModeLookup.cs` |
| DisplayOrder | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventModeLookup; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventModeLookup.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| Needs Verification | Needs Verification | Needs Verification | Needs Verification | See DbContext/migrations | `src/Randevoo.Domain/Entities/EventModeLookup.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: EventParticipantSmsRequest

Source files:
- `src/Randevoo.Domain/Entities/EventParticipantSmsRequest.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Event Participant Sms Request record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| DatingEventId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related DatingEvent record. | `src/Randevoo.Domain/Entities/EventParticipantSmsRequest.cs` |
| DatingEvent | DatingEvent | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventParticipantSmsRequest; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventParticipantSmsRequest.cs` |
| RequestedByUserId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related RequestedByUser record. | `src/Randevoo.Domain/Entities/EventParticipantSmsRequest.cs` |
| RequestedByUser | User | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventParticipantSmsRequest; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventParticipantSmsRequest.cs` |
| Message | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/EventParticipantSmsRequest.cs` |
| ApprovedMessage | string? | Needs Verification |  | See initializer/DbContext | nullable marker | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/EventParticipantSmsRequest.cs` |
| PlannedSendAtUtc | DateTime? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for EventParticipantSmsRequest; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventParticipantSmsRequest.cs` |
| Status | EventParticipantSmsRequestStatus | Likely required |  | See initializer/DbContext | See DbContext/migrations | Lifecycle/status value used by business workflows. | `src/Randevoo.Domain/Entities/EventParticipantSmsRequest.cs` |
| ReviewNote | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for EventParticipantSmsRequest; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventParticipantSmsRequest.cs` |
| ReviewedByAdminUserId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related ReviewedByAdminUser record. | `src/Randevoo.Domain/Entities/EventParticipantSmsRequest.cs` |
| ReviewedByAdminUser | User? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for EventParticipantSmsRequest; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventParticipantSmsRequest.cs` |
| ReviewedAt | DateTime? | Needs Verification |  | See initializer/DbContext | nullable marker | Timestamp for lifecycle state or audit tracking. | `src/Randevoo.Domain/Entities/EventParticipantSmsRequest.cs` |
| QueuedRecipientsCount | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventParticipantSmsRequest; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventParticipantSmsRequest.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| EventParticipantSmsRequest.DatingEventId | Many-to-one candidate | DatingEvent | DatingEventId | See DbContext | `src/Randevoo.Domain/Entities/EventParticipantSmsRequest.cs` |
| EventParticipantSmsRequest.RequestedByUserId | Many-to-one candidate | RequestedByUser | RequestedByUserId | See DbContext | `src/Randevoo.Domain/Entities/EventParticipantSmsRequest.cs` |
| EventParticipantSmsRequest.ReviewedByAdminUserId | Many-to-one candidate | ReviewedByAdminUser | ReviewedByAdminUserId | See DbContext | `src/Randevoo.Domain/Entities/EventParticipantSmsRequest.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: EventPlannerProfile

Source files:
- `src/Randevoo.Domain/Entities/EventPlannerProfile.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Event Planner Profile record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| UserId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related User record. | `src/Randevoo.Domain/Entities/EventPlannerProfile.cs` |
| User | User | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventPlannerProfile; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventPlannerProfile.cs` |
| Title | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/EventPlannerProfile.cs` |
| PictureUrl | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for EventPlannerProfile; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventPlannerProfile.cs` |
| Resume | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Business data for EventPlannerProfile; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventPlannerProfile.cs` |
| SettlementCurrencyCode | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/EventPlannerProfile.cs` |
| SettlementCurrencyLockedAtUtc | DateTime? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for EventPlannerProfile; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventPlannerProfile.cs` |
| SettlementCurrencyLockReason | string? | Needs Verification |  | See initializer/DbContext | nullable marker | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/EventPlannerProfile.cs` |
| HasPendingChanges | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventPlannerProfile; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventPlannerProfile.cs` |
| PendingFullName | string? | Needs Verification |  | See initializer/DbContext | nullable marker | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/EventPlannerProfile.cs` |
| PendingCity | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for EventPlannerProfile; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventPlannerProfile.cs` |
| PendingTitle | string? | Needs Verification |  | See initializer/DbContext | nullable marker | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/EventPlannerProfile.cs` |
| PendingPictureUrl | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for EventPlannerProfile; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventPlannerProfile.cs` |
| PendingResume | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for EventPlannerProfile; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventPlannerProfile.cs` |
| PendingSubmittedAt | DateTime? | Needs Verification |  | See initializer/DbContext | nullable marker | Timestamp for creation/submission lifecycle tracking. | `src/Randevoo.Domain/Entities/EventPlannerProfile.cs` |
| PendingReviewNote | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for EventPlannerProfile; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventPlannerProfile.cs` |
| PendingReviewedAt | DateTime? | Needs Verification |  | See initializer/DbContext | nullable marker | Timestamp for lifecycle state or audit tracking. | `src/Randevoo.Domain/Entities/EventPlannerProfile.cs` |
| PendingReviewedByAdminUserId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related PendingReviewedByAdminUser record. | `src/Randevoo.Domain/Entities/EventPlannerProfile.cs` |
| AverageRating | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Business data for EventPlannerProfile; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventPlannerProfile.cs` |
| TotalSurveyCount | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventPlannerProfile; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventPlannerProfile.cs` |
| HostedEventCount | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventPlannerProfile; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventPlannerProfile.cs` |
| CancelledEventCount | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventPlannerProfile; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventPlannerProfile.cs` |
| CompletedEventCount | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventPlannerProfile; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventPlannerProfile.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| EventPlannerProfile.UserId | Many-to-one candidate | User | UserId | See DbContext | `src/Randevoo.Domain/Entities/EventPlannerProfile.cs` |
| EventPlannerProfile.PendingReviewedByAdminUserId | Many-to-one candidate | PendingReviewedByAdminUser | PendingReviewedByAdminUserId | See DbContext | `src/Randevoo.Domain/Entities/EventPlannerProfile.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: EventReviewStatusLookup

Source files:
- `src/Randevoo.Domain/Entities/EventReviewStatusLookup.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Event Review Status Lookup record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| Name | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/EventReviewStatusLookup.cs` |
| DisplayNameFa | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/EventReviewStatusLookup.cs` |
| IsActive | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventReviewStatusLookup; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventReviewStatusLookup.cs` |
| DisplayOrder | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventReviewStatusLookup; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventReviewStatusLookup.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| Needs Verification | Needs Verification | Needs Verification | Needs Verification | See DbContext/migrations | `src/Randevoo.Domain/Entities/EventReviewStatusLookup.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: EventSurveyRating

Source files:
- `src/Randevoo.Domain/Entities/EventSurveyRating.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Event Survey Rating record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| EventSurveyResponseId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related EventSurveyResponse record. | `src/Randevoo.Domain/Entities/EventSurveyRating.cs` |
| EventSurveyResponse | EventSurveyResponse | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventSurveyRating; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventSurveyRating.cs` |
| Factor | SurveyFactor | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventSurveyRating; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventSurveyRating.cs` |
| Score | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventSurveyRating; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventSurveyRating.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| EventSurveyRating.EventSurveyResponseId | Many-to-one candidate | EventSurveyResponse | EventSurveyResponseId | See DbContext | `src/Randevoo.Domain/Entities/EventSurveyRating.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: EventSurveyResponse

Source files:
- `src/Randevoo.Domain/Entities/EventSurveyResponse.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Event Survey Response record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| DatingEventId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related DatingEvent record. | `src/Randevoo.Domain/Entities/EventSurveyResponse.cs` |
| DatingEvent | DatingEvent | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventSurveyResponse; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventSurveyResponse.cs` |
| UserId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related User record. | `src/Randevoo.Domain/Entities/EventSurveyResponse.cs` |
| User | User | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventSurveyResponse; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventSurveyResponse.cs` |
| Comment | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for EventSurveyResponse; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventSurveyResponse.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| EventSurveyResponse.DatingEventId | Many-to-one candidate | DatingEvent | DatingEventId | See DbContext | `src/Randevoo.Domain/Entities/EventSurveyResponse.cs` |
| EventSurveyResponse.UserId | Many-to-one candidate | User | UserId | See DbContext | `src/Randevoo.Domain/Entities/EventSurveyResponse.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: EventTag

Source files:
- `src/Randevoo.Domain/Entities/EventTag.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Event Tag record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| DatingEventId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related DatingEvent record. | `src/Randevoo.Domain/Entities/EventTag.cs` |
| DatingEvent | DatingEvent | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventTag; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventTag.cs` |
| TagId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related Tag record. | `src/Randevoo.Domain/Entities/EventTag.cs` |
| Tag | Tag | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventTag; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventTag.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| EventTag.DatingEventId | Many-to-one candidate | DatingEvent | DatingEventId | See DbContext | `src/Randevoo.Domain/Entities/EventTag.cs` |
| EventTag.TagId | Many-to-one candidate | Tag | TagId | See DbContext | `src/Randevoo.Domain/Entities/EventTag.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: EventTicket

Source files:
- `src/Randevoo.Domain/Entities/EventTicket.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Event Ticket record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| TicketOrderId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related TicketOrder record. | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| TicketOrder | TicketOrder | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventTicket; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| DatingEventId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related DatingEvent record. | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| DatingEvent | DatingEvent | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventTicket; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| UserId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related User record. | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| User | User | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventTicket; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| Gender | Gender | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventTicket; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| OriginalPrice | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| CurrencyCode | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| ReportingOriginalPriceIrr | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| ReportingPriceIrr | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| ExchangeRateToIrr | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| ExchangeRateCapturedAtUtc | DateTime | Likely required |  | See initializer/DbContext | See DbContext/migrations | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| ExchangeRateId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related ExchangeRate record. | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| ExchangeRate | CurrencyExchangeRate? | Needs Verification |  | See initializer/DbContext | nullable marker | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| DiscountAmount | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| EventDiscountCodeId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related EventDiscountCode record. | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| EventDiscountCode | EventDiscountCode? | Needs Verification |  | See initializer/DbContext | nullable marker | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| DiscountCode | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| Price | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| IsRefunded | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventTicket; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| IsRemoved | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventTicket; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| RemovalReason | string? | Needs Verification |  | See initializer/DbContext | nullable marker | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| RemovedByUserId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related RemovedByUser record. | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| RemovedAt | DateTime? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for EventTicket; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventTicket.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| EventTicket.TicketOrderId | Many-to-one candidate | TicketOrder | TicketOrderId | See DbContext | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| EventTicket.DatingEventId | Many-to-one candidate | DatingEvent | DatingEventId | See DbContext | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| EventTicket.UserId | Many-to-one candidate | User | UserId | See DbContext | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| EventTicket.ExchangeRateId | Many-to-one candidate | ExchangeRate | ExchangeRateId | See DbContext | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| EventTicket.EventDiscountCodeId | Many-to-one candidate | EventDiscountCode | EventDiscountCodeId | See DbContext | `src/Randevoo.Domain/Entities/EventTicket.cs` |
| EventTicket.RemovedByUserId | Many-to-one candidate | RemovedByUser | RemovedByUserId | See DbContext | `src/Randevoo.Domain/Entities/EventTicket.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: EventType

Source files:
- `src/Randevoo.Domain/Entities/EventType.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Event Type record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| Name | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/EventType.cs` |
| Description | string? | Needs Verification |  | See initializer/DbContext | nullable marker | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/EventType.cs` |
| IsActive | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for EventType; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/EventType.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| Needs Verification | Needs Verification | Needs Verification | Needs Verification | See DbContext/migrations | `src/Randevoo.Domain/Entities/EventType.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: GenderLookup

Source files:
- `src/Randevoo.Domain/Entities/GenderLookup.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Gender Lookup record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| Title | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/GenderLookup.cs` |
| IsActive | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for GenderLookup; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/GenderLookup.cs` |
| DisplayOrder | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for GenderLookup; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/GenderLookup.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| Needs Verification | Needs Verification | Needs Verification | Needs Verification | See DbContext/migrations | `src/Randevoo.Domain/Entities/GenderLookup.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: Interest

Source files:
- `src/Randevoo.Domain/Entities/Interest.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Interest record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| Name | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/Interest.cs` |
| Category | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for Interest; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/Interest.cs` |
| UsageCount | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for Interest; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/Interest.cs` |
| UserProfiles | ICollection<UserProfile> | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Navigation collection for related records. | `src/Randevoo.Domain/Entities/Interest.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| Needs Verification | Needs Verification | Needs Verification | Needs Verification | See DbContext/migrations | `src/Randevoo.Domain/Entities/Interest.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: ManualPaymentReceipt

Source files:
- `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Manual Payment Receipt record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| DatingEventId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related DatingEvent record. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| DatingEvent | DatingEvent | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for ManualPaymentReceipt; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| ParticipantUserId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related ParticipantUser record. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| ParticipantUser | User | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for ManualPaymentReceipt; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| PlannerUserId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related PlannerUser record. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| PlannerUser | User | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for ManualPaymentReceipt; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| EventTicketId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related EventTicket record. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| EventTicket | EventTicket? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for ManualPaymentReceipt; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| TicketOrderId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related TicketOrder record. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| TicketOrder | TicketOrder? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for ManualPaymentReceipt; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| EventDiscountCodeId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related EventDiscountCode record. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| EventDiscountCode | EventDiscountCode? | Needs Verification |  | See initializer/DbContext | nullable marker | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| DiscountCode | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| PaymentCollectionMethod | EventPaymentCollectionMethod | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for ManualPaymentReceipt; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| DestinationType | ManualPaymentDestinationType | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for ManualPaymentReceipt; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| OriginalAmount | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| DiscountAmount | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| Amount | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| CurrencyCode | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| ReportingCurrencyCode | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| ReportingAmountIrr | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| ExchangeRateToIrr | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| ExchangeRateCapturedAtUtc | DateTime | Likely required |  | See initializer/DbContext | See DbContext/migrations | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| ExchangeRateId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related ExchangeRate record. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| ExchangeRate | CurrencyExchangeRate? | Needs Verification |  | See initializer/DbContext | nullable marker | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| UploadedFilePath | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Business data for ManualPaymentReceipt; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| TrackingNumber | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for ManualPaymentReceipt; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| PayerNote | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for ManualPaymentReceipt; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| Status | ManualPaymentReceiptStatus | Likely required |  | See initializer/DbContext | See DbContext/migrations | Lifecycle/status value used by business workflows. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| SubmittedAtUtc | DateTime | Likely required |  | See initializer/DbContext | See DbContext/migrations | Timestamp for creation/submission lifecycle tracking. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| ReviewedByUserId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related ReviewedByUser record. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| ReviewedByUser | User? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for ManualPaymentReceipt; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| ReviewedAtUtc | DateTime? | Needs Verification |  | See initializer/DbContext | nullable marker | Timestamp for lifecycle state or audit tracking. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| RejectReason | string? | Needs Verification |  | See initializer/DbContext | nullable marker | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| ManualPaymentReceipt.DatingEventId | Many-to-one candidate | DatingEvent | DatingEventId | See DbContext | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| ManualPaymentReceipt.ParticipantUserId | Many-to-one candidate | ParticipantUser | ParticipantUserId | See DbContext | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| ManualPaymentReceipt.PlannerUserId | Many-to-one candidate | PlannerUser | PlannerUserId | See DbContext | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| ManualPaymentReceipt.EventTicketId | Many-to-one candidate | EventTicket | EventTicketId | See DbContext | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| ManualPaymentReceipt.TicketOrderId | Many-to-one candidate | TicketOrder | TicketOrderId | See DbContext | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| ManualPaymentReceipt.EventDiscountCodeId | Many-to-one candidate | EventDiscountCode | EventDiscountCodeId | See DbContext | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| ManualPaymentReceipt.ExchangeRateId | Many-to-one candidate | ExchangeRate | ExchangeRateId | See DbContext | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |
| ManualPaymentReceipt.ReviewedByUserId | Many-to-one candidate | ReviewedByUser | ReviewedByUserId | See DbContext | `src/Randevoo.Domain/Entities/ManualPaymentReceipt.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: ModerationReport

Source files:
- `src/Randevoo.Domain/Entities/ModerationReport.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Moderation Report record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| ReporterUserId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related ReporterUser record. | `src/Randevoo.Domain/Entities/ModerationReport.cs` |
| ReporterUser | User | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for ModerationReport; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/ModerationReport.cs` |
| ReportedUserId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related ReportedUser record. | `src/Randevoo.Domain/Entities/ModerationReport.cs` |
| ReportedUser | User | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for ModerationReport; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/ModerationReport.cs` |
| DatingEventId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related DatingEvent record. | `src/Randevoo.Domain/Entities/ModerationReport.cs` |
| DatingEvent | DatingEvent? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for ModerationReport; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/ModerationReport.cs` |
| EventConversationId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related EventConversation record. | `src/Randevoo.Domain/Entities/ModerationReport.cs` |
| EventConversation | EventConversation? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for ModerationReport; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/ModerationReport.cs` |
| Reason | ModerationReportReason | Likely required |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/ModerationReport.cs` |
| Description | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/ModerationReport.cs` |
| Status | ModerationReportStatus | Likely required |  | See initializer/DbContext | See DbContext/migrations | Lifecycle/status value used by business workflows. | `src/Randevoo.Domain/Entities/ModerationReport.cs` |
| AdminReviewNote | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for ModerationReport; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/ModerationReport.cs` |
| ReviewedByAdminUserId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related ReviewedByAdminUser record. | `src/Randevoo.Domain/Entities/ModerationReport.cs` |
| ReviewedByAdminUser | User? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for ModerationReport; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/ModerationReport.cs` |
| ReviewedAt | DateTime? | Needs Verification |  | See initializer/DbContext | nullable marker | Timestamp for lifecycle state or audit tracking. | `src/Randevoo.Domain/Entities/ModerationReport.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| ModerationReport.ReporterUserId | Many-to-one candidate | ReporterUser | ReporterUserId | See DbContext | `src/Randevoo.Domain/Entities/ModerationReport.cs` |
| ModerationReport.ReportedUserId | Many-to-one candidate | ReportedUser | ReportedUserId | See DbContext | `src/Randevoo.Domain/Entities/ModerationReport.cs` |
| ModerationReport.DatingEventId | Many-to-one candidate | DatingEvent | DatingEventId | See DbContext | `src/Randevoo.Domain/Entities/ModerationReport.cs` |
| ModerationReport.EventConversationId | Many-to-one candidate | EventConversation | EventConversationId | See DbContext | `src/Randevoo.Domain/Entities/ModerationReport.cs` |
| ModerationReport.ReviewedByAdminUserId | Many-to-one candidate | ReviewedByAdminUser | ReviewedByAdminUserId | See DbContext | `src/Randevoo.Domain/Entities/ModerationReport.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: OnlineEventPlatform

Source files:
- `src/Randevoo.Domain/Entities/OnlineEventPlatform.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Online Event Platform record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| Name | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/OnlineEventPlatform.cs` |
| IsActive | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for OnlineEventPlatform; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/OnlineEventPlatform.cs` |
| DisplayOrder | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for OnlineEventPlatform; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/OnlineEventPlatform.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| Needs Verification | Needs Verification | Needs Verification | Needs Verification | See DbContext/migrations | `src/Randevoo.Domain/Entities/OnlineEventPlatform.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: OnlinePayment

Source files:
- `src/Randevoo.Domain/Entities/OnlinePayment.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Online Payment record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| UserId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related User record. | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |
| User | User | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for OnlinePayment; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |
| DatingEventId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related DatingEvent record. | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |
| DatingEvent | DatingEvent? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for OnlinePayment; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |
| EventTicketId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related EventTicket record. | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |
| EventTicket | EventTicket? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for OnlinePayment; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |
| TicketOrderId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related TicketOrder record. | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |
| TicketOrder | TicketOrder? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for OnlinePayment; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |
| BalanceTransactionId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related BalanceTransaction record. | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |
| BalanceTransaction | BalanceTransaction? | Needs Verification |  | See initializer/DbContext | nullable marker | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |
| Amount | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |
| CurrencyCode | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |
| ReportingAmountIrr | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |
| ExchangeRateToIrr | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |
| ExchangeRateCapturedAtUtc | DateTime | Likely required |  | See initializer/DbContext | See DbContext/migrations | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |
| ExchangeRateId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related ExchangeRate record. | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |
| ExchangeRate | CurrencyExchangeRate? | Needs Verification |  | See initializer/DbContext | nullable marker | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |
| GatewayName | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |
| TrackingCode | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |
| Status | OnlinePaymentStatus | Likely required |  | See initializer/DbContext | See DbContext/migrations | Lifecycle/status value used by business workflows. | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |
| PaidAtUtc | DateTime? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for OnlinePayment; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |
| FailureReason | string? | Needs Verification |  | See initializer/DbContext | nullable marker | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| OnlinePayment.UserId | Many-to-one candidate | User | UserId | See DbContext | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |
| OnlinePayment.DatingEventId | Many-to-one candidate | DatingEvent | DatingEventId | See DbContext | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |
| OnlinePayment.EventTicketId | Many-to-one candidate | EventTicket | EventTicketId | See DbContext | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |
| OnlinePayment.TicketOrderId | Many-to-one candidate | TicketOrder | TicketOrderId | See DbContext | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |
| OnlinePayment.BalanceTransactionId | Many-to-one candidate | BalanceTransaction | BalanceTransactionId | See DbContext | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |
| OnlinePayment.ExchangeRateId | Many-to-one candidate | ExchangeRate | ExchangeRateId | See DbContext | `src/Randevoo.Domain/Entities/OnlinePayment.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: PermissionAction

Source files:
- `src/Randevoo.Domain/Entities/PermissionAction.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Permission Action record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| Entity | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Business data for PermissionAction; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/PermissionAction.cs` |
| Action | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Business data for PermissionAction; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/PermissionAction.cs` |
| EntityLabel | string | Likely required |  | Empty string | Max 120 | User-facing entity label shown in the operation permission tree. | `src/Randevoo.Domain/Entities/PermissionAction.cs` |
| Label | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Business data for PermissionAction; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/PermissionAction.cs` |
| Description | string? | Needs Verification |  | See initializer/DbContext | nullable marker | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/PermissionAction.cs` |
| GroupKey | string | Likely required |  | Empty string | Max 80 | Catalog group key used by the operation permission tree. | `src/Randevoo.Domain/Entities/PermissionAction.cs` |
| GroupLabel | string | Likely required |  | Empty string | Max 120 | User-facing catalog group label. | `src/Randevoo.Domain/Entities/PermissionAction.cs` |
| PagePath | string? | Optional |  | null | Max 160 | AdminPanel page path associated with the operation. | `src/Randevoo.Domain/Entities/PermissionAction.cs` |
| HandlerName | string? | Optional |  | null | Max 120 | Razor Page handler or logical handler associated with the operation. | `src/Randevoo.Domain/Entities/PermissionAction.cs` |
| UiSurface | string | Likely required |  | Manual | Max 40 | Operation surface such as PageAccess, GridAction, FormSubmit, Export, SensitiveData, or SensitiveAction. | `src/Randevoo.Domain/Entities/PermissionAction.cs` |
| RiskLevel | string | Likely required |  | Low | Max 20 | Operational risk label used for filtering and visual badges. | `src/Randevoo.Domain/Entities/PermissionAction.cs` |
| IsActive | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for PermissionAction; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/PermissionAction.cs` |
| IsSystemAction | bool | Likely required |  | true | See DbContext/migrations | Indicates the action came from the system catalog rather than a manual custom row. | `src/Randevoo.Domain/Entities/PermissionAction.cs` |
| IsDeprecated | bool | Likely required |  | false | See DbContext/migrations | Marks old catalog actions that should remain visible for audit but are no longer current. | `src/Randevoo.Domain/Entities/PermissionAction.cs` |
| DisplayOrder | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for PermissionAction; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/PermissionAction.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| Needs Verification | Needs Verification | Needs Verification | Needs Verification | See DbContext/migrations | `src/Randevoo.Domain/Entities/PermissionAction.cs` |

Business rules:
- `OperationPermissionCatalog` is the source of truth for system permission actions.
- Startup sync creates missing catalog rows, refreshes metadata, creates missing role permission rows for admin-panel roles, and marks removed system actions inactive/deprecated.
- Do not hard-delete permission actions that may be referenced by role permissions or user overrides.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: PlannerBankAccount

Source files:
- `src/Randevoo.Domain/Entities/PlannerBankAccount.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Planner Bank Account record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| UserId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related User record. | `src/Randevoo.Domain/Entities/PlannerBankAccount.cs` |
| User | User | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for PlannerBankAccount; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/PlannerBankAccount.cs` |
| CurrencyCode | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/PlannerBankAccount.cs` |
| PayoutMethod | PlannerPayoutMethod | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for PlannerBankAccount; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/PlannerBankAccount.cs` |
| AccountHolderName | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/PlannerBankAccount.cs` |
| Country | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for PlannerBankAccount; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/PlannerBankAccount.cs` |
| CardNumber | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for PlannerBankAccount; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/PlannerBankAccount.cs` |
| Iban | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for PlannerBankAccount; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/PlannerBankAccount.cs` |
| BankName | string? | Needs Verification |  | See initializer/DbContext | nullable marker | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/PlannerBankAccount.cs` |
| AccountNumber | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for PlannerBankAccount; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/PlannerBankAccount.cs` |
| SwiftCode | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/PlannerBankAccount.cs` |
| AccountIdentifier | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for PlannerBankAccount; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/PlannerBankAccount.cs` |
| PublicPaymentInstructions | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for PlannerBankAccount; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/PlannerBankAccount.cs` |
| IsActive | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for PlannerBankAccount; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/PlannerBankAccount.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| PlannerBankAccount.UserId | Many-to-one candidate | User | UserId | See DbContext | `src/Randevoo.Domain/Entities/PlannerBankAccount.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: PlannerWithdrawalRequest

Source files:
- `src/Randevoo.Domain/Entities/PlannerWithdrawalRequest.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Planner Withdrawal Request record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| UserId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related User record. | `src/Randevoo.Domain/Entities/PlannerWithdrawalRequest.cs` |
| User | User | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for PlannerWithdrawalRequest; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/PlannerWithdrawalRequest.cs` |
| Amount | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/PlannerWithdrawalRequest.cs` |
| CurrencyCode | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/PlannerWithdrawalRequest.cs` |
| ReportingAmountIrr | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/PlannerWithdrawalRequest.cs` |
| ExchangeRateToIrr | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/PlannerWithdrawalRequest.cs` |
| ExchangeRateCapturedAtUtc | DateTime | Likely required |  | See initializer/DbContext | See DbContext/migrations | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/PlannerWithdrawalRequest.cs` |
| ExchangeRateId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related ExchangeRate record. | `src/Randevoo.Domain/Entities/PlannerWithdrawalRequest.cs` |
| ExchangeRate | CurrencyExchangeRate? | Needs Verification |  | See initializer/DbContext | nullable marker | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/PlannerWithdrawalRequest.cs` |
| Status | PlannerWithdrawalRequestStatus | Likely required |  | See initializer/DbContext | See DbContext/migrations | Lifecycle/status value used by business workflows. | `src/Randevoo.Domain/Entities/PlannerWithdrawalRequest.cs` |
| RequestedAtUtc | DateTime | Likely required |  | See initializer/DbContext | See DbContext/migrations | Timestamp for creation/submission lifecycle tracking. | `src/Randevoo.Domain/Entities/PlannerWithdrawalRequest.cs` |
| ReviewedAtUtc | DateTime? | Needs Verification |  | See initializer/DbContext | nullable marker | Timestamp for lifecycle state or audit tracking. | `src/Randevoo.Domain/Entities/PlannerWithdrawalRequest.cs` |
| ReviewedByAdminUserId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related ReviewedByAdminUser record. | `src/Randevoo.Domain/Entities/PlannerWithdrawalRequest.cs` |
| ReviewedByAdminUser | User? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for PlannerWithdrawalRequest; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/PlannerWithdrawalRequest.cs` |
| ReviewNote | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for PlannerWithdrawalRequest; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/PlannerWithdrawalRequest.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| PlannerWithdrawalRequest.UserId | Many-to-one candidate | User | UserId | See DbContext | `src/Randevoo.Domain/Entities/PlannerWithdrawalRequest.cs` |
| PlannerWithdrawalRequest.ExchangeRateId | Many-to-one candidate | ExchangeRate | ExchangeRateId | See DbContext | `src/Randevoo.Domain/Entities/PlannerWithdrawalRequest.cs` |
| PlannerWithdrawalRequest.ReviewedByAdminUserId | Many-to-one candidate | ReviewedByAdminUser | ReviewedByAdminUserId | See DbContext | `src/Randevoo.Domain/Entities/PlannerWithdrawalRequest.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: RefreshToken

Source files:
- `src/Randevoo.Domain/Entities/RefreshToken.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Refresh Token record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| UserId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related User record. | `src/Randevoo.Domain/Entities/RefreshToken.cs` |
| TokenHash | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/RefreshToken.cs` |
| ExpiresAt | DateTime | Likely required |  | See initializer/DbContext | See DbContext/migrations | Timestamp for lifecycle state or audit tracking. | `src/Randevoo.Domain/Entities/RefreshToken.cs` |
| RevokedAt | DateTime? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for RefreshToken; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/RefreshToken.cs` |
| ReplacedByTokenHash | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/RefreshToken.cs` |
| User | User | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for RefreshToken; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/RefreshToken.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| RefreshToken.UserId | Many-to-one candidate | User | UserId | See DbContext | `src/Randevoo.Domain/Entities/RefreshToken.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: RoleOperationPermission

Source files:
- `src/Randevoo.Domain/Entities/RoleOperationPermission.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Role Operation Permission record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| Role | UserRole | Likely required |  | See initializer/DbContext | See DbContext/migrations | Role/authorization classification. | `src/Randevoo.Domain/Entities/RoleOperationPermission.cs` |
| Entity | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Business data for RoleOperationPermission; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/RoleOperationPermission.cs` |
| Action | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Business data for RoleOperationPermission; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/RoleOperationPermission.cs` |
| Allowed | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for RoleOperationPermission; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/RoleOperationPermission.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| Needs Verification | Needs Verification | Needs Verification | Needs Verification | See DbContext/migrations | `src/Randevoo.Domain/Entities/RoleOperationPermission.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: SmsQueueItem

Source files:
- `src/Randevoo.Domain/Entities/SmsQueueItem.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Sms Queue Item record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| EventParticipantSmsRequestId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related EventParticipantSmsRequest record. | `src/Randevoo.Domain/Entities/SmsQueueItem.cs` |
| EventParticipantSmsRequest | EventParticipantSmsRequest? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for SmsQueueItem; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/SmsQueueItem.cs` |
| DatingEventId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related DatingEvent record. | `src/Randevoo.Domain/Entities/SmsQueueItem.cs` |
| DatingEvent | DatingEvent | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for SmsQueueItem; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/SmsQueueItem.cs` |
| RecipientUserId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related RecipientUser record. | `src/Randevoo.Domain/Entities/SmsQueueItem.cs` |
| RecipientUser | User | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for SmsQueueItem; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/SmsQueueItem.cs` |
| MobileNumber | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User contact/authentication data; privacy-sensitive. | `src/Randevoo.Domain/Entities/SmsQueueItem.cs` |
| Message | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/SmsQueueItem.cs` |
| PlannedSendAtUtc | DateTime? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for SmsQueueItem; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/SmsQueueItem.cs` |
| Status | SmsQueueItemStatus | Likely required |  | See initializer/DbContext | See DbContext/migrations | Lifecycle/status value used by business workflows. | `src/Randevoo.Domain/Entities/SmsQueueItem.cs` |
| AttemptCount | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for SmsQueueItem; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/SmsQueueItem.cs` |
| SentAt | DateTime? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for SmsQueueItem; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/SmsQueueItem.cs` |
| FailureReason | string? | Needs Verification |  | See initializer/DbContext | nullable marker | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/SmsQueueItem.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| SmsQueueItem.EventParticipantSmsRequestId | Many-to-one candidate | EventParticipantSmsRequest | EventParticipantSmsRequestId | See DbContext | `src/Randevoo.Domain/Entities/SmsQueueItem.cs` |
| SmsQueueItem.DatingEventId | Many-to-one candidate | DatingEvent | DatingEventId | See DbContext | `src/Randevoo.Domain/Entities/SmsQueueItem.cs` |
| SmsQueueItem.RecipientUserId | Many-to-one candidate | RecipientUser | RecipientUserId | See DbContext | `src/Randevoo.Domain/Entities/SmsQueueItem.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: SupportTicket

Source files:
- `src/Randevoo.Domain/Entities/SupportTicket.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Support Ticket record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| Title | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/SupportTicket.cs` |
| TicketTypeId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related TicketType record. | `src/Randevoo.Domain/Entities/SupportTicket.cs` |
| TicketType | SupportTicketCategoryLookup | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for SupportTicket; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/SupportTicket.cs` |
| TicketStatusId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related TicketStatus record. | `src/Randevoo.Domain/Entities/SupportTicket.cs` |
| TicketStatus | SupportTicketStatusLookup | Likely required |  | See initializer/DbContext | See DbContext/migrations | Lifecycle/status value used by business workflows. | `src/Randevoo.Domain/Entities/SupportTicket.cs` |
| TicketRecipientTypeId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related TicketRecipientType record. | `src/Randevoo.Domain/Entities/SupportTicket.cs` |
| TicketRecipientType | SupportTicketRecipientTypeLookup | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for SupportTicket; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/SupportTicket.cs` |
| Category | SupportTicketCategory | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for SupportTicket; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/SupportTicket.cs` |
| Status | SupportTicketStatus | Likely required |  | See initializer/DbContext | See DbContext/migrations | Lifecycle/status value used by business workflows. | `src/Randevoo.Domain/Entities/SupportTicket.cs` |
| SubmitterUserId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related SubmitterUser record. | `src/Randevoo.Domain/Entities/SupportTicket.cs` |
| SubmitterUser | User | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for SupportTicket; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/SupportTicket.cs` |
| SubmitterRole | UserRole | Likely required |  | See initializer/DbContext | See DbContext/migrations | Role/authorization classification. | `src/Randevoo.Domain/Entities/SupportTicket.cs` |
| AssignedSupportUserId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related AssignedSupportUser record. | `src/Randevoo.Domain/Entities/SupportTicket.cs` |
| AssignedSupportUser | User? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for SupportTicket; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/SupportTicket.cs` |
| DatingEventId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related DatingEvent record. | `src/Randevoo.Domain/Entities/SupportTicket.cs` |
| DatingEvent | DatingEvent? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for SupportTicket; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/SupportTicket.cs` |
| RecipientPlannerUserId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related RecipientPlannerUser record. | `src/Randevoo.Domain/Entities/SupportTicket.cs` |
| RecipientPlannerUser | User? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for SupportTicket; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/SupportTicket.cs` |
| ClosedAtUtc | DateTime? | Needs Verification |  | See initializer/DbContext | nullable marker | Timestamp for lifecycle state or audit tracking. | `src/Randevoo.Domain/Entities/SupportTicket.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| SupportTicket.TicketTypeId | Many-to-one candidate | TicketType | TicketTypeId | See DbContext | `src/Randevoo.Domain/Entities/SupportTicket.cs` |
| SupportTicket.TicketStatusId | Many-to-one candidate | TicketStatus | TicketStatusId | See DbContext | `src/Randevoo.Domain/Entities/SupportTicket.cs` |
| SupportTicket.TicketRecipientTypeId | Many-to-one candidate | TicketRecipientType | TicketRecipientTypeId | See DbContext | `src/Randevoo.Domain/Entities/SupportTicket.cs` |
| SupportTicket.SubmitterUserId | Many-to-one candidate | SubmitterUser | SubmitterUserId | See DbContext | `src/Randevoo.Domain/Entities/SupportTicket.cs` |
| SupportTicket.AssignedSupportUserId | Many-to-one candidate | AssignedSupportUser | AssignedSupportUserId | See DbContext | `src/Randevoo.Domain/Entities/SupportTicket.cs` |
| SupportTicket.DatingEventId | Many-to-one candidate | DatingEvent | DatingEventId | See DbContext | `src/Randevoo.Domain/Entities/SupportTicket.cs` |
| SupportTicket.RecipientPlannerUserId | Many-to-one candidate | RecipientPlannerUser | RecipientPlannerUserId | See DbContext | `src/Randevoo.Domain/Entities/SupportTicket.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: SupportTicketAssignmentCursor

Source files:
- `src/Randevoo.Domain/Entities/SupportTicketAssignmentCursor.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Support Ticket Assignment Cursor record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| QueueName | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/SupportTicketAssignmentCursor.cs` |
| LastAssignedUserId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related LastAssignedUser record. | `src/Randevoo.Domain/Entities/SupportTicketAssignmentCursor.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| SupportTicketAssignmentCursor.LastAssignedUserId | Many-to-one candidate | LastAssignedUser | LastAssignedUserId | See DbContext | `src/Randevoo.Domain/Entities/SupportTicketAssignmentCursor.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: SupportTicketAttachment

Source files:
- `src/Randevoo.Domain/Entities/SupportTicketAttachment.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Support Ticket Attachment record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| SupportTicketMessageId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related SupportTicketMessage record. | `src/Randevoo.Domain/Entities/SupportTicketAttachment.cs` |
| Message | SupportTicketMessage | Likely required |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/SupportTicketAttachment.cs` |
| FileName | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/SupportTicketAttachment.cs` |
| ContentType | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/SupportTicketAttachment.cs` |
| SizeBytes | long | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for SupportTicketAttachment; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/SupportTicketAttachment.cs` |
| Url | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Business data for SupportTicketAttachment; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/SupportTicketAttachment.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| SupportTicketAttachment.SupportTicketMessageId | Many-to-one candidate | SupportTicketMessage | SupportTicketMessageId | See DbContext | `src/Randevoo.Domain/Entities/SupportTicketAttachment.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: SupportTicketCategoryLookup

Source files:
- `src/Randevoo.Domain/Entities/SupportTicketCategoryLookup.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Support Ticket Category Lookup record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| Name | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/SupportTicketCategoryLookup.cs` |
| DisplayNameFa | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/SupportTicketCategoryLookup.cs` |
| IsActive | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for SupportTicketCategoryLookup; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/SupportTicketCategoryLookup.cs` |
| DisplayOrder | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for SupportTicketCategoryLookup; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/SupportTicketCategoryLookup.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| Needs Verification | Needs Verification | Needs Verification | Needs Verification | See DbContext/migrations | `src/Randevoo.Domain/Entities/SupportTicketCategoryLookup.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: SupportTicketHistoryEntry

Source files:
- `src/Randevoo.Domain/Entities/SupportTicketHistoryEntry.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Support Ticket History Entry record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| SupportTicketId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related SupportTicket record. | `src/Randevoo.Domain/Entities/SupportTicketHistoryEntry.cs` |
| SupportTicket | SupportTicket | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for SupportTicketHistoryEntry; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/SupportTicketHistoryEntry.cs` |
| ActorUserId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related ActorUser record. | `src/Randevoo.Domain/Entities/SupportTicketHistoryEntry.cs` |
| ActorUser | User | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for SupportTicketHistoryEntry; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/SupportTicketHistoryEntry.cs` |
| Action | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Business data for SupportTicketHistoryEntry; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/SupportTicketHistoryEntry.cs` |
| OldStatus | SupportTicketStatus? | Needs Verification |  | See initializer/DbContext | nullable marker | Lifecycle/status value used by business workflows. | `src/Randevoo.Domain/Entities/SupportTicketHistoryEntry.cs` |
| NewStatus | SupportTicketStatus? | Needs Verification |  | See initializer/DbContext | nullable marker | Lifecycle/status value used by business workflows. | `src/Randevoo.Domain/Entities/SupportTicketHistoryEntry.cs` |
| OldAssigneeUserId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related OldAssigneeUser record. | `src/Randevoo.Domain/Entities/SupportTicketHistoryEntry.cs` |
| NewAssigneeUserId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related NewAssigneeUser record. | `src/Randevoo.Domain/Entities/SupportTicketHistoryEntry.cs` |
| Note | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for SupportTicketHistoryEntry; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/SupportTicketHistoryEntry.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| SupportTicketHistoryEntry.SupportTicketId | Many-to-one candidate | SupportTicket | SupportTicketId | See DbContext | `src/Randevoo.Domain/Entities/SupportTicketHistoryEntry.cs` |
| SupportTicketHistoryEntry.ActorUserId | Many-to-one candidate | ActorUser | ActorUserId | See DbContext | `src/Randevoo.Domain/Entities/SupportTicketHistoryEntry.cs` |
| SupportTicketHistoryEntry.OldAssigneeUserId | Many-to-one candidate | OldAssigneeUser | OldAssigneeUserId | See DbContext | `src/Randevoo.Domain/Entities/SupportTicketHistoryEntry.cs` |
| SupportTicketHistoryEntry.NewAssigneeUserId | Many-to-one candidate | NewAssigneeUser | NewAssigneeUserId | See DbContext | `src/Randevoo.Domain/Entities/SupportTicketHistoryEntry.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: SupportTicketMessage

Source files:
- `src/Randevoo.Domain/Entities/SupportTicketMessage.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Support Ticket Message record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| SupportTicketId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related SupportTicket record. | `src/Randevoo.Domain/Entities/SupportTicketMessage.cs` |
| SupportTicket | SupportTicket | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for SupportTicketMessage; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/SupportTicketMessage.cs` |
| SenderUserId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related SenderUser record. | `src/Randevoo.Domain/Entities/SupportTicketMessage.cs` |
| SenderUser | User | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for SupportTicketMessage; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/SupportTicketMessage.cs` |
| SenderRole | UserRole | Likely required |  | See initializer/DbContext | See DbContext/migrations | Role/authorization classification. | `src/Randevoo.Domain/Entities/SupportTicketMessage.cs` |
| RepresentedUserId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related RepresentedUser record. | `src/Randevoo.Domain/Entities/SupportTicketMessage.cs` |
| RepresentedUser | User? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for SupportTicketMessage; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/SupportTicketMessage.cs` |
| Body | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Business data for SupportTicketMessage; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/SupportTicketMessage.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| SupportTicketMessage.SupportTicketId | Many-to-one candidate | SupportTicket | SupportTicketId | See DbContext | `src/Randevoo.Domain/Entities/SupportTicketMessage.cs` |
| SupportTicketMessage.SenderUserId | Many-to-one candidate | SenderUser | SenderUserId | See DbContext | `src/Randevoo.Domain/Entities/SupportTicketMessage.cs` |
| SupportTicketMessage.RepresentedUserId | Many-to-one candidate | RepresentedUser | RepresentedUserId | See DbContext | `src/Randevoo.Domain/Entities/SupportTicketMessage.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: SupportTicketRecipientTypeLookup

Source files:
- `src/Randevoo.Domain/Entities/SupportTicketRecipientTypeLookup.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Support Ticket Recipient Type Lookup record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| Name | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/SupportTicketRecipientTypeLookup.cs` |
| DisplayNameFa | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/SupportTicketRecipientTypeLookup.cs` |
| IsActive | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for SupportTicketRecipientTypeLookup; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/SupportTicketRecipientTypeLookup.cs` |
| DisplayOrder | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for SupportTicketRecipientTypeLookup; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/SupportTicketRecipientTypeLookup.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| Needs Verification | Needs Verification | Needs Verification | Needs Verification | See DbContext/migrations | `src/Randevoo.Domain/Entities/SupportTicketRecipientTypeLookup.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: SupportTicketStatusLookup

Source files:
- `src/Randevoo.Domain/Entities/SupportTicketStatusLookup.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Support Ticket Status Lookup record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| Name | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/SupportTicketStatusLookup.cs` |
| DisplayNameFa | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/SupportTicketStatusLookup.cs` |
| IsActive | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for SupportTicketStatusLookup; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/SupportTicketStatusLookup.cs` |
| DisplayOrder | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for SupportTicketStatusLookup; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/SupportTicketStatusLookup.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| Needs Verification | Needs Verification | Needs Verification | Needs Verification | See DbContext/migrations | `src/Randevoo.Domain/Entities/SupportTicketStatusLookup.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: Tag

Source files:
- `src/Randevoo.Domain/Entities/Tag.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Tag record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| Name | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/Tag.cs` |
| IsActive | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for Tag; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/Tag.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| Needs Verification | Needs Verification | Needs Verification | Needs Verification | See DbContext/migrations | `src/Randevoo.Domain/Entities/Tag.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: TicketOrder

Source files:
- `src/Randevoo.Domain/Entities/TicketOrder.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Ticket Order record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| DatingEventId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related DatingEvent record. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| DatingEvent | DatingEvent | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for TicketOrder; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| BuyerUserId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related BuyerUser record. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| BuyerUser | User | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for TicketOrder; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| CurrencyCode | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| GrossAmount | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| DiscountAmount | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| NetAmount | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| PlatformCommissionAmount | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| OrganizerIncomeAmount | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| PaymentCollectionMethod | EventPaymentCollectionMethod | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for TicketOrder; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| PaymentStatus | TicketOrderPaymentStatus | Likely required |  | See initializer/DbContext | See DbContext/migrations | Lifecycle/status value used by business workflows. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| OrderStatus | TicketOrderStatus | Likely required |  | See initializer/DbContext | See DbContext/migrations | Lifecycle/status value used by business workflows. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| EventDiscountCodeId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related EventDiscountCode record. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| EventDiscountCode | EventDiscountCode? | Needs Verification |  | See initializer/DbContext | nullable marker | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| DiscountCode | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| ReportingCurrencyCode | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| ReportingGrossAmountIrr | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| ReportingDiscountAmountIrr | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| ReportingNetAmountIrr | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| ReportingPlatformCommissionIrr | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| ReportingOrganizerIncomeIrr | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Business data for TicketOrder; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| ExchangeRateToIrr | decimal | Likely required |  | See initializer/DbContext | numeric precision configured where visible | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| ExchangeRateCapturedAtUtc | DateTime | Likely required |  | See initializer/DbContext | See DbContext/migrations | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| ExchangeRateId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related ExchangeRate record. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| ExchangeRate | CurrencyExchangeRate? | Needs Verification |  | See initializer/DbContext | nullable marker | Financial amount/rate used in payment or settlement flows. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| PaidAtUtc | DateTime? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for TicketOrder; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| ApprovedAtUtc | DateTime? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for TicketOrder; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| ApprovedByUserId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related ApprovedByUser record. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| ApprovedByUser | User? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for TicketOrder; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| Notes | string? | Needs Verification |  | See initializer/DbContext | nullable marker | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/TicketOrder.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| TicketOrder.DatingEventId | Many-to-one candidate | DatingEvent | DatingEventId | See DbContext | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| TicketOrder.BuyerUserId | Many-to-one candidate | BuyerUser | BuyerUserId | See DbContext | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| TicketOrder.EventDiscountCodeId | Many-to-one candidate | EventDiscountCode | EventDiscountCodeId | See DbContext | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| TicketOrder.ExchangeRateId | Many-to-one candidate | ExchangeRate | ExchangeRateId | See DbContext | `src/Randevoo.Domain/Entities/TicketOrder.cs` |
| TicketOrder.ApprovedByUserId | Many-to-one candidate | ApprovedByUser | ApprovedByUserId | See DbContext | `src/Randevoo.Domain/Entities/TicketOrder.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: User

Source files:
- `src/Randevoo.Domain/Entities/User.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
User record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| MobileNumber | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User contact/authentication data; privacy-sensitive. | `src/Randevoo.Domain/Entities/User.cs` |
| Email | string? | Needs Verification |  | See initializer/DbContext | nullable marker | User contact/authentication data; privacy-sensitive. | `src/Randevoo.Domain/Entities/User.cs` |
| IsEmailConfirmed | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | User contact/authentication data; privacy-sensitive. | `src/Randevoo.Domain/Entities/User.cs` |
| PendingEmail | string? | Needs Verification |  | See initializer/DbContext | nullable marker | User contact/authentication data; privacy-sensitive. | `src/Randevoo.Domain/Entities/User.cs` |
| MobileLoginCodeHash | string? | Needs Verification |  | See initializer/DbContext | nullable marker | User contact/authentication data; privacy-sensitive. | `src/Randevoo.Domain/Entities/User.cs` |
| MobileLoginCodeExpiresAt | DateTime? | Needs Verification |  | See initializer/DbContext | nullable marker | Timestamp for lifecycle state or audit tracking. | `src/Randevoo.Domain/Entities/User.cs` |
| MobileLoginCodeRequestWindowStartedAt | DateTime? | Needs Verification |  | See initializer/DbContext | nullable marker | User contact/authentication data; privacy-sensitive. | `src/Randevoo.Domain/Entities/User.cs` |
| MobileLoginCodeRequestCount | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | User contact/authentication data; privacy-sensitive. | `src/Randevoo.Domain/Entities/User.cs` |
| MobileLoginFailedAttemptCount | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | User contact/authentication data; privacy-sensitive. | `src/Randevoo.Domain/Entities/User.cs` |
| MobileLoginLockedUntil | DateTime? | Needs Verification |  | See initializer/DbContext | nullable marker | User contact/authentication data; privacy-sensitive. | `src/Randevoo.Domain/Entities/User.cs` |
| EmailConfirmationTokenHash | string? | Needs Verification |  | See initializer/DbContext | nullable marker | User contact/authentication data; privacy-sensitive. | `src/Randevoo.Domain/Entities/User.cs` |
| EmailConfirmationTokenExpiresAt | DateTime? | Needs Verification |  | See initializer/DbContext | nullable marker | Timestamp for lifecycle state or audit tracking. | `src/Randevoo.Domain/Entities/User.cs` |
| Role | UserRole | Likely required |  | See initializer/DbContext | See DbContext/migrations | Role/authorization classification. | `src/Randevoo.Domain/Entities/User.cs` |
| IsActive | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for User; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/User.cs` |
| Profile | UserProfile? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for User; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/User.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| Needs Verification | Needs Verification | Needs Verification | Needs Verification | See DbContext/migrations | `src/Randevoo.Domain/Entities/User.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: UserOperationPermissionOverride

Source files:
- `src/Randevoo.Domain/Entities/UserOperationPermissionOverride.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
User Operation Permission Override record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| UserId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related User record. | `src/Randevoo.Domain/Entities/UserOperationPermissionOverride.cs` |
| User | User | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for UserOperationPermissionOverride; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/UserOperationPermissionOverride.cs` |
| Entity | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Business data for UserOperationPermissionOverride; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/UserOperationPermissionOverride.cs` |
| Action | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Business data for UserOperationPermissionOverride; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/UserOperationPermissionOverride.cs` |
| Allowed | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for UserOperationPermissionOverride; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/UserOperationPermissionOverride.cs` |
| Note | string? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for UserOperationPermissionOverride; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/UserOperationPermissionOverride.cs` |
| ExpiresAtUtc | DateTime? | Needs Verification |  | See initializer/DbContext | nullable marker | Timestamp for lifecycle state or audit tracking. | `src/Randevoo.Domain/Entities/UserOperationPermissionOverride.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| UserOperationPermissionOverride.UserId | Many-to-one candidate | User | UserId | See DbContext | `src/Randevoo.Domain/Entities/UserOperationPermissionOverride.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: UserProfile

Source files:
- `src/Randevoo.Domain/Entities/UserProfile.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
User Profile record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| UserId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related User record. | `src/Randevoo.Domain/Entities/UserProfile.cs` |
| User | User | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for UserProfile; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/UserProfile.cs` |
| DisplayName | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/UserProfile.cs` |
| Gender | Gender | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for UserProfile; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/UserProfile.cs` |
| GenderId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related Gender record. | `src/Randevoo.Domain/Entities/UserProfile.cs` |
| GenderLookup | GenderLookup? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for UserProfile; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/UserProfile.cs` |
| DateOfBirth | DateOnly | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for UserProfile; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/UserProfile.cs` |
| BirthMonth | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for UserProfile; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/UserProfile.cs` |
| ZodiacSign | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Business data for UserProfile; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/UserProfile.cs` |
| ZodiacSignId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related ZodiacSign record. | `src/Randevoo.Domain/Entities/UserProfile.cs` |
| ZodiacSignLookup | ZodiacSignLookup? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for UserProfile; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/UserProfile.cs` |
| Height | Height | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for UserProfile; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/UserProfile.cs` |
| EducationLevel | EducationLevel | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for UserProfile; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/UserProfile.cs` |
| EducationLevelId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related EducationLevel record. | `src/Randevoo.Domain/Entities/UserProfile.cs` |
| EducationLevelLookup | EducationLevelLookup? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for UserProfile; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/UserProfile.cs` |
| Smoking | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for UserProfile; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/UserProfile.cs` |
| Location | Location | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for UserProfile; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/UserProfile.cs` |
| CountryId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related Country record. | `src/Randevoo.Domain/Entities/UserProfile.cs` |
| Country | Country? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for UserProfile; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/UserProfile.cs` |
| CityId | long? | Needs Verification | FK convention / verify in DbContext | See initializer/DbContext | nullable marker | Reference to the related City record. | `src/Randevoo.Domain/Entities/UserProfile.cs` |
| City | City? | Needs Verification |  | See initializer/DbContext | nullable marker | Business data for UserProfile; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/UserProfile.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| UserProfile.UserId | Many-to-one candidate | User | UserId | See DbContext | `src/Randevoo.Domain/Entities/UserProfile.cs` |
| UserProfile.GenderId | Many-to-one candidate | Gender | GenderId | See DbContext | `src/Randevoo.Domain/Entities/UserProfile.cs` |
| UserProfile.ZodiacSignId | Many-to-one candidate | ZodiacSign | ZodiacSignId | See DbContext | `src/Randevoo.Domain/Entities/UserProfile.cs` |
| UserProfile.EducationLevelId | Many-to-one candidate | EducationLevel | EducationLevelId | See DbContext | `src/Randevoo.Domain/Entities/UserProfile.cs` |
| UserProfile.CountryId | Many-to-one candidate | Country | CountryId | See DbContext | `src/Randevoo.Domain/Entities/UserProfile.cs` |
| UserProfile.CityId | Many-to-one candidate | City | CityId | See DbContext | `src/Randevoo.Domain/Entities/UserProfile.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: UserProfileImage

Source files:
- `src/Randevoo.Domain/Entities/UserProfileImage.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
User Profile Image record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| UserProfileId | long | Likely required | FK convention / verify in DbContext | See initializer/DbContext | See DbContext/migrations | Reference to the related UserProfile record. | `src/Randevoo.Domain/Entities/UserProfileImage.cs` |
| UserProfile | UserProfile | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for UserProfileImage; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/UserProfileImage.cs` |
| ImageUrl | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Business data for UserProfileImage; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/UserProfileImage.cs` |
| DisplayOrder | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for UserProfileImage; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/UserProfileImage.cs` |
| IsPrimary | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for UserProfileImage; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/UserProfileImage.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| UserProfileImage.UserProfileId | Many-to-one candidate | UserProfile | UserProfileId | See DbContext | `src/Randevoo.Domain/Entities/UserProfileImage.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: UserRoleLookup

Source files:
- `src/Randevoo.Domain/Entities/UserRoleLookup.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
User Role Lookup record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| Name | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/UserRoleLookup.cs` |
| DisplayNameFa | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/UserRoleLookup.cs` |
| IsActive | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for UserRoleLookup; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/UserRoleLookup.cs` |
| DisplayOrder | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for UserRoleLookup; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/UserRoleLookup.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| Needs Verification | Needs Verification | Needs Verification | Needs Verification | See DbContext/migrations | `src/Randevoo.Domain/Entities/UserRoleLookup.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Table/Entity: ZodiacSignLookup

Source files:
- `src/Randevoo.Domain/Entities/ZodiacSignLookup.cs`
- `src/Randevoo.Infrastructure/Data/RandevooDbContext.cs`

Purpose:
Zodiac Sign Lookup record in the Randevoo domain. Business meaning is inferred from name and related handlers; verify edge cases before changing schema.

Fields:

| Field | Type | Required | Key | Default | Constraints | Business Meaning | Source |
| ----- | ---- | -------- | --- | ------- | ----------- | ---------------- | ------ |
| Code | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | Security credential or verification data; must be protected. | `src/Randevoo.Domain/Entities/ZodiacSignLookup.cs` |
| Title | string | Needs Verification |  | See initializer/DbContext | See DbContext/migrations | User-facing or operational descriptive text. | `src/Randevoo.Domain/Entities/ZodiacSignLookup.cs` |
| IsActive | bool | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for ZodiacSignLookup; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/ZodiacSignLookup.cs` |
| DisplayOrder | int | Likely required |  | See initializer/DbContext | See DbContext/migrations | Business data for ZodiacSignLookup; exact meaning should be verified with handlers and UI. | `src/Randevoo.Domain/Entities/ZodiacSignLookup.cs` |

Relationships:

| Relationship | Type | Target | Foreign Key | Delete Behavior | Source |
| ------------ | ---- | ------ | ----------- | --------------- | ------ |
| Needs Verification | Needs Verification | Needs Verification | Needs Verification | See DbContext/migrations | `src/Randevoo.Domain/Entities/ZodiacSignLookup.cs` |

Business rules:
- Respect lifecycle/status fields and repository handler behavior for this entity.
- Preserve audit and financial references where delete behavior is restrictive.

Notes:
- Indexes and constraints are centralized in DbContext and migrations; review before altering fields.

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
