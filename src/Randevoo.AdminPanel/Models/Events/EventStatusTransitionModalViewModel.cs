namespace Randevoo.AdminPanel.Models.Events;

public sealed class EventStatusTransitionModalViewModel
{
    public DatingEvent Event { get; init; } = new();

    public IReadOnlyList<EventStatusTransitionOption> Options { get; init; } = Array.Empty<EventStatusTransitionOption>();

    public string? EmptyMessage { get; init; }

    public string HandlerName { get; init; } = "ChangeStatus";

    public string ReturnUrl { get; init; } = string.Empty;

    public string CancellationPreviewUrl { get; init; } = string.Empty;
}
