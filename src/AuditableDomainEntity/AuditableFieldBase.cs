using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Types;

namespace AuditableDomainEntity;

public abstract class AuditableFieldBase : AuditableFieldRoot
{
    protected AuditableDomainFieldType Type { get; init; }
    protected AuditableDomainFieldStatus Status { get; set; } = AuditableDomainFieldStatus.Created;
    protected int Version = 0;

    protected AuditableFieldBase(
        Ulid entityId,
        string name,
        AuditableDomainFieldType type)
        : base(Ulid.NewUlid(), entityId, name)
    {
        FieldId = Ulid.NewUlid();
        EntityId = entityId;
        Name = name;
        Type = type;
    }
    
    protected AuditableFieldBase(
        Ulid fieldId,
        Ulid entityId,
        string name,
        AuditableDomainFieldType type)
        : base(fieldId, entityId, name)
    {
        Type = type;
    }

    protected AuditableFieldBase()
    {
        
    }

    protected void Hydrate(IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents.OrderBy(x => x.EventVersion))
        {
            Hydrate(domainEvent);
        }
    }

    protected abstract void Hydrate(IDomainEvent domainEvent);
}
