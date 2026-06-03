namespace Randevoo.AdminPanel.Models.Events;

public sealed class EventImageCarouselModel
{
    public string CarouselId { get; set; } = string.Empty;

    public IReadOnlyList<string> Images { get; set; } = Array.Empty<string>();
}
