namespace Randevoo.AdminPanel.Models.Events;

public sealed class EventCancellationPreview
{
    public long EventId { get; init; }

    public string EventTitle { get; init; } = string.Empty;

    public string CurrentOperationalStatus { get; init; } = string.Empty;

    public bool CanCancel { get; init; }

    public string Summary { get; init; } = string.Empty;

    public bool RequiresManualRefundFollowUp { get; init; }

    public bool CreatesBuyerRefundCredits { get; init; }

    public int ActiveTicketCount { get; init; }

    public int PaidOrderCount { get; init; }

    public int BuyerCount { get; init; }

    public int ParticipantCount { get; init; }

    public int PendingManualReceiptCount { get; init; }

    public int ApprovedManualReceiptCount { get; init; }

    public int PendingSettlementRequestCount { get; init; }

    public int ApprovedSettlementRequestCount { get; init; }

    public decimal PlatformRefundAmountIrr { get; init; }

    public decimal OrganizerManualRefundAmountIrr { get; init; }

    public string SuggestedPublicMessage { get; init; } = string.Empty;

    public IReadOnlyList<EventCancellationPreviewMetric> Metrics { get; init; } = Array.Empty<EventCancellationPreviewMetric>();

    public IReadOnlyList<string> Consequences { get; init; } = Array.Empty<string>();

    public IReadOnlyList<string> Warnings { get; init; } = Array.Empty<string>();

    public IReadOnlyList<string> BlockingReasons { get; init; } = Array.Empty<string>();
}

public sealed class EventCancellationPreviewMetric
{
    public string Label { get; init; } = string.Empty;

    public string Value { get; init; } = string.Empty;

    public string? Hint { get; init; }
}

public sealed class EventCancellationResult
{
    public DatingEvent Event { get; init; } = new();

    public EventCancellationPreview Preview { get; init; } = new();
}
