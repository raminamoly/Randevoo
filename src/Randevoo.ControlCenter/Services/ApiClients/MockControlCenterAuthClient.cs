using Randevoo.ControlCenter.Models.Auth;

namespace Randevoo.ControlCenter.Services.ApiClients;

public sealed class MockControlCenterAuthClient : IControlCenterAuthClient
{
    public Task<MockUser> VerifySmsCodeAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var displayName = request.Role == ControlCenterRole.Admin ? "Admin Operator" : "Event Planner";
        var user = new MockUser(Guid.NewGuid(), displayName, request.MobileNumber, request.Role);
        return Task.FromResult(user);
    }
}
