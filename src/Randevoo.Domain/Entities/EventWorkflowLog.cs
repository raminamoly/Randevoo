using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class EventWorkflowLog : BaseEntity
{
    public long DatingEventId { get; private set; }
    public DatingEvent DatingEvent { get; private set; } = null!;
    public EventWorkflowActionType ActionType { get; private set; }
    public EventApprovalStatus? FromApprovalStatus { get; private set; }
    public EventApprovalStatus? ToApprovalStatus { get; private set; }
    public EventSaleStatus? FromSaleStatus { get; private set; }
    public EventSaleStatus? ToSaleStatus { get; private set; }
    public EventLifecycleStatus? FromLifecycleStatus { get; private set; }
    public EventLifecycleStatus? ToLifecycleStatus { get; private set; }
    public long? ActorUserId { get; private set; }
    public User? ActorUser { get; private set; }
    public string? Reason { get; private set; }
    public string? BeforeJson { get; private set; }
    public string? AfterJson { get; private set; }
    public string? MetadataJson { get; private set; }

    private EventWorkflowLog() { }

    public EventWorkflowLog(
        DatingEvent datingEvent,
        EventWorkflowActionType actionType,
        long? actorUserId,
        EventApprovalStatus? fromApprovalStatus = null,
        EventApprovalStatus? toApprovalStatus = null,
        EventSaleStatus? fromSaleStatus = null,
        EventSaleStatus? toSaleStatus = null,
        EventLifecycleStatus? fromLifecycleStatus = null,
        EventLifecycleStatus? toLifecycleStatus = null,
        string? reason = null,
        string? beforeJson = null,
        string? afterJson = null,
        string? metadataJson = null)
    {
        DatingEvent = GuardAgainst.Object.Null(datingEvent, nameof(datingEvent));
        DatingEventId = datingEvent.Id;
        ActionType = actionType;
        ActorUserId = actorUserId;
        FromApprovalStatus = fromApprovalStatus;
        ToApprovalStatus = toApprovalStatus;
        FromSaleStatus = fromSaleStatus;
        ToSaleStatus = toSaleStatus;
        FromLifecycleStatus = fromLifecycleStatus;
        ToLifecycleStatus = toLifecycleStatus;
        Reason = Normalize(reason, 1000);
        BeforeJson = Normalize(beforeJson, 8000);
        AfterJson = Normalize(afterJson, 8000);
        MetadataJson = Normalize(metadataJson, 4000);
    }

    private static string? Normalize(string? value, int maxLength)
        => string.IsNullOrWhiteSpace(value) ? null : GuardAgainst.String.MaxLength(value.Trim(), nameof(value), maxLength);
}
