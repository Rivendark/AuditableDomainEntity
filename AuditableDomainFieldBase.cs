using AuditableDomainEntity.Events.FieldEvents;
using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Types;

namespace AuditableDomainEntity;

public abstract class AuditableDomainFieldBase<T> : AuditableDomainFieldRoot
{
    public Ulid FieldId { get; set; }
    public Ulid EntityId { get; set; }
    private AuditableDomainFieldType Type { get; init; }
    private AuditableDomainFieldStatus Status { get; set; } = AuditableDomainFieldStatus.Created;
    public string Name { get; init; }
    private int _version = 0;
    private T? _value;
    public T? FieldValue
    {
        get => _value;
        set => ApplyValue(value);
    }
    public Type? FieldType { get; init; }

    protected AuditableDomainFieldBase(
        Ulid entityId,
        string name,
        AuditableDomainFieldType type)
    {
        FieldType = typeof(T);
        FieldId = Ulid.NewUlid();
        EntityId = entityId;
        Name = name;
        Type = type;
    }
    
    protected AuditableDomainFieldBase(
        Ulid fieldId,
        Ulid entityId,
        string name,
        AuditableDomainFieldType type)
    {
        FieldType = typeof(T);
        FieldId = fieldId;
        EntityId = entityId;
        Name = name;
        Type = type;
    }

    protected AuditableDomainFieldBase(AuditableDomainFieldType type, List<IDomainFieldEvent> domainEvents)
    {
        var initializedEvent = domainEvents.FirstOrDefault(x => x is AuditableFieldInitialized<T>);
        
        if (initializedEvent is null)
            throw new ArgumentException($"Failed to find auditable domain field initialized event for type {type}");
        domainEvents.Remove(initializedEvent);
        
        var iEvent = initializedEvent as AuditableFieldInitialized<T>;

        FieldId = iEvent!.FieldId;
        FieldType = typeof(T);
        EntityId = iEvent.EntityId;
        Name = iEvent.FieldName;
        Type = type;
        _value = iEvent.InitialValue;
        _version = iEvent.EventVersion;
        
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
            case AuditableFieldInitialized<T> auditableFieldInitialized:
                FieldId = auditableFieldInitialized!.FieldId;
                EntityId = auditableFieldInitialized.EntityId;
                _value = auditableFieldInitialized.InitialValue;
                _version = auditableFieldInitialized.EventVersion;
                break;
            
            case AuditableFieldUpdated<T> auditableFieldUpdated:
                _value = auditableFieldUpdated.NewValue;
                _version = auditableFieldUpdated.EventVersion;
                break;
        }
    }

    private void ApplyValue(T? value)
    {
        if (_value is null && value is null) return;
        if (_value is null && value is not null)
        {
            Changes.Add(new AuditableFieldInitialized<T>(
                Ulid.NewUlid(),
                FieldId,
                EntityId,
                Name,
                ++_version,
                value,
                DateTimeOffset.UtcNow));
            _value = value;
            return;
        }
        
        if (_value is not null && _value.Equals(value)) return;
        
        Changes.Add(new AuditableFieldUpdated<T>(
            Ulid.NewUlid(),
            FieldId,
            EntityId,
            Name,
            ++_version,
            _value,
            value,
            DateTimeOffset.UtcNow
        ));
        
        _value = value;
    }

    private void ApplyAuditableEntityValue(T? value)
    {
        if (_value is null && value is not null)
        {
            // Create Entity Added event
            _value = value;
            return;
        }

        if (_value is not null && value is null)
        {
            // Create Entity Removed event
            _value = value;
            return;
        }

        _value = value;
    }

    private void ApplyBaseValue(T? value)
    {
        if (_value is null && value is not null)
        {
            Changes.Add(new AuditableFieldInitialized<T>(
                Ulid.NewUlid(),
                FieldId,
                EntityId,
                Name,
                ++_version,
                value,
                DateTimeOffset.UtcNow));
            _value = value;
            return;
        }
        
        if (_value is not null && _value.Equals(value)) return;
        
        Changes.Add(new AuditableFieldUpdated<T>(
            Ulid.NewUlid(),
            FieldId,
            EntityId,
            Name,
            ++_version,
            _value,
            value,
            DateTimeOffset.UtcNow
        ));
        
        _value = value;
    }
}
