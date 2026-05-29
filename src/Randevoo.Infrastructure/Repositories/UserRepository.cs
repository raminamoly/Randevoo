 
using Microsoft.EntityFrameworkCore;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Infrastructure.Data;

namespace Randevoo.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly RandevooDbContext _db;
    public UserRepository(RandevooDbContext db) => _db = db;

    public async Task<User?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        await _db.Users.FindAsync(new object[] { id }, cancellationToken);

    public async Task<User?> GetByMobileNumberAsync(string mobileNumber, CancellationToken cancellationToken = default)
    {
        return await _db.Users.FirstOrDefaultAsync(u => u.MobileNumber == mobileNumber, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _db.Users.AnyAsync(u => u.Email == email || u.PendingEmail == email, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _db.Users.Add(user);
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _db.Users.Update(user);
        await Task.CompletedTask;
    }
}
