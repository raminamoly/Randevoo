namespace Randevoo.AdminPanel.Services.Infrastructure;

public sealed class AdminActivityTrackRequest
{
    public string Type { get; set; } = "click";

    public string? Action { get; set; }

    public string? Module { get; set; }

    public string? Description { get; set; }

    public string? Path { get; set; }

    public double? DurationSeconds { get; set; }

    public Dictionary<string, string>? Metadata { get; set; }
}
