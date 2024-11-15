using AuditableDomainEntity.Interfaces;

namespace AuditableDomainEntity;

public abstract class AuditableAggregateRootEntity : AuditableEntityBase
{
    public AggregateRootId Id { get; init; }

    protected AuditableAggregateRootEntity(AggregateRootId aggregateRootId, List<IDomainEntityEvent>? events)
        : base(aggregateRootId.Value, events)
    {
        ValidateAggregateRootId(aggregateRootId);
        Id = aggregateRootId;
    }

    protected AuditableAggregateRootEntity(AggregateRootId aggregateRootId) : base(aggregateRootId.Value)
    {
        ValidateAggregateRootId(aggregateRootId);
        Id = aggregateRootId;
    }

    public void Save()
    {
        Save(Id);
    }
}