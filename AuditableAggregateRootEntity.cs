using AuditableDomainEntity.Interfaces;

namespace AuditableDomainEntity;

public abstract class AuditableAggregateRootEntity : AuditableEntityBase
{
    protected AuditableAggregateRootEntity(AggregateRootId aggregateRootId, List<IDomainEntityEvent>? events)
        : base(aggregateRootId, aggregateRootId.Value, events)
    {
        ValidateAggregateRootId(aggregateRootId);
    }

    protected AuditableAggregateRootEntity(AggregateRootId aggregateRootId) : base(aggregateRootId, aggregateRootId.Value)
    {
        ValidateAggregateRootId(aggregateRootId);
    }

    public void FinalizeChanges()
    {
        FinalizeChangesInternal(Id);
        foreach (var entity in Children.Values)
        {
            entity?.FinalizeChanges(Id);
        }
    }
}