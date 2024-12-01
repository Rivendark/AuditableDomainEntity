using AuditableDomainEntity.Events.EntityEvents;
using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Interfaces.Attributes;
using AuditableDomainEntity.Interfaces.Fields.EntityFields;
using AuditableDomainEntity.Interfaces.Fields.ValueFields;
using System.Reflection;

namespace AuditableDomainEntity;

public abstract partial class AuditableEntityBase
{
    private readonly Dictionary<Ulid, List<IDomainEntityEvent>> _entityEvents = new();
    private readonly Dictionary<Ulid, List<IDomainEntityFieldEvent>> _entityFieldEvents = new();
    private readonly Dictionary<Ulid, List<IDomainValueFieldEvent>> _valueFieldEvents = new();
    private readonly Dictionary<Ulid, List<IDomainEntityEvent>> _entityChanges = new();
    private readonly Dictionary<Ulid, List<IDomainEntityEvent>> _events = new();
    private readonly List<IDomainEntityEvent> _aggregateEventHistory = new();

    #region LoadHistory

    protected void LoadHistory(List<IDomainEntityEvent>? events)
    {
        if (events == null || events.Count == 0) return;
        
        _aggregateEventHistory.Clear();
        _aggregateEventHistory.AddRange(events);
        
        _propertyIds.Clear();
        _entityFields.Clear();
        _valueFields.Clear();
        
        _entityChanges.Clear();
        _entityEvents.Clear();
        _entityFieldEvents.Clear();
        _valueFieldEvents.Clear();
        
        _events.Clear();
        
        var initializedEvent = _aggregateEventHistory
            .FirstOrDefault(e => e is AuditableEntityCreated created 
                                 && created.EntityId == EntityId);
        
        if (initializedEvent is null) throw new InvalidOperationException(
            $"Cannot load an entity history for {EntityId}. AuditableEntityCreated event not found.");
        
        ApplyEntityEvent(initializedEvent);
        
        foreach (var domainEvent in events.OrderBy(e => e.EventVersion))
        {
            // Handle Root Entity Events
            if (domainEvent.EventId == initializedEvent.EventId) continue;
            
            if (domainEvent.EntityId == EntityId)
            {
                ApplyEntityEvent(domainEvent);
                continue;
            }

            // Handle Child Entity Events
            if (domainEvent.FieldId is null
                || domainEvent.ParentId is null
                || domainEvent.ParentId != EntityId) continue;
            
            if (domainEvent is not AuditableEntityCreated auditableEntityCreated) continue;
            if (Children.ContainsKey(auditableEntityCreated.EntityId)) continue;
                
            // Create entity
            var childEntity = AuditableEntity.GenerateExistingEntity(auditableEntityCreated.EntityType, AggregateRootId, domainEvent.EntityId, events);
            if (childEntity == null)
                throw new InvalidOperationException(
                    $"Failed to generate child entity. EntityId: {domainEvent.EntityId}. " +
                    $"Type: {auditableEntityCreated.EntityType.Name}");
            
            Children.TryAdd(childEntity.EntityId, childEntity);
        }
        
        LoadPropertyHistory();
        IsInitialized = true;
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
            foreach (var valueEvent in domainEvent.ValueFieldEvents)
            {
                if (!_valueFieldEvents.ContainsKey(valueEvent.FieldId))
                    _valueFieldEvents.TryAdd(valueEvent.FieldId, []);

                _valueFieldEvents[valueEvent.FieldId].Add(valueEvent);
                _propertyIds.TryAdd(valueEvent.FieldName, valueEvent.FieldId);
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
    
    private void LoadPropertyHistory()
    {
        var properties = EntityType.GetProperties();
        foreach (var property in properties)
        {
            if (CheckHasAttribute(property, typeof(IAuditableValueFieldAttribute)))
            {
                LoadValueFieldHistory(property);
                continue;
            }

            if (CheckHasAttribute(property, typeof(IAuditableEntityFieldAttribute)))
            {
                LoadEntityFieldHistory(property);
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
                var valueFieldWithHistory = AuditableFieldBase.GenerateExistingValueField(
                    typeof(AuditableValueField<>),
                    domainEvents,
                    property);
                _valueFields.TryAdd(valueFieldWithHistory.FieldId, valueFieldWithHistory);
                return;
            }
        }
        
        var valueFieldNew = AuditableFieldBase.GenerateNewField(
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
                var entityFieldWithHistory = AuditableFieldBase.GenerateExistingEntityField(
                    typeof(AuditableEntityField<>),
                    domainEvents,
                    Children,
                    property);
                _entityFields.TryAdd(entityFieldWithHistory.FieldId, entityFieldWithHistory);
                return;
            }
        }
        
        var entityFieldNew = AuditableFieldBase.GenerateNewField(
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

    #endregion
}