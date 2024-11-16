using AuditableDomainEntity.Events.FieldEvents;
using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Types;

namespace AuditableDomainEntity;

public sealed class AuditableValueField<T> : AuditableFieldBase<T>
{
    public AuditableValueField(Ulid fieldId, Ulid entityId, string name)
        : base(fieldId, entityId, name, AuditableDomainFieldType.Value) { }

    public AuditableValueField(Ulid entityId, string name)
        : base(entityId, name, AuditableDomainFieldType.Value) { }
    
    public AuditableValueField(List<IDomainValueFieldEvent> events)
        : base(AuditableDomainFieldType.Value, events) { }
    
    protected override void ApplyValue(T? value)
    {
        if (Value is null && value is null) return;
        if (Value is null && value is not null)
        {
            AddDomainEvent(new AuditableValueFieldInitialized<T>(
                Ulid.NewUlid(),
                FieldId,
                EntityId,
                Name,
                ++Version,
                value,
                DateTimeOffset.UtcNow));
            Value = value;
            return;
        }
        
        if (Value is not null && Value.Equals(value)) return;
        
        AddDomainEvent(new AuditableValueFieldUpdated<T>(
            Ulid.NewUlid(),
            FieldId,
            EntityId,
            Name,
            ++Version,
            Value,
            value,
            DateTimeOffset.UtcNow
        ));
        
        Value = value;
    }
    
    public new List<IDomainValueFieldEvent> GetChanges() => base.GetChanges()
        .OfType<IDomainValueFieldEvent>()
        .ToList();
}