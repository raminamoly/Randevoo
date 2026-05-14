using Randevoo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randevoo.Domain.Services
{
    public interface IMatchingService
    {
        Task<IReadOnlyList<UserProfile>> FindMatchesAsync(
            UserProfile user,
            int maxDistanceKm,
            int minAge,
            int maxAge);
    }

  
   
}
