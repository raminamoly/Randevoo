  
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Randevoo.Application.Interfaces.Auth;
using Randevoo.Application.Interfaces.Notifications;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Infrastructure.Data;
using Randevoo.Infrastructure.Services;
using Randevoo.Infrastructure.Repositories;

namespace Randevoo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddRandevooInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<RandevooDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IEventPlannerProfileRepository, EventPlannerProfileRepository>();
        services.AddScoped<IBalanceAccountRepository, BalanceAccountRepository>();
        services.AddScoped<IDatingEventRepository, DatingEventRepository>();
        services.AddScoped<IEventTicketRepository, EventTicketRepository>();
        services.AddScoped<IEventConversationRepository, EventConversationRepository>();
        services.AddScoped<IEventSurveyRepository, EventSurveyRepository>();
        services.AddScoped<IEventTypeRepository, EventTypeRepository>();
        services.AddScoped<IModerationReportRepository, ModerationReportRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<ICodeGenerator, SecureCodeGenerator>();
        services.AddSingleton<ICodeHasher, Sha256CodeHasher>();
        services.AddSingleton<IAuthTokenPolicy, AuthTokenPolicy>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<ISmsSender, ConsoleSmsSender>();
        services.AddSingleton<IEmailSender, ConsoleEmailSender>();

       
        return services;
    }
}
