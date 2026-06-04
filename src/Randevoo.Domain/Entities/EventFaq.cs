using Randevoo.Domain.Common;

namespace Randevoo.Domain.Entities;

public class EventFaq : BaseEntity
{
    public long DatingEventId { get; private set; }
    public DatingEvent DatingEvent { get; private set; } = null!;
    public string Question { get; private set; } = null!;
    public string Answer { get; private set; } = null!;
    public int DisplayOrder { get; private set; }

    private EventFaq() { }

    internal EventFaq(DatingEvent datingEvent, string question, string answer, int displayOrder)
    {
        DatingEvent = GuardAgainst.Object.Null(datingEvent, nameof(datingEvent));
        Question = GuardAgainst.String.InvalidLength(question.Trim(), nameof(question), 3, 250);
        Answer = GuardAgainst.String.InvalidLength(answer.Trim(), nameof(answer), 3, 1200);
        DisplayOrder = GuardAgainst.Number.OutOfRange(displayOrder, nameof(displayOrder), 1, 50);
    }
}
