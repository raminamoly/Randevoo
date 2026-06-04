using Randevoo.Domain.Common;

namespace Randevoo.Domain.Entities;

public class EventTag : BaseEntity
{
    public long DatingEventId { get; private set; }
    public DatingEvent DatingEvent { get; private set; } = null!;
    public long TagId { get; private set; }
    public Tag Tag { get; private set; } = null!;

    private EventTag() { }

    internal EventTag(DatingEvent datingEvent, Tag tag)
    {
        DatingEvent = GuardAgainst.Object.Null(datingEvent, nameof(datingEvent));
        Tag = GuardAgainst.Object.Null(tag, nameof(tag));
        DatingEventId = datingEvent.Id;
        TagId = tag.Id;
    }
}
