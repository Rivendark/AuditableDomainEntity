using AuditableDomainEntity.Events.EntityEvents;
using AuditableDomainEntity.Interfaces;
using System.Reflection;

namespace AuditableDomainEntity;

public abstract partial class AuditableEntityBase
{
    private readonly Dictionary<Ulid, List<IDomainEntityEvent>> _entityEvents = new();
    private readonly Dictionary<Ulid, List<IDomainEntityRegistryEvent>> _entityFieldRegistryEvents = new();
    private readonly Dictionary<Ulid, List<IDomainEntityEvent>> _entityFieldEvents = new();
    private readonly Dictionary<Ulid, List<IDomainFieldEvent>> _fieldEvents = new();
    private readonly Dictionary<Ulid, List<IDomainEntityEvent>> _entityChanges = new();
    private readonly Dictionary<Ulid, List<IDomainEntityEvent>> _events = new();
    private readonly bool _isNewEntity = true;
    
    protected AuditableEntityBase(Ulid entityId, List<IDomainEntityEvent>? events)
    {
        if (events == null || events.Count == 0) return;
        
        var initializedEvent = events
            .FirstOrDefault(e => e is AuditableEntityCreated created 
                                 && created.EntityId == entityId);
        
        if (initializedEvent is null) throw new InvalidOperationException(
            $"Cannot load an entity history for {entityId}. AuditableEntityCreated event not found.");
        
        EntityId = initializedEvent.EntityId;
        
        ApplyEntityEvent(initializedEvent);
        
        LoadEntityHistory(EntityId, events);
        LoadPropertyHistory();
        _isInitialized = true;
        _isNewEntity = false;
    }

    public void Save(AggregateRootId aggregateRootId)
    {
        SaveInternal(aggregateRootId);
    }

    private void SaveInternal(AggregateRootId aggregateRootId)
    {
        if (!_isInitialized) throw new InvalidOperationException("Cannot save an entity before the initialization is called.");
        var entityChanges = GetEntityChanges();
        var fieldChanges = GetFieldChanges();
        if (_isNewEntity)
        {
            if (!_entityChanges.ContainsKey(aggregateRootId.Value))
            {
                _entityChanges.TryAdd(EntityId, [
                    new AuditableEntityCreated(
                        aggregateRootId,
                        Ulid.NewUlid(),
                        EntityId,
                        null,
                        null,
                        ++_version,
                        fieldChanges,
                        entityChanges,
                        DateTimeOffset.UtcNow)
                ]);
                
                CommitFieldChanges();
                return;
            }
        }

        if (entityChanges.Count == 0 && fieldChanges.Count == 0) return;
        
        _entityChanges.TryAdd(EntityId, []);
        _entityChanges[EntityId].Add(
            new AuditableEntityUpdated(
                aggregateRootId,
                Ulid.NewUlid(),
                EntityId,
                null,
                null,
                ++_version,
                fieldChanges,
                entityChanges,
                DateTimeOffset.UtcNow)
        );
            
        CommitFieldChanges();
    }
    
    private void CommitFieldChanges()
    {
        foreach (var field in _auditableEntityFields.Values)
        {
            field.CommitChanges();
        }
    }

    public void Commit()
    {
        foreach (var (entityId, events) in _entityChanges)
        {
            if (!_events.TryGetValue(entityId, out var currentEvents))
            {
                _events.Add(entityId, currentEvents = new List<IDomainEntityEvent>());
            }
            
            currentEvents.AddRange(events);
        }
        
        _entityChanges.Clear();
    }

    private void LoadPropertyHistory()
    {
        var properties = GetType().GetProperties();
        foreach (var property in properties)
        {
            var attributes = property.GetCustomAttributes();
            if (!attributes.Any(a => a is IAuditableFieldAttribute)) continue;
            
            if (_propertyIds.TryGetValue(property.Name, out var fieldId))
            {
                if (_fieldEvents.TryGetValue(fieldId, out var domainEvents))
                {
                    var contextType = typeof(AuditableDomainValueField<>).MakeGenericType(property.PropertyType);
                    dynamic auditableDomainField = Activator.CreateInstance(contextType, domainEvents)!;
                    _auditableEntityFields.TryAdd(auditableDomainField.FieldId, auditableDomainField);
                }
            }
        }
    }

    private void LoadEntityHistory(Ulid entityId, List<IDomainEntityEvent>? events)
    {
        if (events == null || events.Count == 0) return;
        
        foreach (var domainEvent in events.OrderBy(e => e.EventVersion))
        {
            // Handle Root Entity Events
            if (domainEvent.EntityId == EntityId)
            {
                if (!_entityEvents.TryGetValue(domainEvent.EventId, out var value))
                {
                    value = [];
                    _entityEvents.Add(domainEvent.EventId, value);
                }

                value.Add(domainEvent);
                if (domainEvent.FieldEvents != null)
                {
                    foreach (var fieldEvent in domainEvent.FieldEvents)
                    {
                        if (!_fieldEvents.ContainsKey(fieldEvent.FieldId))
                            _fieldEvents.TryAdd(fieldEvent.FieldId, []);

                        _fieldEvents[fieldEvent.FieldId].Add(fieldEvent);
                        _propertyIds.TryAdd(fieldEvent.FieldName, fieldEvent.FieldId);
                    }
                }
                
                continue;
            }

            // Handle Entity Field Events
            if (domainEvent.FieldId is not null
                && domainEvent.ParentId is not null
                && domainEvent.ParentId == EntityId)
            {
                if (!_entityFieldEvents.ContainsKey(domainEvent.EntityId))
                    _entityFieldEvents.TryAdd(domainEvent.EntityId, []);
                    
                _entityFieldEvents[domainEvent.EntityId].Add(domainEvent);

                foreach (var fieldEvent in domainEvent.FieldEvents)
                {
                    if (!_fieldEvents.ContainsKey(fieldEvent.FieldId))
                        _fieldEvents.TryAdd(fieldEvent.FieldId, []);
                    
                    _fieldEvents[fieldEvent.FieldId].Add(fieldEvent);
                    _propertyIds.TryAdd(fieldEvent.FieldName, fieldEvent.FieldId);
                    break;
                }
            }
        }
    }
}