# Enums

## Purpose
Catalog domain enums and lifecycle/status values.

## Current implementation summary
This document was generated from the current repository snapshot. It references source paths where evidence exists and marks uncertain areas as Needs Verification.

## Important source files
- `src/Randevoo.Domain/Enums/BalanceTransactionType.cs`
- `src/Randevoo.Domain/Enums/EducationLevel.cs`
- `src/Randevoo.Domain/Enums/EventDiscountGenderScope.cs`
- `src/Randevoo.Domain/Enums/EventDiscountType.cs`
- `src/Randevoo.Domain/Enums/EventEducationLevelRestriction.cs`
- `src/Randevoo.Domain/Enums/EventLikeStatus.cs`
- `src/Randevoo.Domain/Enums/EventOperationalStatus.cs`
- `src/Randevoo.Domain/Enums/EventParticipantSmsRequestStatus.cs`
- `src/Randevoo.Domain/Enums/EventPaymentCollectionMethod.cs`
- `src/Randevoo.Domain/Enums/EventReviewStatus.cs`
- `src/Randevoo.Domain/Enums/Gender.cs`
- `src/Randevoo.Domain/Enums/ManualPaymentDestinationType.cs`
- `src/Randevoo.Domain/Enums/ManualPaymentReceiptStatus.cs`
- `src/Randevoo.Domain/Enums/ModerationReportReason.cs`
- `src/Randevoo.Domain/Enums/ModerationReportStatus.cs`
- `src/Randevoo.Domain/Enums/OnlinePaymentStatus.cs`
- `src/Randevoo.Domain/Enums/PlannerPayoutMethod.cs`
- `src/Randevoo.Domain/Enums/PlannerWithdrawalRequestStatus.cs`
- `src/Randevoo.Domain/Enums/SmsQueueItemStatus.cs`
- `src/Randevoo.Domain/Enums/SupportTicketCategory.cs`
- `src/Randevoo.Domain/Enums/SupportTicketStatus.cs`
- `src/Randevoo.Domain/Enums/SurveyFactor.cs`
- `src/Randevoo.Domain/Enums/TicketOrderPaymentStatus.cs`
- `src/Randevoo.Domain/Enums/TicketOrderStatus.cs`
- `src/Randevoo.Domain/Enums/UserRole.cs`

| Enum | Values | Source |
| --- | --- | --- |
| BalanceTransactionType | AdminAdjustment, TicketPurchase, TicketRefund, EventPlannerIncome, PlatformCommission, EmergencyRemovalRefund, PlannerWithdrawalPayout, EventPlannerIncomeReversal | `src/Randevoo.Domain/Enums/BalanceTransactionType.cs` |
| EducationLevel | NotSpecified, Diploma, Undergraduate, Graduated, Postgraduate, PhD, PostDoc | `src/Randevoo.Domain/Enums/EducationLevel.cs` |
| EventDiscountGenderScope | All, Male, Female | `src/Randevoo.Domain/Enums/EventDiscountGenderScope.cs` |
| EventDiscountType | FixedAmount, Percentage | `src/Randevoo.Domain/Enums/EventDiscountType.cs` |
| EventEducationLevelRestriction | WithoutLimit, DiplomaOrHigher, BachelorOrHigher, MasterOrHigher, ProfessionalDoctorateOrPhD | `src/Randevoo.Domain/Enums/EventEducationLevelRestriction.cs` |
| EventLikeStatus | Pending, Matched, Rejected | `src/Randevoo.Domain/Enums/EventLikeStatus.cs` |
| EventOperationalStatus | Draft, Selling, Closed, Cancelled | `src/Randevoo.Domain/Enums/EventOperationalStatus.cs` |
| EventParticipantSmsRequestStatus | Pending, Approved, Rejected | `src/Randevoo.Domain/Enums/EventParticipantSmsRequestStatus.cs` |
| EventPaymentCollectionMethod | PlatformGateway, PlatformManualTransfer, OrganizerManualTransfer | `src/Randevoo.Domain/Enums/EventPaymentCollectionMethod.cs` |
| EventReviewStatus | NotSubmitted, PendingReview, Approved, Rejected | `src/Randevoo.Domain/Enums/EventReviewStatus.cs` |
| Gender | Unknown, Male, Female | `src/Randevoo.Domain/Enums/Gender.cs` |
| ManualPaymentDestinationType | Platform, Organizer | `src/Randevoo.Domain/Enums/ManualPaymentDestinationType.cs` |
| ManualPaymentReceiptStatus | Submitted, Approved, Rejected | `src/Randevoo.Domain/Enums/ManualPaymentReceiptStatus.cs` |
| ModerationReportReason | Harassment, UnsafeBehavior, FakeProfile, Spam, InappropriateContent, Other | `src/Randevoo.Domain/Enums/ModerationReportReason.cs` |
| ModerationReportStatus | Pending, Reviewed, Dismissed, ActionTaken | `src/Randevoo.Domain/Enums/ModerationReportStatus.cs` |
| OnlinePaymentStatus | Pending, Succeeded, Failed, Refunded | `src/Randevoo.Domain/Enums/OnlinePaymentStatus.cs` |
| PlannerPayoutMethod | IranianBankCard, BankTransfer, IbanSwift, PayPal, Wise, StripeConnect, Other | `src/Randevoo.Domain/Enums/PlannerPayoutMethod.cs` |
| PlannerWithdrawalRequestStatus | Pending, Confirmed, Rejected | `src/Randevoo.Domain/Enums/PlannerWithdrawalRequestStatus.cs` |
| SmsQueueItemStatus | Pending, Processing, Sent, Failed | `src/Randevoo.Domain/Enums/SmsQueueItemStatus.cs` |
| SupportTicketCategory | FinancialProblem, EventProblem, GeneralQuestion | `src/Randevoo.Domain/Enums/SupportTicketCategory.cs` |
| SupportTicketStatus | Open, InProgress, WaitingForUser, Closed, Reopened | `src/Randevoo.Domain/Enums/SupportTicketStatus.cs` |
| SurveyFactor | OverallExperience, EventOrganization, VenueAndLocation, ParticipantQuality, SafetyAndComfort | `src/Randevoo.Domain/Enums/SurveyFactor.cs` |
| TicketOrderPaymentStatus | Pending, Paid, Rejected, Refunded | `src/Randevoo.Domain/Enums/TicketOrderPaymentStatus.cs` |
| TicketOrderStatus | PendingPayment, Confirmed, Cancelled, Refunded | `src/Randevoo.Domain/Enums/TicketOrderStatus.cs` |
| UserRole | EndUser, EventPlanner, Admin, PlatformSupportTeam | `src/Randevoo.Domain/Enums/UserRole.cs` |

## Practical notes for developers
Use the linked source files as the authority. Validate behavior with tests before changing production code.

## Practical notes for future AI coding agents
Do not invent missing behavior. If a feature is represented only by docs or UI labels, mark it as partial until a handler, endpoint, entity, or test confirms it.
