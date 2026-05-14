 
using Randevoo.Domain.Common;
using Randevoo.Domain.ValueObjects;

namespace Randevoo.Domain.Entities;

public class Interest : BaseEntity
{
    public string Name { get; private set; }
    public string? Category { get; private set; }
    public int UsageCount { get; private set; } // Popularity tracking
    public ICollection<UserProfile> UserProfiles { get; private set; }


    private Interest() : base()
    {
       
        UserProfiles = new List<UserProfile>();
    }
    public Interest(string name, string? category = null)
    {
        UserProfiles = new List<UserProfile>();

        Name = GuardAgainst.String.InvalidLength(
            name,
            nameof(name),
            min: 2,
            max: 50);

        if (category != null)
        {
            Category = GuardAgainst.String.InvalidLength(
                category,
                nameof(category),
                min: 0,
                max: 30);
        }

        UsageCount = 0;
    }

    public void IncrementUsage()
    {
        UsageCount++;
        UpdateTimestamp();
    }

    public void DecrementUsage()
    {
        if (UsageCount > 0)
            UsageCount--;
        UpdateTimestamp();
    }

  

    
}