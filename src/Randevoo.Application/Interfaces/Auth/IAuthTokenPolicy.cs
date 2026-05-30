namespace Randevoo.Application.Interfaces.Auth;

public interface IAuthTokenPolicy
{
    int RefreshTokenExpiresDays { get; }
}
