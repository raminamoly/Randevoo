using MediatR;
using Microsoft.Extensions.DependencyInjection;
 

namespace Randevoo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddRandevooApplication(this IServiceCollection services)
    {
       // services.AddScoped<IUserProfileService, UserProfileService>();

        // Register MediatR handlers from this assembly (Application)
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection)));

        return services;
    }
}