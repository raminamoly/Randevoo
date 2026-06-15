namespace Randevoo.AdminPanel.Models.Events;

public sealed class EventStatusTransitionOption
{
    public EventStatusTransitionAction Action { get; init; }

    public string Title { get; init; } = string.Empty;

    public string TargetLabel { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public string ConfirmationText { get; init; } = "تایید می‌کنم این تغییر وضعیت ثبت شود.";

    public string IconCssClass { get; init; } = "bi-arrow-repeat";

    public string ToneCssClass { get; init; } = "status-transition-neutral";

    public bool RequiresNote { get; init; }

    public string NoteLabel { get; init; } = "توضیحات";

    public string NotePlaceholder { get; init; } = "توضیح کوتاهی درباره این تغییر بنویسید.";
}
