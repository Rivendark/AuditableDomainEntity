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
}