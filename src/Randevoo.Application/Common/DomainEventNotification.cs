using MediatR;
using Randevoo.Domain.Common.Events;

namespace Randevoo.Application.Common;

public record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent) : INotification
    where TDomainEvent : IDomainEvent;
