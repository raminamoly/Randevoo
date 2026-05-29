  
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Infrastructure.Data;
using Randevoo.Infrastructure.Repositories;

namespace Randevoo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddRandevooInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<RandevooDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

       
        return services;
    }
}
