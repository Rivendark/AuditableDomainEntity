using AuditableDomainEntity.Interfaces;

namespace AuditableDomainEntity;

public abstract class AuditableEntity : AuditableEntityBase, IAuditableChildEntity
{
    public Ulid ParentEntityId { get; protected set; }
    public string PropertyName { get; protected set; }

    public AuditableEntity(Ulid entityId, Ulid parentEntityId, string propertyName) : base(entityId)
    {
        ParentEntityId = parentEntityId;
        PropertyName = propertyName;
    }

    public AuditableEntity()
    {
        
    }

    public void Attach(Ulid parent, string propertyName)
    {
        ParentEntityId = parent;
        PropertyName = propertyName;
        InitializeNewProperties();
        IsInitialized = true;
    }

    public bool Initialized()
    {
        return IsInitialized;
    }
    
    public static AuditableEntity GenerateExistingEntity(
        Type entityType,
        List<IDomainEntityEvent> domainEntityEvents)
    {
        if (!entityType.IsSubclassOf(typeof(AuditableEntity)))
            throw new ArgumentException($"Entity type must be a subclass of AuditableEntityBase. Given: {entityType.Name}");
        
        dynamic entity = Activator.CreateInstance(entityType, domainEntityEvents)
                         ?? throw new InvalidOperationException($"Failed to create entity of type {entityType.Name}");
        return (AuditableEntity)entity;
    }
}