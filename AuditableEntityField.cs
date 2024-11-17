using AuditableDomainEntity.Events.EntityEvents;
using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Types;

namespace AuditableDomainEntity;

public sealed class AuditableEntityField<T> : AuditableFieldBase<T>
{
    public AuditableEntityField(Ulid entityId, string name)
        : base(entityId, name, AuditableDomainFieldType.Entity) { }

    public AuditableEntityField(Ulid fieldId, Ulid entityId, string name)
        : base(fieldId, entityId, name, AuditableDomainFieldType.Entity) { }

    public AuditableEntityField(List<IDomainValueFieldEvent> domainEvents)
        : base(AuditableDomainFieldType.Entity, domainEvents) { }

    protected override void ApplyValue(T? value)
    {
        if (Value is null && value is null) return;
        if (Value is null && value is not null)
        {
            if (value is AuditableEntity auditableNewValue)
            {
                AddDomainEvent(new AuditableEntityAdded(
                    Ulid.NewUlid(),
                    auditableNewValue.EntityId,
                    FieldId,
                    Name,
                    EntityId,
                    Version,
                    DateTimeOffset.UtcNow
                    ));
                Value = value;
                return;
            }
            
            throw new InvalidCastException($"Cannot convert {value} to {typeof(T)}");
        }

        if (Value is not null && value is null)
        {
            if (value is AuditableEntity auditableExistingValue)
            {
                AddDomainEvent(new AuditableEntityRemoved(
                Ulid.NewUlid(),
                auditableExistingValue.EntityId,
                FieldId,
                Name,
                EntityId,
                Version,
                DateTimeOffset.UtcNow));
                
                Value = value;
                return;
            }
            
            throw new InvalidCastException($"Cannot convert {value} to {typeof(T)}");
        }
    }
    
    public new List<IDomainEntityFieldEvent> GetChanges() => base.GetChanges()
        .OfType<IDomainEntityFieldEvent>()
        .ToList()!;
}