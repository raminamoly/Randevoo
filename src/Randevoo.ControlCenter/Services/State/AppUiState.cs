namespace Randevoo.ControlCenter.Services.State;

public sealed class AppUiState
{
    public bool IsDrawerOpen { get; private set; } = true;

    public void ToggleDrawer()
    {
        IsDrawerOpen = !IsDrawerOpen;
    }
}
