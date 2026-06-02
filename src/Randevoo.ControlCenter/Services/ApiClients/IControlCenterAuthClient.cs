using Randevoo.ControlCenter.Models.Auth;

namespace Randevoo.ControlCenter.Services.ApiClients;

public interface IControlCenterAuthClient
{
    Task<MockUser> VerifySmsCodeAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
