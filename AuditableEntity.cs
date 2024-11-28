using AuditableDomainEntity.Events.EntityEvents;
using AuditableDomainEntity.Interfaces;

namespace AuditableDomainEntity;

public abstract class AuditableEntity : AuditableEntityBase, IAuditableChildEntity
{ 
    public AuditableEntity() { }
    
    public AuditableEntity(AggregateRootId aggregateRootId, Ulid entityId) : base(aggregateRootId, entityId) { }
    
    public Ulid GetEntityId()
    {
        return EntityId;
    }

    public Ulid? GetParentEntityId()
    {
        return ParentEntityId;
    }

    public void SetParentEntityId(Ulid parentId)
    {
        ParentEntityId = parentId;
    }

    public void SetFieldId(Ulid? fieldId)
    {
        FieldId = fieldId;
    }

    public Ulid? GetFieldId()
    {
        return FieldId;
    }

    public void Attach(Ulid parent, string propertyName)
    {
        ParentEntityId = parent;
        InitializeNewProperties();
        IsInitialized = true;
    }

    public bool Initialized()
    {
        return IsInitialized;
    }

    public AggregateRootId GetAggregateRootId()
    {
        return AggregateRootId;
    }

    public static AuditableEntity? GenerateExistingEntity(
        Type entityType,
        AggregateRootId id,
        Ulid entityId,
        List<IDomainEntityEvent> domainEntityEvents)
    {
        if (!entityType.IsSubclassOf(typeof(AuditableEntity)))
            throw new ArgumentException($"Entity type must be a subclass of AuditableEntityBase. Given: {entityType.Name}");
        
        dynamic entity = Activator.CreateInstance(entityType, id, entityId)
                         ?? throw new InvalidOperationException($"Failed to create entity of type {entityType.Name}");

        if (entity is AuditableEntity auditableEntity)
        {
            auditableEntity.LoadHistory(domainEntityEvents);
        }
        return (AuditableEntity)entity;
    }
    
    public void FinalizeChanges(AggregateRootId aggregateRootId)
    {
        FinalizeChangesInternal(aggregateRootId);
        foreach (var entity in Children.Values)
        {
            entity?.FinalizeChanges(aggregateRootId);
        }
    }
    
    protected override AuditableEntityCreated CreateAuditableEntityCreated(
        AggregateRootId aggregateRootId,
        List<IDomainValueFieldEvent> valueFieldEvents,
        List<IDomainEntityFieldEvent> entityFieldEvents)
    {
        return new AuditableEntityCreated(
            aggregateRootId,
            Ulid.NewUlid(),
            EntityId,
            EntityType,
            FieldId,
            ParentEntityId,
            ++Version,
            valueFieldEvents,
            entityFieldEvents,
            DateTimeOffset.UtcNow);
    }

    protected override AuditableEntityUpdated CreateAuditableEntityUpdated(
        AggregateRootId aggregateRootId,
        List<IDomainValueFieldEvent> valueFieldEvents,
        List<IDomainEntityFieldEvent> entityFieldEvents)
    {
        return new AuditableEntityUpdated(
            aggregateRootId,
            Ulid.NewUlid(),
            EntityId,
            FieldId,
            ParentEntityId,
            ++Version,
            valueFieldEvents,
            entityFieldEvents,
            DateTimeOffset.UtcNow);
    }
}