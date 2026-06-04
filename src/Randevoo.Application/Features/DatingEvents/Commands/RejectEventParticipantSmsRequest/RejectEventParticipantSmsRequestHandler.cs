using MediatR;
using Randevoo.Domain.Enums;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.DatingEvents.Commands.RejectEventParticipantSmsRequest;

public class RejectEventParticipantSmsRequestHandler : IRequestHandler<RejectEventParticipantSmsRequestCommand>
{
    private readonly IUserRepository _users;
    private readonly IEventParticipantSmsRequestRepository _requests;
    private readonly IUnitOfWork _unitOfWork;

    public RejectEventParticipantSmsRequestHandler(
        IUserRepository users,
        IEventParticipantSmsRequestRepository requests,
        IUnitOfWork unitOfWork)
    {
        _users = users;
        _requests = requests;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RejectEventParticipantSmsRequestCommand request, CancellationToken cancellationToken)
    {
        var admin = await _users.GetByIdAsync(request.AdminUserId, cancellationToken)
            ?? throw new NotFoundException("User", request.AdminUserId);
        if (admin.Role != UserRole.Admin)
        {
            throw new BusinessRuleViolationException(
                "Access denied",
                "Only admins can reject participant SMS requests");
        }

        var smsRequest = await _requests.GetByIdAsync(request.RequestId, cancellationToken)
            ?? throw new NotFoundException("EventParticipantSmsRequest", request.RequestId);

        smsRequest.Reject(admin.Id, request.Note);
        await _requests.UpdateAsync(smsRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
