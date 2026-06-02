using Randevoo.ControlCenter.Models.Auth;

namespace Randevoo.ControlCenter.Services.Auth;

public sealed class MockAuthState
{
    private static MockUser? s_currentMockUser;

    public event Action? Changed;

    public MockUser? CurrentUser { get; private set; } = s_currentMockUser;

    public bool IsAuthenticated => CurrentUser is not null;

    public bool IsInRole(ControlCenterRole role) => CurrentUser?.Role == role;

    public void SignIn(MockUser user)
    {
        s_currentMockUser = user;
        CurrentUser = user;
        Changed?.Invoke();
    }

    public void SignOut()
    {
        s_currentMockUser = null;
        CurrentUser = null;
        Changed?.Invoke();
    }
}
