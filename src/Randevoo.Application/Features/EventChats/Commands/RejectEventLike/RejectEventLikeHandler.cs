using MediatR;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.EventChats.Commands.RejectEventLike;

public class RejectEventLikeHandler : IRequestHandler<RejectEventLikeCommand>
{
    private readonly IEventLikeRepository _likes;
    private readonly IUnitOfWork _unitOfWork;

    public RejectEventLikeHandler(IEventLikeRepository likes, IUnitOfWork unitOfWork)
    {
        _likes = likes;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RejectEventLikeCommand request, CancellationToken cancellationToken)
    {
        var eventLike = await _likes.GetDirectedAsync(request.EventId, request.FromUserId, request.RejectingUserId, cancellationToken)
            ?? throw new NotFoundException("EventLike", $"{request.EventId}:{request.FromUserId}:{request.RejectingUserId}");

        eventLike.Reject(request.RejectingUserId);
        await _likes.UpdateAsync(eventLike, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
