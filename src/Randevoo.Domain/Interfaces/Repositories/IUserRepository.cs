using System;
using System.Collections.Generic;
using System.Text;

namespace Randevoo.Domain.Interfaces.Repositories
{
    using global::Randevoo.Domain.Entities; 
                                 

    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<User?> GetByMobileNumberAsync(string mobileNumber, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<User>> ListActiveSupportUsersAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task AddAsync(User user, CancellationToken cancellationToken = default);
        Task UpdateAsync(User user, CancellationToken cancellationToken = default);
    }                                 
}
