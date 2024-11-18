using AuditableDomainEntity.Events.EntityEvents;
using AuditableDomainEntity.Interfaces;

namespace AuditableDomainEntity;

public abstract class AuditableEntity : AuditableEntityBase, IAuditableChildEntity
{
    private Ulid? ParentEntityId { get; set; }
    private Ulid? FieldId { get; set; }

    public AuditableEntity(Ulid entityid, List<IDomainEntityEvent> events) : base(entityid, events)
    {
        var entityCreatedEvent = events.First(x => x.EntityId == entityid && x is AuditableEntityCreated);
        ParentEntityId = entityCreatedEvent.ParentId;
        FieldId = entityCreatedEvent.FieldId;
    }

    public AuditableEntity() { }
    
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
    
    public static AuditableEntity? GenerateExistingEntity(
        Type entityType,
        Ulid entityId,
        List<IDomainEntityEvent> domainEntityEvents)
    {
        if (!entityType.IsSubclassOf(typeof(AuditableEntity)))
            throw new ArgumentException($"Entity type must be a subclass of AuditableEntityBase. Given: {entityType.Name}");
        
        dynamic entity = Activator.CreateInstance(entityType, entityId, domainEntityEvents)
                         ?? throw new InvalidOperationException($"Failed to create entity of type {entityType.Name}");
        return (AuditableEntity)entity;
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