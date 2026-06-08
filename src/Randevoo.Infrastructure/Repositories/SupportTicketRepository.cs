using Microsoft.EntityFrameworkCore;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Interfaces.Repositories;
using Randevoo.Infrastructure.Data;

namespace Randevoo.Infrastructure.Repositories;

public class SupportTicketRepository : ISupportTicketRepository
{
    private const string DefaultQueueName = "platform-support";
    private readonly RandevooDbContext _db;

    public SupportTicketRepository(RandevooDbContext db)
    {
        _db = db;
    }

    public Task<SupportTicket?> GetByIdWithDetailsAsync(long id, CancellationToken cancellationToken = default)
    {
        return _db.SupportTickets
            .Include(ticket => ticket.SubmitterUser).ThenInclude(user => user.Profile)!.ThenInclude(profile => profile!.Images)
            .Include(ticket => ticket.AssignedSupportUser).ThenInclude(user => user!.Profile)
            .Include(ticket => ticket.Messages).ThenInclude(message => message.SenderUser).ThenInclude(user => user.Profile)
            .Include(ticket => ticket.Messages).ThenInclude(message => message.RepresentedUser).ThenInclude(user => user!.Profile)
            .Include(ticket => ticket.Messages).ThenInclude(message => message.Attachments)
            .Include(ticket => ticket.History).ThenInclude(history => history.ActorUser).ThenInclude(user => user.Profile)
            .FirstOrDefaultAsync(ticket => ticket.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<SupportTicket>> ListAsync(
        long requesterUserId,
        UserRole requesterRole,
        SupportTicketStatus? status = null,
        SupportTicketCategory? category = null,
        UserRole? submitterRole = null,
        long? assigneeUserId = null,
        DateTime? createdFromUtc = null,
        DateTime? createdToUtc = null,
        int limit = 100,
        CancellationToken cancellationToken = default)
    {
        var query = _db.SupportTickets
            .Include(ticket => ticket.SubmitterUser).ThenInclude(user => user.Profile)
            .Include(ticket => ticket.AssignedSupportUser).ThenInclude(user => user!.Profile)
            .AsQueryable();

        if (requesterRole is not (UserRole.Admin or UserRole.PlatformSupportTeam))
            query = query.Where(ticket => ticket.SubmitterUserId == requesterUserId);

        if (status is not null)
            query = query.Where(ticket => ticket.Status == status);
        if (category is not null)
            query = query.Where(ticket => ticket.Category == category);
        if (submitterRole is not null)
            query = query.Where(ticket => ticket.SubmitterRole == submitterRole);
        if (assigneeUserId is not null)
            query = query.Where(ticket => ticket.AssignedSupportUserId == assigneeUserId);
        if (createdFromUtc is not null)
            query = query.Where(ticket => ticket.CreatedAt >= createdFromUtc);
        if (createdToUtc is not null)
            query = query.Where(ticket => ticket.CreatedAt <= createdToUtc);

        return await query
            .OrderBy(ticket => ticket.Status == SupportTicketStatus.Closed)
            .ThenByDescending(ticket => ticket.UpdatedAt ?? ticket.CreatedAt)
            .Take(Math.Clamp(limit, 1, 250))
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> GetNextRoundRobinAssigneeAsync(CancellationToken cancellationToken = default)
    {
        var supportUsers = await _db.Users
            .Where(user => user.Role == UserRole.PlatformSupportTeam && user.IsActive)
            .OrderBy(user => user.Id)
            .ToListAsync(cancellationToken);

        if (supportUsers.Count == 0)
            return null;

        var cursor = await _db.SupportTicketAssignmentCursors
            .FirstOrDefaultAsync(item => item.QueueName == DefaultQueueName, cancellationToken);

        if (cursor is null)
        {
            cursor = new SupportTicketAssignmentCursor(DefaultQueueName);
            _db.SupportTicketAssignmentCursors.Add(cursor);
        }

        var next = supportUsers.FirstOrDefault(user => cursor.LastAssignedUserId is null || user.Id > cursor.LastAssignedUserId.Value)
            ?? supportUsers[0];

        cursor.MarkAssigned(next.Id);
        return next;
    }

    public async Task AddAsync(SupportTicket ticket, CancellationToken cancellationToken = default)
    {
        _db.SupportTickets.Add(ticket);
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(SupportTicket ticket, CancellationToken cancellationToken = default)
    {
        _db.SupportTickets.Update(ticket);
        await Task.CompletedTask;
    }
}
