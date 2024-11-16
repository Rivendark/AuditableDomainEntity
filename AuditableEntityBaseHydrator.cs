using AuditableDomainEntity.Events.EntityEvents;
using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Interfaces.Attributes;
using System.Reflection;

namespace AuditableDomainEntity;

public abstract partial class AuditableEntityBase
{
    private readonly Dictionary<Ulid, List<IDomainEntityEvent>> _entityEvents = new();
    private readonly Dictionary<Ulid, List<IDomainEntityFieldEvent>> _entityFieldEvents = new();
    private readonly Dictionary<Ulid, List<IDomainValueFieldEvent>> _valueFieldEvents = new();
    private readonly Dictionary<Ulid, List<IDomainEntityEvent>> _entityChanges = new();
    private readonly Dictionary<Ulid, List<IDomainEntityEvent>> _events = new();
    
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
        
        LoadHistory(events);
        LoadPropertyHistory();
        _isInitialized = true;
    }

    public void FinalizeChanges(AggregateRootId aggregateRootId)
    {
        FinalizeChangesInternal(aggregateRootId);
    }

    private void FinalizeChangesInternal(AggregateRootId aggregateRootId)
    {
        if (!_isInitialized) throw new InvalidOperationException("Cannot save an entity before the initialization is called.");
        var entityFieldChanges = GetEntityFieldChanges();
        var valueFieldChanges = GetValueFieldChanges();
        if (_entityEvents.Count == 0)
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
                        valueFieldChanges,
                        entityFieldChanges,
                        DateTimeOffset.UtcNow)
                ]);
                
                CommitFieldChanges();
                return;
            }
        }

        if (entityFieldChanges.Count == 0 && valueFieldChanges.Count == 0) return;
        
        _entityChanges.TryAdd(EntityId, []);
        _entityChanges[EntityId].Add(
            new AuditableEntityUpdated(
                aggregateRootId,
                Ulid.NewUlid(),
                EntityId,
                null,
                null,
                ++_version,
                valueFieldChanges,
                entityFieldChanges,
                DateTimeOffset.UtcNow)
        );
            
        CommitFieldChanges();
    }
    
    private void CommitFieldChanges()
    {
        foreach (var field in _entityFields.Values)
        {
            field.CommitChanges();
        }

        foreach (var field in _valueFields.Values)
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
                _events.Add(entityId, currentEvents = []);
            }

            if (EntityId == entityId)
            {
                if (!_entityEvents.TryGetValue(entityId, out var currentEventsByEvent))
                {
                    _entityEvents.Add(entityId, currentEventsByEvent = []);
                }
                currentEventsByEvent.AddRange(events);
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
            if (CheckHasAttribute(property, typeof(IAuditableValueFieldAttribute)))
            {
                LoadValueFieldHistory(property);
                continue;
            }
        }
    }

    private void LoadValueFieldHistory(PropertyInfo property)
    {
        if (_propertyIds.TryGetValue(property.Name, out var fieldId))
        {
            if (_valueFieldEvents.TryGetValue(fieldId, out var domainEvents))
            {
                var valueFieldWithHistory = AuditableFieldRoot.GenerateExistingValueField(
                    typeof(AuditableValueField<>),
                    domainEvents,
                    property);
                _valueFields.TryAdd(valueFieldWithHistory.FieldId, valueFieldWithHistory);
                return;
            }
        }
        
        var valueFieldNew = AuditableFieldRoot.GenerateNewField(
            typeof(AuditableValueField<>),
            EntityId,
            property);
        _propertyIds.Add(valueFieldNew.Name, valueFieldNew.FieldId);
        _valueFields.TryAdd(valueFieldNew.FieldId, valueFieldNew);
    }

    private void LoadEntityFieldHistory(PropertyInfo property)
    {
        if (_propertyIds.TryGetValue(property.Name, out var fieldId))
        {
            if (_entityFieldEvents.TryGetValue(fieldId, out var domainEvents))
            {
                var entityFieldWithHistory = AuditableFieldRoot.GenerateExistingEntityField(
                    typeof(AuditableEntityField<>),
                    domainEvents,
                    property.PropertyType);
                _entityFields.TryAdd(entityFieldWithHistory.FieldId, entityFieldWithHistory);
                return;
            }
        }
        
        var entityFieldNew = AuditableFieldRoot.GenerateNewField(
            typeof(AuditableEntityField<>),
            EntityId,
            property);
        _propertyIds.Add(entityFieldNew.Name, entityFieldNew.FieldId);
        _entityFields.TryAdd(entityFieldNew.FieldId, entityFieldNew);
    }

    private static bool CheckHasAttribute(PropertyInfo property, Type attributeInterface)
    {
        var hasAttribute = new Dictionary<Type, bool>();
        var hasValueFieldAttribute = property.GetCustomAttributes()
            .Any(a => a is IAuditableValueFieldAttribute);
        hasAttribute.Add(typeof(IAuditableValueFieldAttribute), hasValueFieldAttribute);
        
        var hasEntityFieldAttribute = property.GetCustomAttributes()
            .Any(a => a is IAuditableEntityFieldAttribute);
        hasAttribute.Add(typeof(IAuditableEntityFieldAttribute), hasEntityFieldAttribute);
        
        if (hasAttribute.Count(a => a.Value) > 1)
            throw new InvalidOperationException($"Property {property.Name} has more than one AuditableFieldType attribute.");
        
        return hasAttribute[attributeInterface];
    }

    private void LoadEntityHistory(IDomainEntityEvent domainEvent)
    {
        if (!_entityEvents.TryGetValue(domainEvent.EventId, out var value))
        {
            value = [];
            _entityEvents.Add(domainEvent.EventId, value);
        }

        value.Add(domainEvent);
        if (domainEvent.ValueFieldEvents != null)
        {
            foreach (var fieldEvent in domainEvent.ValueFieldEvents)
            {
                if (!_valueFieldEvents.ContainsKey(fieldEvent.FieldId))
                    _valueFieldEvents.TryAdd(fieldEvent.FieldId, []);

                _valueFieldEvents[fieldEvent.FieldId].Add(fieldEvent);
                _propertyIds.TryAdd(fieldEvent.FieldName, fieldEvent.FieldId);
            }
        }

        if (domainEvent.EntityFieldEvents != null)
        {
            foreach (var fieldEvent in domainEvent.EntityFieldEvents)
            {
                if (!_entityFieldEvents.ContainsKey(fieldEvent.FieldId))
                    _entityFieldEvents.TryAdd(fieldEvent.FieldId, []);

                _entityFieldEvents[fieldEvent.FieldId].Add(fieldEvent);
                _propertyIds.TryAdd(fieldEvent.FieldName, fieldEvent.FieldId);
            }
        }
    }

    private void LoadHistory(List<IDomainEntityEvent>? events)
    {
        if (events == null || events.Count == 0) return;
        
        foreach (var domainEvent in events.OrderBy(e => e.EventVersion))
        {
            // Handle Root Entity Events
            if (domainEvent.EntityId == EntityId)
            {
                LoadEntityHistory(domainEvent);
                continue;
            }

            // Handle Child Entity Events
            if (domainEvent.FieldId is not null
                && domainEvent.ParentId is not null
                && domainEvent.ParentId == EntityId)
            { 
                // Create entity
                if (domainEvent is AuditableEntityCreated auditableEntityCreated)
                {
                    if (!_children.ContainsKey(auditableEntityCreated.EntityId))
                    {
                        if (!_entityFields.TryGetValue(domainEvent.FieldId.Value, out var childEntityField))
                            throw new InvalidOperationException($"Property {domainEvent.FieldId.Value} has no child entity field.");
                
                        if (childEntityField.FieldType is null)
                            throw new InvalidOperationException($"Property {domainEvent.FieldId.Value} has no field type defined.");
                
                        var childEntity = AuditableEntity.GenerateExistingEntity(childEntityField.FieldType, events);
                        _children.TryAdd(childEntity.EntityId, childEntity);
                    }
                }
            }
        }
    }
}