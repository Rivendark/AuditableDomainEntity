using AuditableDomainEntity.Interfaces;

namespace AuditableDomainEntity;

public abstract class AuditableAggregateRootEntity : AuditableDomainEntity
{
    public new Ulid EntityId => Id.Value;
    protected AuditableAggregateRootEntity(AggregateRootId aggregateRootId, List<IDomainEvent>? events)
        : base(aggregateRootId, events) { }
    
    protected AuditableAggregateRootEntity(AggregateRootId aggregateRootId)
        : base(aggregateRootId) { }
}