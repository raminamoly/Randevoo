namespace Randevoo.Domain.Enums;

public enum BalanceTransactionType
{
    AdminAdjustment = 0,
    TicketPurchase = 1,
    TicketRefund = 2,
    EventPlannerIncome = 3,
    PlatformCommission = 4,
    EmergencyRemovalRefund = 5,
    PlannerWithdrawalPayout = 6,
    EventPlannerIncomeReversal = 7,
    EventSettlementCredit = 8,
    EventSettlementReversal = 9,
    PlatformCommissionRecognized = 10,
    ManualReceiptWalletCredit = 11,
    OrganizerManualReceiptLiability = 12
}
