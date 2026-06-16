using Randevoo.Domain.Common;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class SpecialOperationLog : BaseEntity
{
    public string OperationType { get; private set; } = null!;
    public string Status { get; private set; } = null!;
    public long PerformedByUserId { get; private set; }
    public long? TargetUserId { get; private set; }
    public long? RelatedTicketId { get; private set; }
    public long? RelatedOrderId { get; private set; }
    public long? RelatedEventId { get; private set; }
    public long? RelatedWalletTransactionId { get; private set; }
    public decimal? Amount { get; private set; }
    public string? CurrencyCode { get; private set; }
    public string Reason { get; private set; } = null!;
    public string? SupportTicketNumber { get; private set; }
    public string IdempotencyKey { get; private set; } = null!;
    public string? RequestPayloadJson { get; private set; }
    public string? PreviewPayloadJson { get; private set; }
    public string? ResultPayloadJson { get; private set; }
    public string? FailureMessage { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public string? CorrelationId { get; private set; }

    private SpecialOperationLog() { }

    public SpecialOperationLog(
        string operationType,
        long performedByUserId,
        long? targetUserId,
        string reason,
        string idempotencyKey,
        string? requestPayloadJson,
        string? previewPayloadJson,
        string? supportTicketNumber = null,
        long? relatedTicketId = null,
        long? relatedOrderId = null,
        long? relatedEventId = null,
        decimal? amount = null,
        string? currencyCode = null,
        string? correlationId = null)
    {
        OperationType = NormalizeRequired(operationType, nameof(operationType), 80);
        Status = "Pending";
        PerformedByUserId = performedByUserId;
        TargetUserId = targetUserId;
        Reason = NormalizeRequired(reason, nameof(reason), 5, 1000);
        IdempotencyKey = NormalizeRequired(idempotencyKey, nameof(idempotencyKey), 20, 120);
        RequestPayloadJson = NormalizeOptional(requestPayloadJson, nameof(requestPayloadJson), 8000);
        PreviewPayloadJson = NormalizeOptional(previewPayloadJson, nameof(previewPayloadJson), 8000);
        SupportTicketNumber = NormalizeOptional(supportTicketNumber, nameof(supportTicketNumber), 80);
        RelatedTicketId = relatedTicketId;
        RelatedOrderId = relatedOrderId;
        RelatedEventId = relatedEventId;
        Amount = amount;
        CurrencyCode = NormalizeOptional(currencyCode, nameof(currencyCode), 12);
        CorrelationId = NormalizeOptional(correlationId, nameof(correlationId), 100);
    }

    public void MarkSucceeded(string? resultPayloadJson, long? walletTransactionId = null)
    {
        Status = "Succeeded";
        ResultPayloadJson = NormalizeOptional(resultPayloadJson, nameof(resultPayloadJson), 8000);
        RelatedWalletTransactionId = walletTransactionId;
        FailureMessage = null;
        CompletedAtUtc = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void AttachResultReferences(
        long? targetUserId,
        long? relatedTicketId,
        long? relatedOrderId,
        long? relatedEventId,
        decimal? amount,
        string? currencyCode)
    {
        TargetUserId = targetUserId;
        RelatedTicketId = relatedTicketId;
        RelatedOrderId = relatedOrderId;
        RelatedEventId = relatedEventId;
        Amount = amount;
        CurrencyCode = NormalizeOptional(currencyCode, nameof(currencyCode), 12);
        UpdateTimestamp();
    }

    public void MarkFailed(string failureMessage, string? resultPayloadJson = null)
    {
        Status = "Failed";
        FailureMessage = NormalizeRequired(failureMessage, nameof(failureMessage), 1, 1000);
        ResultPayloadJson = NormalizeOptional(resultPayloadJson, nameof(resultPayloadJson), 8000);
        CompletedAtUtc = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public override void SoftDelete()
    {
        throw new BusinessRuleViolationException("Special operation logs are append-only", "Special operation log records cannot be deleted");
    }

    private static string NormalizeRequired(string value, string parameterName, int maxLength)
        => GuardAgainst.String.MaxLength(GuardAgainst.String.NullOrWhiteSpace(value, parameterName).Trim(), parameterName, maxLength);

    private static string NormalizeRequired(string value, string parameterName, int minLength, int maxLength)
        => GuardAgainst.String.InvalidLength(GuardAgainst.String.NullOrWhiteSpace(value, parameterName).Trim(), parameterName, minLength, maxLength);

    private static string? NormalizeOptional(string? value, string parameterName, int maxLength)
        => string.IsNullOrWhiteSpace(value) ? null : GuardAgainst.String.MaxLength(value.Trim(), parameterName, maxLength);
}
