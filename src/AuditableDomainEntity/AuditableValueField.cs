using AuditableDomainEntity.Events.ValueFieldEvents;
using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Interfaces.Fields.ValueFields;
using AuditableDomainEntity.Types;
using System.Reflection;

namespace AuditableDomainEntity;

public sealed class AuditableValueField<T> : AuditableFieldBase
{
    private T? _value;
    public T? FieldValue
    {
        get => _value;
        set => ApplyValue(value);
    }
    
    public AuditableValueField(Ulid fieldId, Ulid entityId, PropertyInfo property)
        : base(fieldId, entityId, property, AuditableDomainFieldType.Value)
    {
        FieldType = typeof(T);
    }

    public AuditableValueField(Ulid entityId, PropertyInfo property)
        : base(entityId, property, AuditableDomainFieldType.Value)
    {
        FieldType = typeof(T);
    }
    
    public AuditableValueField(List<IDomainValueFieldEvent> domainEvents, PropertyInfo property)
    {
        var initializedEvent = domainEvents.FirstOrDefault(x => x is AuditableValueFieldInitialized<T>);
        
        if (initializedEvent is null)
            throw new ArgumentException($"Failed to find auditable domain field initialized event for type {GetType().Name}");
        
        SetEvents(domainEvents.Select(IDomainEvent (e) => e).ToList());
        
        var iEvent = initializedEvent as AuditableValueFieldInitialized<T>;
        
        if (property.Name != iEvent!.FieldName)
            throw new ArgumentException($"Field name mismatch. Expected {property.Name} but got {iEvent.FieldName}");
        
        FieldType = typeof(T);
        Name = iEvent.FieldName;
        Type = AuditableDomainFieldType.Value;
        
        Hydrate();

        Status = AuditableDomainFieldStatus.Initialized;
    }
    
    private void ApplyValue(T? value)
    {
        if (!HasEvents())
        {
            if (value is not null)
            {
                AddDomainEvent(new AuditableValueFieldInitialized<T>(
                    Ulid.NewUlid(),
                    FieldId,
                    EntityId,
                    Name,
                    ++Version,
                    value,
                    DateTimeOffset.UtcNow));
                _value = value;
                return;
            }
            
            return;
        }
        
        if (_value is null && value is null) return;
        if (_value is not null && _value.Equals(value)) return;
       
        AddDomainEvent(new AuditableValueFieldUpdated<T>(
            Ulid.NewUlid(),
            FieldId,
            EntityId,
            Name,
            ++Version,
            _value,
            value,
            DateTimeOffset.UtcNow
        ));
        
        _value = value;
    }
    
    protected override void Hydrate(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case IAuditableValueFieldInitialized:
                var auditableFieldInitialized = domainEvent as AuditableValueFieldInitialized<T>;
                FieldId = auditableFieldInitialized!.FieldId;
                EntityId = auditableFieldInitialized.EntityId;
                _value = auditableFieldInitialized.InitialValue;
                Version = auditableFieldInitialized.EventVersion;
                break;
            
            case IAuditableValueFieldUpdated:
                var auditableFieldUpdated = domainEvent as AuditableValueFieldUpdated<T>;
                _value = auditableFieldUpdated!.NewValue;
                Version = auditableFieldUpdated.EventVersion;
                break;
        }
    }
}