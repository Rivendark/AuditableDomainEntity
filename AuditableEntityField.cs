using AuditableDomainEntity.Events.EntityEvents;
using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Types;

namespace AuditableDomainEntity;

public sealed class AuditableEntityField<T> : AuditableFieldBase where T : IAuditableChildEntity?
{
    private T? _value;
    public T? FieldValue
    {
        get => _value;
        set => ApplyValue(value);
    }
    
    private readonly Dictionary<Ulid, T> _auditableEntities = new();
    public AuditableEntityField(Ulid entityId, string name)
        : base(entityId, name, AuditableDomainFieldType.Entity) { }

    public AuditableEntityField(Ulid fieldId, Ulid entityId, string name)
        : base(fieldId, entityId, name, AuditableDomainFieldType.Entity) { }
    
    public AuditableEntityField(
        List<IDomainEntityFieldEvent> domainEvents,
        Dictionary<Ulid, IAuditableChildEntity> auditableEntities)
    {
        var initializedEvent = domainEvents.OrderBy(e => e.EventVersion).FirstOrDefault(x => x is AuditableEntityAdded);
        
        if (initializedEvent is null)
            throw new ArgumentException($"Failed to find auditable domain field initialized event for type {GetType().Name}");
        
        domainEvents.Remove(initializedEvent);
        
        var iEvent = initializedEvent as AuditableEntityAdded;

        // Load Field Properties
        FieldId = iEvent!.FieldId;
        FieldType = typeof(T);
        EntityId = iEvent.EntityId;
        Name = iEvent.FieldName;
        Type = AuditableDomainFieldType.Value;
        Version = iEvent.EventVersion;
        
        foreach (var child in auditableEntities
                     .Where(e => e.Value.GetFieldId() == FieldId  && e.Value is T))
        {
            _auditableEntities.Add(child.Key, (T)child.Value);
        }
        
        // Try load field Entity
        _auditableEntities.TryGetValue(iEvent.EntityId, out var existingEntity);
        _value = existingEntity ?? default;
        
        if (domainEvents.Any())
            Hydrate(domainEvents);

        Status = AuditableDomainFieldStatus.Initialized;
        
        SetEvents(domainEvents.Select(IDomainEvent (e) => e).ToList());
    }

    private void ApplyValue(T? value)
    {
        if (_value is null && value is null) return;
        if (_value is null && value is not null)
        {
            if (value is IAuditableChildEntity)
            {
                value.SetParentEntityId(EntityId);
                value.SetFieldId(FieldId);
                AddDomainEvent(new AuditableEntityAdded(
                    Ulid.NewUlid(),
                    value.GetEntityId(),
                    FieldId,
                    Name,
                    EntityId,
                    Version,
                    DateTimeOffset.UtcNow
                    ));
                _value = value;
                return;
            }
            
            throw new InvalidCastException($"Cannot convert {value} to {typeof(T)}");
        }

        if (_value is not null && value is null)
        {
            if (value is IAuditableChildEntity)
            {
                AddDomainEvent(new AuditableEntityRemoved(
                Ulid.NewUlid(),
                value.GetEntityId(),
                FieldId,
                Name,
                EntityId,
                Version,
                DateTimeOffset.UtcNow));
                
                _value = value;
                return;
            }
            
            throw new InvalidCastException($"Cannot convert {value} to {typeof(T)}");
        }
    }
    
    public new List<IDomainEntityFieldEvent> GetChanges() => base.GetChanges()
        .OfType<IDomainEntityFieldEvent>()
        .ToList()!;

    protected override void Hydrate(IDomainEvent domainEvent)
    {
        
    }
}