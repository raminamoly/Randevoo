using Microsoft.EntityFrameworkCore;
using Randevoo.Application.Features.Privacy.Common;
using Randevoo.Application.Interfaces.Privacy;
using Randevoo.Domain.Exceptions;
using Randevoo.Infrastructure.Data;

namespace Randevoo.Infrastructure.Services;

public class PrivacyDataReader : IPrivacyDataReader
{
    private readonly RandevooDbContext _db;

    public PrivacyDataReader(RandevooDbContext db)
    {
        _db = db;
    }

    public async Task<PrivacyExportDto> ExportUserDataAsync(long userId, CancellationToken cancellationToken = default)
    {
        var user = await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new NotFoundException("User", userId);

        var profile = await _db.UserProfiles
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .Select(p => new
            {
                p.DisplayName,
                p.Gender,
                p.DateOfBirth,
                p.EducationLevel,
                p.Smoking,
                Country = p.Country == null ? string.Empty : p.Country.Name,
                City = p.City == null ? string.Empty : p.City.Name,
                p.Location.Region
            })
            .FirstOrDefaultAsync(cancellationToken);

        var plannerProfile = await _db.EventPlannerProfiles
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .Select(p => new
            {
                p.Title,
                p.PictureUrl,
                p.Resume,
                p.AverageRating,
                p.TotalSurveyCount,
                p.HostedEventCount,
                p.CancelledEventCount,
                p.CompletedEventCount
            })
            .FirstOrDefaultAsync(cancellationToken);

        var balance = await _db.BalanceAccounts
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .Select(a => new { a.Balance })
            .FirstOrDefaultAsync(cancellationToken);

        var ticketRows = await _db.EventTickets
            .AsNoTracking()
            .Where(t => t.UserId == userId)
            .Select(t => new
            {
                t.Id,
                t.DatingEventId,
                t.Price,
                t.IsRefunded,
                t.IsRemoved,
                t.CreatedAt
            })
            .ToListAsync(cancellationToken);
        var tickets = ticketRows.Cast<object>().ToList();

        return new PrivacyExportDto(
            user.Id,
            user.MobileNumber,
            user.Email,
            user.IsEmailConfirmed,
            user.Role.ToString(),
            profile,
            plannerProfile,
            balance,
            tickets);
    }
}
