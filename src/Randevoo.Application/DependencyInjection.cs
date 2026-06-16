using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Randevoo.Application.EndUsers.Events;
using Randevoo.Application.EndUsers.Profile;

namespace Randevoo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddRandevooApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection)));
        services.AddScoped<IEndUserProfileStatusService, EndUserProfileStatusService>();
        services.AddScoped<IEndUserEventEligibilityService, EndUserEventEligibilityService>();
        services.AddScoped<IUserFacingEventStatusResolver, UserFacingEventStatusResolver>();

        return services;
    }
}
