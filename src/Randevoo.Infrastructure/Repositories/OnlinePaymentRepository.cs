using Randevoo.Domain.Entities;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Infrastructure.Data;

namespace Randevoo.Infrastructure.Repositories;

public sealed class OnlinePaymentRepository : IOnlinePaymentRepository
{
    private readonly RandevooDbContext _db;

    public OnlinePaymentRepository(RandevooDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(OnlinePayment payment, CancellationToken cancellationToken = default)
    {
        _db.OnlinePayments.Add(payment);
        await Task.CompletedTask;
    }
}
