using MediatR;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.DatingEvents.Commands.SetDatingEventCommission;

public class SetDatingEventCommissionHandler : IRequestHandler<SetDatingEventCommissionCommand>
{
    private readonly IDatingEventRepository _events;
    private readonly IUnitOfWork _unitOfWork;

    public SetDatingEventCommissionHandler(IDatingEventRepository events, IUnitOfWork unitOfWork)
    {
        _events = events;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(SetDatingEventCommissionCommand request, CancellationToken cancellationToken)
    {
        var datingEvent = await _events.GetByIdAsync(request.EventId, cancellationToken)
            ?? throw new NotFoundException("DatingEvent", request.EventId);

        datingEvent.SetCommissionPercent(request.CommissionPercent);
        await _events.UpdateAsync(datingEvent, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
