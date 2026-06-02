namespace Randevoo.ControlCenter.Models.Balances;

public sealed record BalanceSummary(Guid OwnerId, string OwnerName, decimal Available, decimal Pending);
