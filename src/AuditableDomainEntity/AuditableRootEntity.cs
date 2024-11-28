using AuditableDomainEntity.Interfaces;

namespace AuditableDomainEntity;

public abstract class AuditableRootEntity : AuditableEntityBase
{
    protected AuditableRootEntity(AggregateRootId aggregateRootId) : base(aggregateRootId, aggregateRootId.Value)
    {
        ValidateAggregateRootId(aggregateRootId);
    }

    public AuditableRootEntity()
    {
        
    }

    public void FinalizeChanges()
    {
        FinalizeChangesInternal(AggregateRootId);
        foreach (var entity in Children.Values)
        {
            entity?.FinalizeChanges(AggregateRootId);
        }
    }

    public static T? LoadFromHistory<T>(
        AggregateRootId aggregateRootId,
        List<IDomainEntityEvent>? events)
        where T : AuditableRootEntity, new()
    {
        var root = Activator.CreateInstance(typeof(T), aggregateRootId) as T
               ?? throw new InvalidOperationException($"Failed to generate new instance of type({typeof(T).Name}");
        
        root.LoadHistory(events);
        
        return root;
    }
}