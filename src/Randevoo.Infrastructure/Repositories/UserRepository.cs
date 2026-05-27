 
using Microsoft.EntityFrameworkCore;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Domain.Interfaces;
using Randevoo.Infrastructure.Data;

namespace Randevoo.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly RandevooDbContext _db;
    public UserRepository(RandevooDbContext db) => _db = db;

    public async Task<User?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        await _db.Users.FindAsync(new object[] { id }, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync(cancellationToken);
    }
}