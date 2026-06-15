using Randevoo.Domain.Enums;

namespace Randevoo.Domain.Constants;

public static class SupportTicketLookupIds
{
    public const long TypeFinancialProblem = 1;
    public const long TypeEventProblem = 2;
    public const long TypeGeneralQuestion = 3;
    public const long TypeTicketProblem = 4;
    public const long TypePrePurchaseQuestion = 5;

    public const long StatusOpen = 1;
    public const long StatusInProgress = 2;
    public const long StatusWaitingForUser = 3;
    public const long StatusClosed = 4;
    public const long StatusReopened = 5;

    public const long RecipientPlatformSupport = 1;
    public const long RecipientEventPlanner = 2;

    public static long FromCategory(SupportTicketCategory category) => category switch
    {
        SupportTicketCategory.FinancialProblem => TypeFinancialProblem,
        SupportTicketCategory.EventProblem => TypeEventProblem,
        SupportTicketCategory.GeneralQuestion => TypeGeneralQuestion,
        _ => TypeGeneralQuestion
    };

    public static SupportTicketCategory ToCategory(long ticketTypeId) => ticketTypeId switch
    {
        TypeFinancialProblem => SupportTicketCategory.FinancialProblem,
        TypeEventProblem => SupportTicketCategory.EventProblem,
        TypeGeneralQuestion => SupportTicketCategory.GeneralQuestion,
        TypeTicketProblem => SupportTicketCategory.FinancialProblem,
        TypePrePurchaseQuestion => SupportTicketCategory.GeneralQuestion,
        _ => SupportTicketCategory.GeneralQuestion
    };

    public static long FromStatus(SupportTicketStatus status) => status switch
    {
        SupportTicketStatus.Open => StatusOpen,
        SupportTicketStatus.InProgress => StatusInProgress,
        SupportTicketStatus.WaitingForUser => StatusWaitingForUser,
        SupportTicketStatus.Closed => StatusClosed,
        SupportTicketStatus.Reopened => StatusReopened,
        _ => StatusOpen
    };

    public static SupportTicketStatus ToStatus(long statusId) => statusId switch
    {
        StatusOpen => SupportTicketStatus.Open,
        StatusInProgress => SupportTicketStatus.InProgress,
        StatusWaitingForUser => SupportTicketStatus.WaitingForUser,
        StatusClosed => SupportTicketStatus.Closed,
        StatusReopened => SupportTicketStatus.Reopened,
        _ => SupportTicketStatus.Open
    };
}
