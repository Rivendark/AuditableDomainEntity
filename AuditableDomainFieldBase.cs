using AuditableDomainEntity.Events.FieldEvents;
using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Types;

namespace AuditableDomainEntity;

public abstract class AuditableDomainFieldRoot;

public abstract class AuditableDomainFieldBase<T> : AuditableDomainFieldRoot
{
    private readonly List<IDomainEvent> _changes = [];
    public Ulid FieldId { get; private set; }
    public Ulid EntityId { get; private set; }
    public AuditableDomainFieldType Type { get; init; }
    public AuditableDomainFieldStatus Status { get; private set; } = AuditableDomainFieldStatus.Created;
    public string Name { get; init; }
    private int _version = 0;
    private T? _value;
    public T? FieldValue
    {
        get => _value;
        set => ApplyValue(value);
    }
    public Type? FieldType { get; init; }

    protected AuditableDomainFieldBase(string name, AuditableDomainFieldType type)
    {
        FieldId = Ulid.NewUlid();
        FieldType = typeof(T);
        Name = name;
        Type = type;
    }
    
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
        AuditableDomainFieldType type,
        T? value)
    {
        FieldType = typeof(T);
        FieldId = fieldId;
        EntityId = entityId;
        Name = name;
        Type = type;
        FieldValue = value;
    }

    protected AuditableDomainFieldBase(AuditableDomainFieldType type, List<IDomainFieldEvent> domainEvents)
    {
        var initializedEvent = domainEvents.FirstOrDefault(x => x is AuditableFieldInitialized<T>);
        
        if (initializedEvent is null)
            throw new ArgumentException($"Failed to find auditable domain field initialized event for type {type}");
        
        var iEvent = initializedEvent as AuditableFieldInitialized<T>;

        FieldId = iEvent!.FieldId;
        FieldType = typeof(T);
        EntityId = iEvent.FieldId;
        Name = iEvent.FieldName;
        Type = type;
        
        Hydrate(domainEvents);
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
                EntityId = auditableFieldInitialized.EntityId;
                FieldId = auditableFieldInitialized.FieldId;
                _value = auditableFieldInitialized.InitialValue;
                break;
            
            case AuditableFieldUpdated<T> auditableFieldUpdated:
                _value = auditableFieldUpdated.NewValue;
                break;
        }
    }

    public void Initialize(Ulid id, Ulid entityId)
    {
        FieldId = id;
        EntityId = entityId;
        Status = AuditableDomainFieldStatus.Initialized;
    }

    public void Initialize(Ulid id, Ulid entityId, List<IDomainEvent> domainEvents)
    {
        FieldId = id;
        EntityId = entityId;
        _changes.AddRange(domainEvents);
        Status = AuditableDomainFieldStatus.Initialized;
    }

    public bool IsInitialized()
    {
        return Status != AuditableDomainFieldStatus.Created;
    }
    
    public bool HasChanges() => _changes.Count > 0;
    
    public List<IDomainEvent> GetChanges() => _changes;

    private void ApplyValue(T? value)
    {
        if (_value is null && value is null) return;
        if (_value is null && value is not null)
        {
            _changes.Add(new AuditableFieldInitialized<T>(
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
        
        _changes.Add(new AuditableFieldUpdated<T>(
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
