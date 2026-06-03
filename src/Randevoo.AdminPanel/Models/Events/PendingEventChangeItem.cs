namespace Randevoo.AdminPanel.Models.Events;

public sealed class PendingEventChangeItem
{
    public string Label { get; set; } = string.Empty;

    public string? BeforeText { get; set; }

    public string? AfterText { get; set; }

    public string? BeforeHtml { get; set; }

    public string? AfterHtml { get; set; }

    public string? BeforeImageUrl { get; set; }

    public string? AfterImageUrl { get; set; }

    public IReadOnlyList<string> BeforeTags { get; set; } = Array.Empty<string>();

    public IReadOnlyList<string> AfterTags { get; set; } = Array.Empty<string>();

    public bool IsHtml { get; set; }

    public bool IsImage { get; set; }

    public bool IsTagList { get; set; }
}
