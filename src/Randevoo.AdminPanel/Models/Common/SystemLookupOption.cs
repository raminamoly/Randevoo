namespace Randevoo.AdminPanel.Models.Common;

public sealed class SystemLookupOption
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayNameFa { get; set; } = string.Empty;
    public string Value => Name;
}
