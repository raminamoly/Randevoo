using Microsoft.EntityFrameworkCore;
using Randevoo.Domain.Constants;
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
            .Include(ticket => ticket.TicketType)
            .Include(ticket => ticket.TicketStatus)
            .Include(ticket => ticket.TicketRecipientType)
            .Include(ticket => ticket.DatingEvent)
            .Include(ticket => ticket.RecipientPlannerUser).ThenInclude(user => user!.Profile)
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
        long? ticketStatusId = null,
        long? ticketTypeId = null,
        long? ticketRecipientTypeId = null,
        UserRole? submitterRole = null,
        long? assigneeUserId = null,
        DateTime? createdFromUtc = null,
        DateTime? createdToUtc = null,
        int limit = 100,
        CancellationToken cancellationToken = default)
    {
        var query = _db.SupportTickets
            .Include(ticket => ticket.TicketType)
            .Include(ticket => ticket.TicketStatus)
            .Include(ticket => ticket.TicketRecipientType)
            .Include(ticket => ticket.DatingEvent)
            .Include(ticket => ticket.RecipientPlannerUser).ThenInclude(user => user!.Profile)
            .Include(ticket => ticket.SubmitterUser).ThenInclude(user => user.Profile)
            .Include(ticket => ticket.AssignedSupportUser).ThenInclude(user => user!.Profile)
            .AsQueryable();

        query = requesterRole switch
        {
            UserRole.Admin => query,
            UserRole.PlatformSupportTeam => query.Where(ticket => ticket.TicketRecipientTypeId == SupportTicketLookupIds.RecipientPlatformSupport),
            UserRole.EventPlanner => query.Where(ticket => ticket.SubmitterUserId == requesterUserId || ticket.RecipientPlannerUserId == requesterUserId),
            _ => query.Where(ticket => ticket.SubmitterUserId == requesterUserId)
        };

        if (ticketStatusId is not null)
            query = query.Where(ticket => ticket.TicketStatusId == ticketStatusId);
        if (ticketTypeId is not null)
            query = query.Where(ticket => ticket.TicketTypeId == ticketTypeId);
        if (ticketRecipientTypeId is not null)
            query = query.Where(ticket => ticket.TicketRecipientTypeId == ticketRecipientTypeId);
        if (submitterRole is not null)
            query = query.Where(ticket => ticket.SubmitterRole == submitterRole);
        if (assigneeUserId is not null)
            query = query.Where(ticket => ticket.AssignedSupportUserId == assigneeUserId);
        if (createdFromUtc is not null)
            query = query.Where(ticket => ticket.CreatedAt >= createdFromUtc);
        if (createdToUtc is not null)
            query = query.Where(ticket => ticket.CreatedAt <= createdToUtc);

        return await query
            .OrderBy(ticket => ticket.TicketStatusId == SupportTicketLookupIds.StatusClosed)
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

    public Task<bool> IsTicketTypeActiveAsync(long ticketTypeId, CancellationToken cancellationToken = default)
    {
        return _db.SupportTicketCategories.AnyAsync(item => item.Id == ticketTypeId && item.IsActive, cancellationToken);
    }

    public Task<bool> IsTicketStatusActiveAsync(long ticketStatusId, CancellationToken cancellationToken = default)
    {
        return _db.SupportTicketStatuses.AnyAsync(item => item.Id == ticketStatusId && item.IsActive, cancellationToken);
    }

    public Task<bool> IsTicketRecipientTypeActiveAsync(long ticketRecipientTypeId, CancellationToken cancellationToken = default)
    {
        return _db.SupportTicketRecipientTypes.AnyAsync(item => item.Id == ticketRecipientTypeId && item.IsActive, cancellationToken);
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
