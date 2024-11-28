using AuditableDomainEntity.Events.ValueFieldEvents;
using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Types;

namespace AuditableDomainEntity;

public sealed class AuditableValueField<T> : AuditableFieldBase
{
    protected T? Value;
    public T? FieldValue
    {
        get => Value;
        set => ApplyValue(value);
    }

    public AuditableValueField(Ulid fieldId, Ulid entityId, string name)
        : base(fieldId, entityId, name, AuditableDomainFieldType.Value)
    {
        FieldType = typeof(T);
    }

    public AuditableValueField(Ulid entityId, string name)
        : base(entityId, name, AuditableDomainFieldType.Value)
    {
        FieldType = typeof(T);
    }
    
    public AuditableValueField(List<IDomainValueFieldEvent> domainEvents)
    {
        var initializedEvent = domainEvents.FirstOrDefault(x => x is AuditableValueFieldInitialized<T>);
        
        if (initializedEvent is null)
            throw new ArgumentException($"Failed to find auditable domain field initialized event for type {GetType().Name}");
        
        domainEvents.Remove(initializedEvent);
        
        var iEvent = initializedEvent as AuditableValueFieldInitialized<T>;

        FieldId = iEvent!.FieldId;
        FieldType = typeof(T);
        EntityId = iEvent.EntityId;
        Name = iEvent.FieldName;
        Type = AuditableDomainFieldType.Value;
        Value = iEvent.InitialValue;
        Version = iEvent.EventVersion;
        
        if (domainEvents.Any())
            Hydrate(domainEvents);

        Status = AuditableDomainFieldStatus.Initialized;
        
        SetEvents(domainEvents.Select(IDomainEvent (e) => e).ToList());
    }
    
    private void ApplyValue(T? value)
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
    
    protected override void Hydrate(IDomainEvent domainEvent)
    {
        switch (domainEvent.GetType())
        {
            case AuditableValueFieldInitialized<T> auditableFieldInitialized:
                FieldId = auditableFieldInitialized!.FieldId;
                EntityId = auditableFieldInitialized.EntityId;
                Value = auditableFieldInitialized.InitialValue;
                Version = auditableFieldInitialized.EventVersion;
                break;
            
            case AuditableValueFieldUpdated<T> auditableFieldUpdated:
                Value = auditableFieldUpdated.NewValue;
                Version = auditableFieldUpdated.EventVersion;
                break;
        }
    }
    
    public new List<IDomainValueFieldEvent> GetChanges() => base.GetChanges()
        .OfType<IDomainValueFieldEvent>()
        .ToList();
}