using Randevoo.AdminPanel.Models.Auth;
using Randevoo.AdminPanel.Models.Support;
using Randevoo.Application.Features.SupportTickets.Common;
using Randevoo.Domain.Enums;

namespace Randevoo.AdminPanel.Services.ApiClients;

public interface ISupportTicketsApiClient
{
    Task<IReadOnlyList<SupportTicketListItemDto>> GetTicketsAsync(MockUser currentUser, SupportTicketStatus? status, SupportTicketCategory? category, UserRole? submitterRole, long? assigneeUserId, DateTime? createdFromUtc = null, DateTime? createdToUtc = null, CancellationToken cancellationToken = default);
    Task<SupportTicketDashboardViewModel> GetDashboardAsync(MockUser currentUser, SupportTicketDashboardFilters filters, CancellationToken cancellationToken = default);
    Task<SupportTicketDetailDto> GetTicketAsync(MockUser currentUser, long ticketId, CancellationToken cancellationToken = default);
    Task<SupportTicketDetailDto> CreateTicketAsync(MockUser currentUser, string title, SupportTicketCategory category, string body, IReadOnlyList<SupportTicketAttachmentInput> attachments, CancellationToken cancellationToken = default);
    Task<SupportTicketDetailDto> ReplyAsync(MockUser currentUser, long ticketId, string body, IReadOnlyList<SupportTicketAttachmentInput> attachments, long? representedUserId, CancellationToken cancellationToken = default);
    Task<SupportTicketDetailDto> ChangeStatusAsync(MockUser currentUser, long ticketId, SupportTicketStatus status, string? note, CancellationToken cancellationToken = default);
    Task<SupportTicketDetailDto> ReassignAsync(MockUser currentUser, long ticketId, long? assigneeUserId, string? note, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<(long Id, string DisplayName)>> GetSupportUsersAsync(MockUser currentUser, CancellationToken cancellationToken = default);
    Task<SupportSubmitterFinanceContext> GetSubmitterFinanceAsync(MockUser currentUser, long ticketId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupportSubmitterEventBookingItem>> GetSubmitterEventsAsync(MockUser currentUser, long ticketId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupportTicketListItemDto>> GetSubmitterPreviousTicketsAsync(MockUser currentUser, long ticketId, CancellationToken cancellationToken = default);
}
