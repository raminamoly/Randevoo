using Randevoo.Domain.Entities;
using Randevoo.Domain.Enums;

namespace Randevoo.Application.Features.SupportTickets.Common;

public record SupportTicketListItemDto(
    long Id,
    string Title,
    SupportTicketCategory Category,
    SupportTicketStatus Status,
    long SubmitterUserId,
    string SubmitterDisplayName,
    string SubmitterMobileNumber,
    UserRole SubmitterRole,
    long? AssignedSupportUserId,
    string? AssignedSupportDisplayName,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record SupportTicketDetailDto(
    long Id,
    string Title,
    SupportTicketCategory Category,
    SupportTicketStatus Status,
    SubmitterContextDto Submitter,
    long? AssignedSupportUserId,
    string? AssignedSupportDisplayName,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    DateTime? ClosedAtUtc,
    IReadOnlyList<SupportTicketMessageDto> Messages,
    IReadOnlyList<SupportTicketHistoryDto> History);

public record SupportTicketMessageDto(
    long Id,
    long SenderUserId,
    string SenderDisplayName,
    UserRole SenderRole,
    long? RepresentedUserId,
    string? RepresentedDisplayName,
    string Body,
    DateTime CreatedAt,
    IReadOnlyList<SupportTicketAttachmentDto> Attachments);

public record SupportTicketAttachmentDto(long Id, string FileName, string ContentType, long SizeBytes, string Url);

public record SupportTicketHistoryDto(
    long Id,
    long ActorUserId,
    string ActorDisplayName,
    string Action,
    SupportTicketStatus? OldStatus,
    SupportTicketStatus? NewStatus,
    long? OldAssigneeUserId,
    long? NewAssigneeUserId,
    string? Note,
    DateTime CreatedAt);

public record SubmitterContextDto(
    long UserId,
    string DisplayName,
    string MobileNumber,
    string? Email,
    string? ProfileImageUrl,
    UserRole Role,
    bool IsActive);

public static class SupportTicketDtoMapper
{
    public static SupportTicketListItemDto ToListItem(SupportTicket ticket) =>
        new(
            ticket.Id,
            ticket.Title,
            ticket.Category,
            ticket.Status,
            ticket.SubmitterUserId,
            ResolveDisplayName(ticket.SubmitterUser),
            ticket.SubmitterUser.MobileNumber,
            ticket.SubmitterRole,
            ticket.AssignedSupportUserId,
            ticket.AssignedSupportUser is null ? null : ResolveDisplayName(ticket.AssignedSupportUser),
            ticket.CreatedAt,
            ticket.UpdatedAt);

    public static SupportTicketDetailDto ToDetail(SupportTicket ticket) =>
        new(
            ticket.Id,
            ticket.Title,
            ticket.Category,
            ticket.Status,
            new SubmitterContextDto(
                ticket.SubmitterUserId,
                ResolveDisplayName(ticket.SubmitterUser),
                ticket.SubmitterUser.MobileNumber,
                ticket.SubmitterUser.Email,
                ticket.SubmitterUser.Profile?.Images
                    .OrderByDescending(image => image.IsPrimary)
                    .ThenBy(image => image.DisplayOrder)
                    .Select(image => image.ImageUrl)
                    .FirstOrDefault(),
                ticket.SubmitterRole,
                ticket.SubmitterUser.IsActive),
            ticket.AssignedSupportUserId,
            ticket.AssignedSupportUser is null ? null : ResolveDisplayName(ticket.AssignedSupportUser),
            ticket.CreatedAt,
            ticket.UpdatedAt,
            ticket.ClosedAtUtc,
            ticket.Messages.OrderBy(message => message.CreatedAt).Select(message => new SupportTicketMessageDto(
                message.Id,
                message.SenderUserId,
                ResolveDisplayName(message.SenderUser),
                message.SenderRole,
                message.RepresentedUserId,
                message.RepresentedUser is null ? null : ResolveDisplayName(message.RepresentedUser),
                message.Body,
                message.CreatedAt,
                message.Attachments.Select(attachment => new SupportTicketAttachmentDto(
                    attachment.Id,
                    attachment.FileName,
                    attachment.ContentType,
                    attachment.SizeBytes,
                    attachment.Url)).ToList())).ToList(),
            ticket.History.OrderByDescending(history => history.CreatedAt).Select(history => new SupportTicketHistoryDto(
                history.Id,
                history.ActorUserId,
                ResolveDisplayName(history.ActorUser),
                history.Action,
                history.OldStatus,
                history.NewStatus,
                history.OldAssigneeUserId,
                history.NewAssigneeUserId,
                history.Note,
                history.CreatedAt)).ToList());

    public static string ResolveDisplayName(User user) =>
        user.Profile?.DisplayName ?? user.Email ?? user.MobileNumber;
}
