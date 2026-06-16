using System.ComponentModel.DataAnnotations;

namespace Randevoo.AdminPanel.Models.Events;

public sealed class InterestTagMappingListItem
{
    public long Id { get; init; }
    public long InterestId { get; init; }
    public string InterestName { get; init; } = string.Empty;
    public string? InterestCategory { get; init; }
    public int InterestUsageCount { get; init; }
    public long TagId { get; init; }
    public string TagName { get; init; } = string.Empty;
    public int RelevanceWeight { get; init; }
    public bool IsActive { get; init; }
}

public sealed class InterestOption
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Category { get; init; }
    public int UsageCount { get; init; }
}

public sealed class InterestTagMappingInput
{
    [Required(ErrorMessage = "علاقه را انتخاب کنید.")]
    public long? InterestId { get; set; }

    [Required(ErrorMessage = "تگ را انتخاب کنید.")]
    public long? TagId { get; set; }

    [Range(1, 100, ErrorMessage = "وزن باید بین 1 تا 100 باشد.")]
    public int RelevanceWeight { get; set; } = 80;

    public bool IsActive { get; set; } = true;
}
