using AuditableDomainEntity.Interfaces;

namespace AuditableDomainEntity;

public abstract class AuditableAggregateRootEntity : AuditableDomainEntity
{
    public new Ulid EntityId => Id.Value;
    private readonly List<IDomainEntityEvent> _events = new();
    protected AuditableAggregateRootEntity(AggregateRootId aggregateRootId, List<IDomainEntityEvent>? events)
        : base(aggregateRootId, events) { }
    
    protected AuditableAggregateRootEntity(AggregateRootId aggregateRootId)
        : base(aggregateRootId) { }
}