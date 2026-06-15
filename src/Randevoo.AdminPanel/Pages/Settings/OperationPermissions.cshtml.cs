using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Randevoo.AdminPanel.Models.Common;
using Randevoo.Domain.Constants;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Infrastructure.Data;
using System.ComponentModel.DataAnnotations;

namespace Randevoo.AdminPanel.Pages.Settings;

[Authorize(Policy = Randevoo.AdminPanel.Models.Common.Policies.AdminOnly)]
public class OperationPermissionsModel : PageModel
{
    private const string ActionKeySeparator = "::";
    private readonly RandevooDbContext _db;

    public OperationPermissionsModel(RandevooDbContext db)
    {
        _db = db;
    }

    [BindProperty(SupportsGet = true)] public string? Search { get; set; }
    [BindProperty(SupportsGet = true)] public string? GroupKey { get; set; }
    [BindProperty(SupportsGet = true)] public string? Entity { get; set; }
    [BindProperty(SupportsGet = true)] public string? RiskLevel { get; set; }
    [BindProperty(SupportsGet = true)] public string? Surface { get; set; }
    [BindProperty(SupportsGet = true)] public bool IncludeInactive { get; set; }
    [BindProperty(SupportsGet = true)] public string? UserSearch { get; set; }

    [BindProperty] public List<RolePermissionInput> RolePermissions { get; set; } = [];
    [BindProperty] public UserOverrideInput OverrideInput { get; set; } = new();

    [TempData] public string? StatusMessage { get; set; }

    public int TotalActions { get; private set; }
    public int VisibleActions { get; private set; }
    public int CriticalActions { get; private set; }
    public int UnconfiguredCells { get; private set; }
    public bool HasActiveFilters => !string.IsNullOrWhiteSpace(Search) || !string.IsNullOrWhiteSpace(RiskLevel) || !string.IsNullOrWhiteSpace(Surface) || IncludeInactive;

    public IReadOnlyList<SelectListItem> RiskOptions { get; private set; } = [];
    public IReadOnlyList<SelectListItem> SurfaceOptions { get; private set; } = [];
    public IReadOnlyList<SelectListItem> ActionOptions { get; private set; } = [];
    public IReadOnlyList<PermissionGroupNode> Groups { get; private set; } = [];
    public IReadOnlyList<RoleOption> Roles { get; } =
    [
        new(UserRole.Admin, "مدیر", "admin"),
        new(UserRole.EventPlanner, "برگزارکننده", "planner"),
        new(UserRole.PlatformSupportTeam, "پشتیبان", "support")
    ];
    public IReadOnlyList<PermissionMatrixRow> MatrixRows { get; private set; } = [];
    public IReadOnlyList<UserOption> UserOptions { get; private set; } = [];
    public IReadOnlyList<UserOverrideRow> Overrides { get; private set; } = [];

    public async Task OnGetAsync(CancellationToken cancellationToken) => await LoadAsync(cancellationToken);

    public async Task<IActionResult> OnPostSyncCatalogAsync(CancellationToken cancellationToken)
    {
        await OperationPermissionCatalogSync.SyncAsync(_db, cancellationToken);
        StatusMessage = "کاتالوگ دسترسی عملیات همگام‌سازی شد.";
        return RedirectToSelf();
    }

    public async Task<IActionResult> OnPostSaveRolePermissionsAsync(CancellationToken cancellationToken)
    {
        foreach (var input in RolePermissions)
        {
            if (string.IsNullOrWhiteSpace(input.Entity) || string.IsNullOrWhiteSpace(input.Action))
                continue;

            if (!OperationPermissionCatalog.AdminPanelRoles.Contains(input.Role))
                continue;

            var entity = input.Entity.Trim();
            var action = input.Action.Trim();
            var actionExists = await _db.PermissionActions.AnyAsync(item => item.Entity == entity && item.Action == action, cancellationToken);
            if (!actionExists)
                continue;

            var permission = await _db.RoleOperationPermissions.FirstOrDefaultAsync(
                item => item.Role == input.Role && item.Entity == entity && item.Action == action,
                cancellationToken);

            if (permission is null)
                _db.RoleOperationPermissions.Add(new RoleOperationPermission(input.Role, entity, action, input.Allowed));
            else
                permission.SetAllowed(input.Allowed);
        }

        await _db.SaveChangesAsync(cancellationToken);
        StatusMessage = "دسترسی نقش‌ها ذخیره شد.";
        return RedirectToSelf();
    }

    public async Task<IActionResult> OnPostSaveOverrideAsync(CancellationToken cancellationToken)
    {
        if (!TryParseActionKey(OverrideInput.ActionKey, out var entity, out var action))
            ModelState.AddModelError(nameof(OverrideInput.ActionKey), "عملیات انتخاب شده معتبر نیست.");

        if (!ModelState.IsValid)
        {
            await LoadAsync(cancellationToken);
            return Page();
        }

        var actionExists = await _db.PermissionActions.AnyAsync(item => item.Entity == entity && item.Action == action && item.IsActive, cancellationToken);
        if (!actionExists)
        {
            ModelState.AddModelError(nameof(OverrideInput.ActionKey), "عملیات انتخاب شده معتبر نیست.");
            await LoadAsync(cancellationToken);
            return Page();
        }

        var userExists = await _db.Users.AnyAsync(
            item => item.Id == OverrideInput.UserId
                && (item.Role == UserRole.Admin || item.Role == UserRole.EventPlanner || item.Role == UserRole.PlatformSupportTeam),
            cancellationToken);
        if (!userExists)
        {
            ModelState.AddModelError(nameof(OverrideInput.UserId), "کاربر انتخاب شده معتبر نیست.");
            await LoadAsync(cancellationToken);
            return Page();
        }

        var existing = await _db.UserOperationPermissionOverrides.FirstOrDefaultAsync(
            item => item.UserId == OverrideInput.UserId && item.Entity == entity && item.Action == action,
            cancellationToken);

        var expiresAtUtc = OverrideInput.ExpiresAtUtc?.Date;
        if (existing is null)
        {
            _db.UserOperationPermissionOverrides.Add(new UserOperationPermissionOverride(
                OverrideInput.UserId,
                entity,
                action,
                OverrideInput.Allowed,
                OverrideInput.Note,
                expiresAtUtc));
        }
        else
        {
            existing.Update(OverrideInput.Allowed, OverrideInput.Note, expiresAtUtc);
        }

        await _db.SaveChangesAsync(cancellationToken);
        StatusMessage = "دسترسی اختصاصی کاربر ذخیره شد.";
        return RedirectToSelf();
    }

    public async Task<IActionResult> OnPostDeleteOverrideAsync(long id, CancellationToken cancellationToken)
    {
        var item = await _db.UserOperationPermissionOverrides.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (item is not null)
        {
            item.SoftDelete();
            await _db.SaveChangesAsync(cancellationToken);
            StatusMessage = "override حذف شد.";
        }

        return RedirectToSelf();
    }

    private IActionResult RedirectToSelf() => RedirectToPage(new { Search, GroupKey, Entity, RiskLevel, Surface, IncludeInactive, UserSearch });

    private async Task LoadAsync(CancellationToken cancellationToken)
    {
        var catalogActions = await _db.PermissionActions
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(item => !item.IsDeleted)
            .OrderBy(item => item.DisplayOrder)
            .ThenBy(item => item.Entity)
            .ThenBy(item => item.Action)
            .ToListAsync(cancellationToken);

        TotalActions = catalogActions.Count(item => item.IsActive && !item.IsDeprecated);
        CriticalActions = catalogActions.Count(item => item.IsActive && string.Equals(item.RiskLevel, "Critical", StringComparison.OrdinalIgnoreCase));
        Groups = BuildGroups(catalogActions);
        NormalizeCurrentScope();

        RiskOptions = catalogActions
            .Select(item => item.RiskLevel)
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(item => item)
            .Select(item => new SelectListItem(GetRiskTitle(item), item, string.Equals(item, RiskLevel, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        SurfaceOptions = catalogActions
            .Select(item => item.UiSurface)
            .Where(item => !string.IsNullOrWhiteSpace(item))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(item => item)
            .Select(item => new SelectListItem(GetSurfaceTitle(item), item, string.Equals(item, Surface, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        var visibleActions = ApplyActionFilters(catalogActions).ToList();
        VisibleActions = visibleActions.Count;

        var rolePermissions = await _db.RoleOperationPermissions
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Where(item => !item.IsDeleted)
            .ToDictionaryAsync(
                item => $"{item.Role}{ActionKeySeparator}{item.Entity}{ActionKeySeparator}{item.Action}",
                item => item.Allowed,
                StringComparer.OrdinalIgnoreCase,
                cancellationToken);

        var roleInputs = new List<RolePermissionInput>();
        var matrixRows = new List<PermissionMatrixRow>();
        foreach (var action in visibleActions)
        {
            var values = new List<RolePermissionValue>();
            foreach (var role in Roles)
            {
                var key = $"{role.Role}{ActionKeySeparator}{action.Entity}{ActionKeySeparator}{action.Action}";
                var isConfigured = rolePermissions.TryGetValue(key, out var allowed);
                values.Add(new RolePermissionValue(role.Role, role.Title, allowed, isConfigured));
                roleInputs.Add(new RolePermissionInput(role.Role, action.Entity, action.Action, allowed));
            }

            matrixRows.Add(new PermissionMatrixRow(PermissionActionRow.From(action), values));
        }

        RolePermissions = roleInputs;
        MatrixRows = matrixRows;
        UnconfiguredCells = MatrixRows.Sum(row => row.RoleValues.Count(value => !value.IsConfigured));

        ActionOptions = visibleActions
            .Select(item => new SelectListItem($"{item.EntityLabel} / {item.Label}", BuildActionKey(item.Entity, item.Action)))
            .ToList();

        UserOptions = await LoadUserOptionsAsync(cancellationToken);
        Overrides = await LoadOverridesAsync(catalogActions, cancellationToken);
    }

    private IEnumerable<PermissionAction> ApplyActionFilters(IEnumerable<PermissionAction> actions)
    {
        var filtered = actions;
        if (!IncludeInactive)
            filtered = filtered.Where(item => item.IsActive && !item.IsDeprecated);
        if (!string.IsNullOrWhiteSpace(GroupKey))
            filtered = filtered.Where(item => string.Equals(item.GroupKey, GroupKey, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(Entity))
            filtered = filtered.Where(item => string.Equals(item.Entity, Entity, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(RiskLevel))
            filtered = filtered.Where(item => string.Equals(item.RiskLevel, RiskLevel, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(Surface))
            filtered = filtered.Where(item => string.Equals(item.UiSurface, Surface, StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrWhiteSpace(Search))
        {
            var search = Search.Trim();
            filtered = filtered.Where(item =>
                Contains(item.Label, search)
                || Contains(item.Description, search)
                || Contains(item.Entity, search)
                || Contains(item.EntityLabel, search)
                || Contains(item.Action, search)
                || Contains(item.PagePath, search)
                || Contains(item.HandlerName, search)
                || Contains($"{item.Entity}.{item.Action}", search));
        }

        return filtered;
    }

    private void NormalizeCurrentScope()
    {
        if (Groups.Count == 0)
        {
            GroupKey = null;
            Entity = null;
            return;
        }

        var selectedGroup = Groups.FirstOrDefault(group => string.Equals(group.GroupKey, GroupKey, StringComparison.OrdinalIgnoreCase));
        if (selectedGroup is null)
        {
            selectedGroup = Groups.FirstOrDefault(group => group.Entities.Any(entity => string.Equals(entity.Entity, Entity, StringComparison.OrdinalIgnoreCase)))
                ?? Groups[0];
            GroupKey = selectedGroup.GroupKey;
        }

        if (string.IsNullOrWhiteSpace(Entity) || selectedGroup.Entities.All(item => !string.Equals(item.Entity, Entity, StringComparison.OrdinalIgnoreCase)))
            Entity = selectedGroup.Entities.FirstOrDefault()?.Entity;
    }

    private static IReadOnlyList<PermissionGroupNode> BuildGroups(IEnumerable<PermissionAction> actions) =>
        actions
            .Where(item => item.IsActive || !item.IsDeprecated)
            .GroupBy(item => new { item.GroupKey, item.GroupLabel })
            .OrderBy(group => group.Min(item => item.DisplayOrder))
            .Select(group => new PermissionGroupNode(
                group.Key.GroupKey,
                group.Key.GroupLabel,
                group.Count(),
                group
                    .GroupBy(item => new { item.Entity, item.EntityLabel })
                    .OrderBy(entityGroup => entityGroup.Min(item => item.DisplayOrder))
                    .Select(entityGroup => new PermissionEntityNode(entityGroup.Key.Entity, entityGroup.Key.EntityLabel, entityGroup.Count()))
                    .ToList()))
            .ToList();

    private async Task<IReadOnlyList<UserOption>> LoadUserOptionsAsync(CancellationToken cancellationToken)
    {
        var query = _db.Users
            .AsNoTracking()
            .Include(user => user.Profile)
            .Where(user => user.Role == UserRole.Admin || user.Role == UserRole.EventPlanner || user.Role == UserRole.PlatformSupportTeam);

        if (!string.IsNullOrWhiteSpace(UserSearch))
        {
            var search = UserSearch.Trim();
            query = query.Where(user =>
                user.MobileNumber.Contains(search)
                || (user.Email != null && user.Email.Contains(search))
                || (user.Profile != null && user.Profile.DisplayName.Contains(search)));
        }

        var users = await query
            .OrderBy(user => user.Role)
            .ThenBy(user => user.Profile != null ? user.Profile.DisplayName : user.MobileNumber)
            .Take(50)
            .ToListAsync(cancellationToken);

        return users
            .Select(user => new UserOption(user.Id, user.Profile?.DisplayName ?? user.Email ?? user.MobileNumber, user.MobileNumber, RoleTitle(user.Role)))
            .ToList();
    }

    private async Task<IReadOnlyList<UserOverrideRow>> LoadOverridesAsync(IReadOnlyList<PermissionAction> actions, CancellationToken cancellationToken)
    {
        var actionLabels = actions.ToDictionary(item => BuildActionKey(item.Entity, item.Action), item => item.Label, StringComparer.OrdinalIgnoreCase);
        var query = _db.UserOperationPermissionOverrides
            .AsNoTracking()
            .Include(item => item.User)
                .ThenInclude(user => user.Profile)
            .Where(item => item.User.Role == UserRole.Admin || item.User.Role == UserRole.EventPlanner || item.User.Role == UserRole.PlatformSupportTeam)
            .Where(item => string.IsNullOrWhiteSpace(Entity) || item.Entity == Entity);

        var overrides = await query
            .OrderByDescending(item => item.CreatedAt)
            .Take(50)
            .ToListAsync(cancellationToken);

        return overrides
            .Select(item => new UserOverrideRow(
                item.Id,
                item.User.Profile?.DisplayName ?? item.User.Email ?? item.User.MobileNumber,
                item.User.MobileNumber,
                item.Entity,
                item.Action,
                actionLabels.TryGetValue(BuildActionKey(item.Entity, item.Action), out var label) ? label : item.Action,
                item.Allowed,
                item.ExpiresAtUtc,
                item.Note))
            .ToList();
    }

    public static string GetRiskClass(string? riskLevel) => (riskLevel ?? string.Empty).Trim().ToLowerInvariant() switch
    {
        "critical" => "status-cancelled",
        "high" => "status-rejected",
        "medium" => "status-pending",
        _ => "status-approved"
    };

    public static string GetRiskTitle(string? riskLevel) => (riskLevel ?? string.Empty).Trim().ToLowerInvariant() switch
    {
        "critical" => "بحرانی",
        "high" => "بالا",
        "medium" => "متوسط",
        "low" => "پایین",
        var value when !string.IsNullOrWhiteSpace(value) => value,
        _ => "نامشخص"
    };

    public static string GetSurfaceTitle(string? surface) => (surface ?? string.Empty).Trim() switch
    {
        "PageAccess" => "دسترسی صفحه",
        "GridAction" => "عملیات جدول",
        "SensitiveData" => "داده حساس",
        "SensitiveAction" => "عملیات حساس",
        "FormSubmit" => "ثبت فرم",
        "Export" => "خروجی",
        "Manual" => "دستی",
        var value when !string.IsNullOrWhiteSpace(value) => value,
        _ => "نامشخص"
    };

    private static string RoleTitle(UserRole role) => role switch
    {
        UserRole.Admin => "مدیر",
        UserRole.EventPlanner => "برگزارکننده",
        UserRole.PlatformSupportTeam => "پشتیبان",
        _ => role.ToString()
    };

    private static bool Contains(string? value, string search) =>
        !string.IsNullOrWhiteSpace(value) && value.Contains(search, StringComparison.OrdinalIgnoreCase);

    private static string BuildActionKey(string entity, string action) => $"{entity}{ActionKeySeparator}{action}";

    private static bool TryParseActionKey(string? actionKey, out string entity, out string action)
    {
        entity = string.Empty;
        action = string.Empty;
        if (string.IsNullOrWhiteSpace(actionKey))
            return false;

        var parts = actionKey.Split(ActionKeySeparator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
            return false;

        entity = parts[0];
        action = parts[1];
        return !string.IsNullOrWhiteSpace(entity) && !string.IsNullOrWhiteSpace(action);
    }

    public sealed record RoleOption(UserRole Role, string Title, string CssClass);
    public sealed record RolePermissionValue(UserRole Role, string RoleTitle, bool Allowed, bool IsConfigured);
    public sealed record PermissionMatrixRow(PermissionActionRow Action, IReadOnlyList<RolePermissionValue> RoleValues);

    public sealed record PermissionActionRow(
        string Entity,
        string EntityLabel,
        string Action,
        string TechnicalKey,
        string Label,
        string? Description,
        string RiskLevel,
        string UiSurface,
        string? PagePath,
        string? HandlerName,
        bool IsActive,
        bool IsDeprecated)
    {
        public static PermissionActionRow From(PermissionAction action) => new(
            action.Entity,
            action.EntityLabel,
            action.Action,
            $"{action.Entity}.{action.Action}",
            action.Label,
            action.Description,
            action.RiskLevel,
            action.UiSurface,
            action.PagePath,
            action.HandlerName,
            action.IsActive,
            action.IsDeprecated);
    }

    public sealed record PermissionGroupNode(string GroupKey, string GroupLabel, int ActionCount, IReadOnlyList<PermissionEntityNode> Entities);
    public sealed record PermissionEntityNode(string Entity, string EntityLabel, int ActionCount);
    public sealed record UserOption(long UserId, string DisplayName, string MobileNumber, string RoleTitle);
    public sealed record UserOverrideRow(long Id, string UserTitle, string UserMobile, string Entity, string Action, string ActionLabel, bool Allowed, DateTime? ExpiresAtUtc, string? Note);
    public sealed record RolePermissionInput(UserRole Role, string Entity, string Action, bool Allowed);

    public sealed class UserOverrideInput
    {
        [Required(ErrorMessage = "کاربر را انتخاب کنید.")]
        public long UserId { get; set; }

        [Required(ErrorMessage = "عملیات را انتخاب کنید.")]
        public string ActionKey { get; set; } = string.Empty;

        public bool Allowed { get; set; } = true;

        public DateTime? ExpiresAtUtc { get; set; }

        [StringLength(500, ErrorMessage = "یادداشت نمی‌تواند بیشتر از ۵۰۰ کاراکتر باشد.")]
        public string? Note { get; set; }
    }
}
