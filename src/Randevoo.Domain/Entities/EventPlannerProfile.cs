using Randevoo.Domain.Common;
using Randevoo.Domain.Events;

namespace Randevoo.Domain.Entities;

public class EventPlannerProfile : BaseEntity, IAggregateRoot
{
    public long UserId { get; private set; }
    public User User { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string? PictureUrl { get; private set; }
    public string Resume { get; private set; } = null!;
    public decimal AverageRating { get; private set; }
    public int TotalSurveyCount { get; private set; }
    public int HostedEventCount { get; private set; }
    public int CancelledEventCount { get; private set; }
    public int CompletedEventCount { get; private set; }

    private EventPlannerProfile() { }

    public EventPlannerProfile(User user, string title, string? pictureUrl, string resume)
    {
        User = GuardAgainst.Object.Null(user, nameof(user));
        User.BecomeEventPlanner();
        Title = GuardAgainst.String.InvalidLength(title, nameof(title), 2, 100);
        PictureUrl = string.IsNullOrWhiteSpace(pictureUrl) ? null : GuardAgainst.String.MaxLength(pictureUrl, nameof(pictureUrl), 500);
        Resume = GuardAgainst.String.InvalidLength(resume, nameof(resume), 10, 4000);
        AverageRating = 0;
        TotalSurveyCount = 0;
        HostedEventCount = 0;
        CancelledEventCount = 0;
        CompletedEventCount = 0;

        AddDomainEvent(new EntityCreatedEvent<EventPlannerProfile>(this));
    }

    public void Update(string title, string? pictureUrl, string resume)
    {
        Title = GuardAgainst.String.InvalidLength(title, nameof(title), 2, 100);
        PictureUrl = string.IsNullOrWhiteSpace(pictureUrl) ? null : GuardAgainst.String.MaxLength(pictureUrl, nameof(pictureUrl), 500);
        Resume = GuardAgainst.String.InvalidLength(resume, nameof(resume), 10, 4000);
        UpdateTimestamp();
        AddDomainEvent(new EntityUpdatedEvent<EventPlannerProfile>(this, nameof(Title), string.Empty, Title));
    }

    public void UpdateMetrics(decimal averageRating, int totalSurveyCount, int hostedEventCount, int cancelledEventCount, int completedEventCount)
    {
        AverageRating = GuardAgainst.Number.OutOfRange(averageRating, nameof(averageRating), 0, 5);
        TotalSurveyCount = GuardAgainst.Number.Negative(totalSurveyCount, nameof(totalSurveyCount));
        HostedEventCount = GuardAgainst.Number.Negative(hostedEventCount, nameof(hostedEventCount));
        CancelledEventCount = GuardAgainst.Number.Negative(cancelledEventCount, nameof(cancelledEventCount));
        CompletedEventCount = GuardAgainst.Number.Negative(completedEventCount, nameof(completedEventCount));
        UpdateTimestamp();
    }
}
