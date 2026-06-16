using Randevoo.Domain.Entities;

namespace Randevoo.Domain.Interfaces.Repositories;

public interface IInterestRepository
{
    Task<IReadOnlyList<Interest>> GetByNamesAsync(IReadOnlyCollection<string> names, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<Interest> interests, CancellationToken cancellationToken = default);
}
