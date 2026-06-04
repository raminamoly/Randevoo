using Microsoft.EntityFrameworkCore;
using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Users;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.ValueObjects;
using Randevoo.Infrastructure.Data;

namespace Randevoo.AdminPanel.Services.ApiClients;

public sealed class DatabasePlannerProfilesApiClient : IPlannerProfilesApiClient
{
    private readonly RandevooDbContext _db;
    private readonly IUnitOfWork _unitOfWork;

    public DatabasePlannerProfilesApiClient(RandevooDbContext db, IUnitOfWork unitOfWork)
    {
        _db = db;
        _unitOfWork = unitOfWork;
    }

    public Task<PlannerProfileViewModel?> GetCurrentAsync(MockUser currentUser, CancellationToken cancellationToken = default)
        => GetByUserIdAsync(currentUser.Id, cancellationToken);

    public async Task<IReadOnlyList<PlannerProfileApprovalItem>> ListForApprovalAsync(CancellationToken cancellationToken = default)
    {
        var profiles = await _db.EventPlannerProfiles
            .Include(item => item.User)
            .ThenInclude(user => user.Profile)
            .OrderByDescending(item => item.HasPendingChanges)
            .ThenByDescending(item => item.PendingSubmittedAt)
            .ThenBy(item => item.User.Profile != null ? item.User.Profile.DisplayName : item.User.MobileNumber)
            .ToListAsync(cancellationToken);

        var plannerIds = profiles.Select(item => item.UserId).ToList();
        var hostedCounts = await _db.DatingEvents
            .Where(item => plannerIds.Contains(item.EventPlannerUserId))
            .GroupBy(item => item.EventPlannerUserId)
            .Select(group => new { UserId = group.Key, Count = group.Count() })
            .ToDictionaryAsync(item => item.UserId, item => item.Count, cancellationToken);

        return profiles.Select(profile => new PlannerProfileApprovalItem
        {
            UserId = profile.UserId,
            FullName = DatabaseModelMapper.ResolveUserDisplayName(profile.User),
            Title = profile.Title,
            City = profile.User.Profile?.City?.Name ?? string.Empty,
            HasPendingChanges = profile.HasPendingChanges,
            PendingTitle = profile.PendingTitle,
            PendingSubmittedAtUtc = profile.PendingSubmittedAt is null ? null : DateTime.SpecifyKind(profile.PendingSubmittedAt.Value, DateTimeKind.Utc),
            HostedEventCount = hostedCounts.TryGetValue(profile.UserId, out var count) ? count : 0
        }).ToList();
    }

    public async Task<PlannerProfileViewModel?> GetByUserIdAsync(long userId, CancellationToken cancellationToken = default)
    {
        var profile = await _db.EventPlannerProfiles
            .Include(item => item.User)
            .ThenInclude(user => user.Profile)
            .FirstOrDefaultAsync(item => item.UserId == userId, cancellationToken);

        if (profile is null)
            return null;

        return await BuildViewModelAsync(profile, cancellationToken);
    }

    public async Task<PlannerProfileViewModel> UpsertAsync(MockUser currentUser, PlannerProfileInput input, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users
            .Include(item => item.Profile)
            .FirstOrDefaultAsync(item => item.Id == currentUser.Id, cancellationToken)
            ?? throw new InvalidOperationException("کاربر برگزارکننده پیدا نشد.");

        if (user.Role != UserRole.EventPlanner && user.Role != UserRole.Admin)
        {
            user.ChangeUserRole(UserRole.EventPlanner);
        }

        var profile = await _db.EventPlannerProfiles
            .Include(item => item.User)
            .ThenInclude(planner => planner.Profile)
            .FirstOrDefaultAsync(item => item.UserId == currentUser.Id, cancellationToken);

        if (profile is null)
        {
            profile = new EventPlannerProfile(
                user,
                NormalizeTitle(input.Title),
                input.PictureUrl,
                NormalizeResume(input.Resume));

            _db.EventPlannerProfiles.Add(profile);
            EnsureProfile(user, input.FullName, input.City);
        }
        else
        {
            profile.SubmitChangesForApproval(
                NormalizeFullName(user, input.FullName),
                NormalizeCity(input.City),
                NormalizeTitle(input.Title),
                input.PictureUrl,
                NormalizeResume(input.Resume));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await BuildViewModelAsync(profile, cancellationToken);
    }

    public async Task<PlannerProfileViewModel> ApproveAsync(MockUser adminUser, long plannerUserId, PlannerProfileApprovalInput input, CancellationToken cancellationToken = default)
    {
        var admin = await _db.Users
            .Include(item => item.Profile)
            .FirstOrDefaultAsync(item => item.Id == adminUser.Id, cancellationToken)
            ?? throw new InvalidOperationException("مدیر جاری پیدا نشد.");
        if (admin.Role != UserRole.Admin)
            throw new InvalidOperationException("فقط مدیر می تواند پروفایل برگزارکننده را تایید کند.");

        var profile = await _db.EventPlannerProfiles
            .Include(item => item.User)
            .ThenInclude(user => user.Profile)
            .FirstOrDefaultAsync(item => item.UserId == plannerUserId, cancellationToken)
            ?? throw new InvalidOperationException("پروفایل برگزارکننده پیدا نشد.");

        var normalizedFullName = NormalizeFullName(profile.User, input.FullName);
        var normalizedCity = NormalizeCity(input.City);
        EnsureProfile(profile.User, normalizedFullName, normalizedCity);
        profile.PublishApprovedChanges(
            normalizedFullName,
            normalizedCity,
            NormalizeTitle(input.Title),
            input.PictureUrl,
            NormalizeResume(input.Resume),
            admin.Id,
            input.ReviewNote);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await BuildViewModelAsync(profile, cancellationToken);
    }

    public async Task<PlannerProfileViewModel> RejectAsync(MockUser adminUser, long plannerUserId, string? reviewNote, CancellationToken cancellationToken = default)
    {
        var admin = await _db.Users.FirstOrDefaultAsync(item => item.Id == adminUser.Id, cancellationToken)
            ?? throw new InvalidOperationException("مدیر جاری پیدا نشد.");
        if (admin.Role != UserRole.Admin)
            throw new InvalidOperationException("فقط مدیر می تواند پروفایل برگزارکننده را رد کند.");

        var profile = await _db.EventPlannerProfiles
            .Include(item => item.User)
            .ThenInclude(user => user.Profile)
            .FirstOrDefaultAsync(item => item.UserId == plannerUserId, cancellationToken)
            ?? throw new InvalidOperationException("پروفایل برگزارکننده پیدا نشد.");

        if (!profile.HasPendingChanges)
            throw new InvalidOperationException("تغییر در انتظار تاییدی برای این پروفایل وجود ندارد.");

        profile.RejectPendingChanges(admin.Id, reviewNote);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await BuildViewModelAsync(profile, cancellationToken);
    }

    private async Task<PlannerProfileViewModel> BuildViewModelAsync(EventPlannerProfile profile, CancellationToken cancellationToken)
    {
        var hostedEventCount = await _db.DatingEvents.CountAsync(item => item.EventPlannerUserId == profile.UserId, cancellationToken);
        var cancelledEventCount = await _db.DatingEvents.CountAsync(item => item.EventPlannerUserId == profile.UserId && item.IsCancelled, cancellationToken);
        var completedEventCount = await _db.DatingEvents.CountAsync(item => item.EventPlannerUserId == profile.UserId && !item.IsCancelled && item.DateTimeEnd <= DateTime.UtcNow, cancellationToken);
        return DatabaseModelMapper.ToPlannerProfileViewModel(profile, hostedEventCount, cancelledEventCount, completedEventCount);
    }

    private static void EnsureProfile(User user, string fullName, string city)
    {
        var normalizedFullName = NormalizeFullName(user, fullName);
        var normalizedCity = NormalizeCity(city);
        var location = new Location("ایران", normalizedCity, new Coordinates(35.7219m, 51.3347m), normalizedCity);

        if (user.Profile is null)
        {
            user.CreateProfile(normalizedFullName, new DateOnly(1990, 1, 1), Gender.Unknown, location, new Height(170));
            return;
        }

        user.Profile.UpdateDisplayName(normalizedFullName);
        user.Profile.UpdateLocation(location);
    }

    private static string NormalizeFullName(User user, string fullName)
        => string.IsNullOrWhiteSpace(fullName) ? user.MobileNumber : fullName.Trim();

    private static string NormalizeCity(string city)
        => string.IsNullOrWhiteSpace(city) ? "تهران" : city.Trim();

    private static string NormalizeTitle(string title)
        => string.IsNullOrWhiteSpace(title) ? "برگزارکننده رندوو" : title.Trim();

    private static string NormalizeResume(string resume)
        => string.IsNullOrWhiteSpace(resume)
            ? "پروفایل برگزارکننده در حال تکمیل است و اطلاعات کامل تر به زودی ثبت می شود."
            : resume.Trim();
}
