using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.DiscountCodes;

namespace Randevoo.AdminPanel.Services.ApiClients;

public interface IEventDiscountCodesApiClient
{
    Task<IReadOnlyList<EventDiscountCodeAdminItem>> GetDiscountCodesAsync(CancellationToken cancellationToken = default);

    Task<EventDiscountCodeAdminItem?> GetDiscountCodeAsync(long id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EventDiscountCodeUsageItem>> GetDiscountCodeUsageAsync(long id, CancellationToken cancellationToken = default);

    Task<EventDiscountCodeAdminItem> UpsertDiscountCodeAsync(EventDiscountCodeEditorInput input, MockUser actor, long? existingDiscountCodeId = null, CancellationToken cancellationToken = default);

    Task SetDiscountCodeActiveAsync(long id, MockUser actor, bool isActive, CancellationToken cancellationToken = default);
}
