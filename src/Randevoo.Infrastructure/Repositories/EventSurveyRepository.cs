using Microsoft.EntityFrameworkCore;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Infrastructure.Data;

namespace Randevoo.Infrastructure.Repositories;

public class EventSurveyRepository : IEventSurveyRepository
{
    private readonly RandevooDbContext _db;

    public EventSurveyRepository(RandevooDbContext db)
    {
        _db = db;
    }

    public Task<EventSurveyResponse?> GetByEventAndUserAsync(long eventId, long userId, CancellationToken cancellationToken = default)
    {
        return _db.EventSurveyResponses
            .Include(response => response.DatingEvent)
            .Include(response => response.Ratings)
            .FirstOrDefaultAsync(response => response.DatingEventId == eventId && response.UserId == userId, cancellationToken);
    }

    public async Task<(decimal AverageRating, int SurveyCount)> GetPlannerQualityAsync(long plannerUserId, CancellationToken cancellationToken = default)
    {
        var surveyIds = await _db.EventSurveyResponses
            .Where(response => response.DatingEvent.EventPlannerUserId == plannerUserId)
            .Select(response => response.Id)
            .ToListAsync(cancellationToken);

        if (surveyIds.Count == 0)
            return (0, 0);

        var average = await _db.EventSurveyRatings
            .Where(rating => surveyIds.Contains(rating.EventSurveyResponseId))
            .AverageAsync(rating => (decimal)rating.Score, cancellationToken);

        return (Math.Round(average, 2), surveyIds.Count);
    }

    public async Task AddAsync(EventSurveyResponse response, CancellationToken cancellationToken = default)
    {
        _db.EventSurveyResponses.Add(response);
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(EventSurveyResponse response, CancellationToken cancellationToken = default)
    {
        _db.EventSurveyResponses.Update(response);
        await Task.CompletedTask;
    }
}
