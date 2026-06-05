using Randevoo.Domain.Common;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;

namespace Randevoo.Domain.Entities;

public class EventTicket : BaseEntity
{
    public long DatingEventId { get; private set; }
    public DatingEvent DatingEvent { get; private set; } = null!;
    public long UserId { get; private set; }
    public User User { get; private set; } = null!;
    public Gender Gender { get; private set; }
    public decimal OriginalPrice { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public long? EventDiscountCodeId { get; private set; }
    public EventDiscountCode? EventDiscountCode { get; private set; }
    public string? DiscountCode { get; private set; }
    public decimal Price { get; private set; }
    public bool IsRefunded { get; private set; }
    public bool IsRemoved { get; private set; }
    public string? RemovalReason { get; private set; }
    public long? RemovedByUserId { get; private set; }
    public DateTime? RemovedAt { get; private set; }
    public bool IsValidForEventAccess => !IsRefunded && !IsRemoved;

    private EventTicket() { }

    internal EventTicket(
        DatingEvent datingEvent,
        User user,
        Gender gender,
        decimal originalPrice,
        decimal finalPrice,
        EventDiscountCode? discountCode = null)
    {
        DatingEvent = datingEvent;
        User = user;
        UserId = user.Id;
        Gender = gender;
        OriginalPrice = GuardAgainst.Number.OutOfRange(originalPrice, nameof(originalPrice), 0.01m, 1_000_000m);
        Price = GuardAgainst.Number.OutOfRange(finalPrice, nameof(finalPrice), 0.01m, OriginalPrice);
        DiscountAmount = OriginalPrice - Price;
        EventDiscountCode = discountCode;
        EventDiscountCodeId = discountCode?.Id;
        DiscountCode = discountCode?.Code;
        IsRefunded = false;
        IsRemoved = false;
    }

    public void MarkRefunded()
    {
        IsRefunded = true;
        UpdateTimestamp();
    }

    public void RemoveWithRefund(long removedByUserId, string reason)
    {
        if (IsRemoved)
            throw new BusinessRuleViolationException("Participant already removed", "This ticket was already removed from the event");

        IsRemoved = true;
        RemovalReason = GuardAgainst.String.InvalidLength(reason, nameof(reason), 5, 500);
        RemovedByUserId = removedByUserId;
        RemovedAt = DateTime.UtcNow;
        MarkRefunded();
    }
}
