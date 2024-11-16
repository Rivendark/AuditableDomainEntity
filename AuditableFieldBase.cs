using AuditableDomainEntity.Events.FieldEvents;
using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Types;

namespace AuditableDomainEntity;

public abstract class AuditableFieldBase<T> : AuditableFieldRoot
{
    public T? FieldValue
    {
        get => Value;
        set => ApplyValue(value);
    }
    
    protected AuditableDomainFieldType Type { get; init; }
    protected AuditableDomainFieldStatus Status { get; set; } = AuditableDomainFieldStatus.Created;
    protected int Version = 0;
    protected T? Value;

    protected AuditableFieldBase(
        Ulid entityId,
        string name,
        AuditableDomainFieldType type)
        : base(Ulid.NewUlid(), entityId, name)
    {
        FieldType = typeof(T);
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
        FieldType = typeof(T);
        Type = type;
    }

    protected AuditableFieldBase(AuditableDomainFieldType type, List<IDomainValueFieldEvent> domainEvents) : base(domainEvents)
    {
        var initializedEvent = domainEvents.FirstOrDefault(x => x is AuditableValueFieldInitialized<T>);
        
        if (initializedEvent is null)
            throw new ArgumentException($"Failed to find auditable domain field initialized event for type {type}");
        
        domainEvents.Remove(initializedEvent);
        
        var iEvent = initializedEvent as AuditableValueFieldInitialized<T>;

        FieldId = iEvent!.FieldId;
        FieldType = typeof(T);
        EntityId = iEvent.EntityId;
        Name = iEvent.FieldName;
        Type = type;
        Value = iEvent.InitialValue;
        Version = iEvent.EventVersion;
        
        if (domainEvents.Any())
            Hydrate(domainEvents);

        Status = AuditableDomainFieldStatus.Initialized;
    }

    private void Hydrate(IEnumerable<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents.OrderBy(x => x.EventVersion))
        {
            Hydrate(domainEvent);
        }
    }

    private void Hydrate(IDomainEvent domainEvent)
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
    
    protected abstract void ApplyValue(T value);
}
