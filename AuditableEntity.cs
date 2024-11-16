using AuditableDomainEntity.Interfaces;

namespace AuditableDomainEntity;

public abstract class AuditableEntity : AuditableEntityBase
{
    public Ulid ParentEntityId { get; init; }
    public required string PropertyName { get; init; }

    public AuditableEntity(Ulid entityId, Ulid parentEntityId, string propertyName) : base(entityId)
    {
        ParentEntityId = parentEntityId;
        PropertyName = propertyName;
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