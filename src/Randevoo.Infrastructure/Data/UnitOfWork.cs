using MediatR;
using Randevoo.Application.Common;
using Randevoo.Domain.Common;
using Randevoo.Domain.Common.Events;
using Randevoo.Domain.Interfaces;

namespace Randevoo.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly RandevooDbContext _db;
    private readonly IPublisher _publisher;

    public UnitOfWork(RandevooDbContext db, IPublisher publisher)
    {
        _db = db;
        _publisher = publisher;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = _db.ChangeTracker
            .Entries<BaseEntity>()
            .SelectMany(entry => entry.Entity.DomainEvents)
            .ToList();

        var result = await _db.SaveChangesAsync(cancellationToken);

        foreach (var entity in _db.ChangeTracker.Entries<BaseEntity>())
            entity.Entity.ClearDomainEvents();

        foreach (var domainEvent in domainEvents)
            await PublishDomainEventAsync(domainEvent, cancellationToken);

        return result;
    }

    private Task PublishDomainEventAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var notificationType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());
        var notification = Activator.CreateInstance(notificationType, domainEvent)
            ?? throw new InvalidOperationException($"Could not create notification for {domainEvent.GetType().Name}.");

        return _publisher.Publish(notification, cancellationToken);
    }
}
