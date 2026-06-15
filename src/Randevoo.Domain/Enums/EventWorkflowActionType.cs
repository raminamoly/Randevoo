namespace Randevoo.Domain.Enums;

public enum EventWorkflowActionType
{
    DraftSaved = 0,
    SubmittedForReview = 1,
    Approved = 2,
    ReturnedToDraft = 3,
    SaleOpened = 4,
    SaleClosed = 5,
    ChangeRequested = 6,
    ChangeApproved = 7,
    ChangeRejected = 8,
    CancellationRequested = 9,
    Cancelled = 10,
    Completed = 11,
    SettlementRequested = 12,
    SettlementApproved = 13,
    SettlementRejected = 14,
    OrganizerCredited = 15,
    WithdrawalRequested = 16
}
