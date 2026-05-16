using Randevoo.Domain.Common;
using Randevoo.Domain.Common.Events;
using Randevoo.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Randevoo.Domain.Events
{
    public class EntitySoftDeletedEvent : DomainEvent
    {
        public BaseEntity Entity { get; }

        public EntitySoftDeletedEvent(BaseEntity entity)
        {
            Entity = entity;
        }
    }

    public class EntityRestoredEvent : DomainEvent
    {
        public BaseEntity Entity { get; }

        public EntityRestoredEvent(BaseEntity entity)
        {
            Entity = entity;
        }
    }


    public class EntityCreatedEvent<T> : DomainEvent where T : BaseEntity
    {
        public T Entity { get; }
        public EntityCreatedEvent(T entity)
        {
            Entity = entity;
        }
    }

    public class EntityUpdatedEvent<T> : DomainEvent where T : BaseEntity
    {
        public T Entity { get; }
        public string UpdatedField { get; }
        public object OldValue { get; }
        public object NewValue { get; }

        public EntityUpdatedEvent(T entity, string updatedField, object oldValue, object newValue)
        {
            Entity = entity;
            UpdatedField = updatedField;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }

}

