using Microsoft.Extensions.Configuration;
using Randevoo.Application.Interfaces.Auth;

namespace Randevoo.Infrastructure.Services;

public class AuthTokenPolicy : IAuthTokenPolicy
{
    private readonly IConfiguration _configuration;

    public AuthTokenPolicy(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public int RefreshTokenExpiresDays =>
        int.TryParse(_configuration["Auth:RefreshTokenExpiresDays"], out var days) ? days : 30;
}
