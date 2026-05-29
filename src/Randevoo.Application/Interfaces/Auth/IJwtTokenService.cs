using Randevoo.Domain.Entities;

namespace Randevoo.Application.Interfaces.Auth;

public interface IJwtTokenService
{
    string CreateToken(User user);
}
