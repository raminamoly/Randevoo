using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;

namespace Randevoo.Application.EndUsers.Events;

public sealed class UserFacingEventStatusResolver : IUserFacingEventStatusResolver
{
    private readonly UserFacingEventTimingOptions _options;

    public UserFacingEventStatusResolver()
        : this(UserFacingEventTimingOptions.Default)
    {
    }

    public UserFacingEventStatusResolver(UserFacingEventTimingOptions options)
    {
        _options = options;
    }

    public UserFacingEventStatusKind Resolve(DatingEvent datingEvent, DateTime nowUtc)
    {
        var normalizedNow = nowUtc.Kind == DateTimeKind.Utc
            ? nowUtc
            : DateTime.SpecifyKind(nowUtc, DateTimeKind.Utc);

        if (datingEvent.IsCancelled || datingEvent.LifecycleStatus == EventLifecycleStatus.Cancelled)
            return UserFacingEventStatusKind.Cancelled;

        if (normalizedNow > datingEvent.DateTimeEnd.Add(_options.LikeWindowAfterEnd))
            return UserFacingEventStatusKind.LikeWindowClosed;

        if (normalizedNow >= datingEvent.DateTimeEnd)
            return UserFacingEventStatusKind.LikeWindowOpen;

        if (normalizedNow >= datingEvent.DateTimeStart)
            return UserFacingEventStatusKind.InProgress;

        if (normalizedNow >= datingEvent.DateTimeStart.Subtract(_options.ParticipantProfilesOpenBeforeStart))
            return UserFacingEventStatusKind.ParticipantProfilesOpen;

        if (datingEvent.IsOpenForSell && datingEvent.SaleStatus == EventSaleStatus.Open)
            return IsCapacityFull(datingEvent)
                ? UserFacingEventStatusKind.CapacityFull
                : UserFacingEventStatusKind.SaleOpen;

        return UserFacingEventStatusKind.SaleClosed;
    }

    private static bool IsCapacityFull(DatingEvent datingEvent)
    {
        var activeTickets = datingEvent.Tickets.Where(ticket => ticket.IsValidForEventAccess).ToList();
        var maleTickets = activeTickets.Count(ticket => ticket.Gender == Gender.Male);
        var femaleTickets = activeTickets.Count(ticket => ticket.Gender == Gender.Female);

        return maleTickets >= datingEvent.MaleCapacity && femaleTickets >= datingEvent.FemaleCapacity;
    }
}
