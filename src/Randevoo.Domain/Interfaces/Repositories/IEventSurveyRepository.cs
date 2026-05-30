using Randevoo.Domain.Entities;

namespace Randevoo.Domain.Interfaces.Repositories;

public interface IEventSurveyRepository
{
    Task<EventSurveyResponse?> GetByEventAndUserAsync(long eventId, long userId, CancellationToken cancellationToken = default);
    Task<(decimal AverageRating, int SurveyCount)> GetPlannerQualityAsync(long plannerUserId, CancellationToken cancellationToken = default);
    Task AddAsync(EventSurveyResponse response, CancellationToken cancellationToken = default);
    Task UpdateAsync(EventSurveyResponse response, CancellationToken cancellationToken = default);
}
