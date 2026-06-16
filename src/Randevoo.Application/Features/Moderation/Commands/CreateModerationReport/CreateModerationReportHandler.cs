using MediatR;
using Randevoo.Application.Features.Moderation.Common;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.Moderation.Commands.CreateModerationReport;

public class CreateModerationReportHandler : IRequestHandler<CreateModerationReportCommand, ModerationReportDto>
{
    private readonly IUserRepository _users;
    private readonly IEventTicketRepository _tickets;
    private readonly IEventConversationRepository _conversations;
    private readonly IModerationReportRepository _reports;
    private readonly IUnitOfWork _unitOfWork;

    public CreateModerationReportHandler(
        IUserRepository users,
        IEventTicketRepository tickets,
        IEventConversationRepository conversations,
        IModerationReportRepository reports,
        IUnitOfWork unitOfWork)
    {
        _users = users;
        _tickets = tickets;
        _conversations = conversations;
        _reports = reports;
        _unitOfWork = unitOfWork;
    }

    public async Task<ModerationReportDto> Handle(CreateModerationReportCommand request, CancellationToken cancellationToken)
    {
        var reporter = await _users.GetByIdAsync(request.ReporterUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.ReporterUserId);
        var reported = await _users.GetByIdAsync(request.ReportedUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.ReportedUserId);

        if (request.ReporterUserId == request.ReportedUserId)
            throw new BusinessRuleViolationException("Invalid report", "User cannot report themselves");

        if (await _reports.HasOpenDuplicateAsync(request.ReporterUserId, request.ReportedUserId, request.DatingEventId, cancellationToken))
            throw new BusinessRuleViolationException("Duplicate report", "An open report already exists for this user in this event");

        if (request.DatingEventId is not null)
        {
            var reporterTicket = await _tickets.GetByEventAndUserAsync(request.DatingEventId.Value, request.ReporterUserId, cancellationToken);
            var reportedTicket = await _tickets.GetByEventAndUserAsync(request.DatingEventId.Value, request.ReportedUserId, cancellationToken);
            if (reporterTicket is null || reportedTicket is null)
                throw new BusinessRuleViolationException("Invalid report event", "Both users must belong to the reported event");
        }

        if (request.EventConversationId is not null)
        {
            var conversation = await _conversations.GetByIdWithDetailsAsync(request.EventConversationId.Value, cancellationToken)
                ?? throw new NotFoundException("EventConversation", request.EventConversationId.Value);
            if (!conversation.HasParticipant(request.ReporterUserId))
                throw new BusinessRuleViolationException("Access denied", "Reporter must be part of the reported conversation");
        }

        var report = new ModerationReport(reporter, reported, request.Reason, request.Description, request.DatingEventId, request.EventConversationId);
        await _reports.AddAsync(report, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ModerationReportDto.FromEntity(report);
    }
}
