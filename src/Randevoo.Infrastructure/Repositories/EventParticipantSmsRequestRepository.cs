using Microsoft.EntityFrameworkCore;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Infrastructure.Data;

namespace Randevoo.Infrastructure.Repositories;

public class EventParticipantSmsRequestRepository : IEventParticipantSmsRequestRepository
{
    private readonly RandevooDbContext _db;

    public EventParticipantSmsRequestRepository(RandevooDbContext db)
    {
        _db = db;
    }

    public Task<EventParticipantSmsRequest?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return _db.EventParticipantSmsRequests
            .Include(request => request.RequestedByUser)
            .Include(request => request.ReviewedByAdminUser)
            .FirstOrDefaultAsync(request => request.Id == id, cancellationToken);
    }

    public async Task AddAsync(EventParticipantSmsRequest request, CancellationToken cancellationToken = default)
    {
        _db.EventParticipantSmsRequests.Add(request);
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(EventParticipantSmsRequest request, CancellationToken cancellationToken = default)
    {
        _db.EventParticipantSmsRequests.Update(request);
        await Task.CompletedTask;
    }
}
