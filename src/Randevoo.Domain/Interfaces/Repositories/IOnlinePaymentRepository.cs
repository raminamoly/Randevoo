using Randevoo.Domain.Entities;

namespace Randevoo.Domain.Interfaces.Repositories;

public interface IOnlinePaymentRepository
{
    Task AddAsync(OnlinePayment payment, CancellationToken cancellationToken = default);
}
