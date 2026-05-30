using MediatR;
using Randevoo.Application.Features.EventTypes.Common;
using Randevoo.Domain.Entities;
using Randevoo.Domain.Exceptions;
using Randevoo.Domain.Interfaces;
using Randevoo.Domain.Interfaces.Repositories;

namespace Randevoo.Application.Features.EventTypes.Commands.UpsertEventType;

public class UpsertEventTypeHandler : IRequestHandler<UpsertEventTypeCommand, EventTypeDto>
{
    private readonly IEventTypeRepository _eventTypes;
    private readonly IUnitOfWork _unitOfWork;

    public UpsertEventTypeHandler(IEventTypeRepository eventTypes, IUnitOfWork unitOfWork)
    {
        _eventTypes = eventTypes;
        _unitOfWork = unitOfWork;
    }

    public async Task<EventTypeDto> Handle(UpsertEventTypeCommand request, CancellationToken cancellationToken)
    {
        EventType eventType;
        if (request.Id is null)
        {
            eventType = new EventType(request.Name, request.Description);
            eventType.Update(request.Name, request.Description, request.IsActive);
            await _eventTypes.AddAsync(eventType, cancellationToken);
        }
        else
        {
            eventType = await _eventTypes.GetByIdAsync(request.Id.Value, cancellationToken)
                ?? throw new NotFoundException("EventType", request.Id.Value);
            eventType.Update(request.Name, request.Description, request.IsActive);
            await _eventTypes.UpdateAsync(eventType, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return EventTypeDto.FromEntity(eventType);
    }
}
