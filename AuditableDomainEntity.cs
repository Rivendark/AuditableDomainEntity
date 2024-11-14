using AuditableDomainEntity.Interfaces;
using System.Reflection;

namespace AuditableDomainEntity;

public abstract class AuditableDomainEntity
{
    public AggregateRootId Id { get; private init; }
    public Ulid EntityId { get; init; } = Ulid.NewUlid();
    
    private readonly bool _isInitialized;
    private readonly Dictionary<Ulid, List<IDomainEntityEvent>> _entityChanges = new ();
    private readonly Dictionary<Ulid, List<IDomainEntityEvent>> _historicalDomainEntityEvents = new();
    private readonly Dictionary<Ulid, List<IDomainFieldEvent>> _historicalDomainFieldEvents = new();
    private readonly Dictionary<string, Ulid> _propertyIds = new();
    private readonly Dictionary<Ulid, AuditableDomainFieldRoot> _auditableEntityFields = new();

    protected AuditableDomainEntity(AggregateRootId aggregateRootId, List<IDomainEvent>? events)
    {
        // Temp
        Id = aggregateRootId;
        if (events == null || events.Count == 0) return;
        LoadEntityHistory(events);
        LoadPropertyHistory();
        _isInitialized = true;
    }

    protected AuditableDomainEntity(AggregateRootId aggregateRootId)
    {
        Id = aggregateRootId;
        LoadPropertyClean();
        _isInitialized = true;
    }
    
    public List<IDomainEvent>? GetDomainChanges()
    {
        var events = new List<IDomainEvent>();
        foreach (var fieldEvents in _auditableEntityFields.Values
                     .Select(fieldChanges => fieldChanges.GetChanges()))
        {
            events.AddRange(fieldEvents);
        }
        
        // TODO gather entity events into list as well

        return events;
    }

    protected void SetValue<T>(T value, string propertyName)
    {
        if (!_isInitialized) return;
        var properties = GetType().GetProperties();
        foreach (var property in properties)
        {
            if (property.Name != propertyName) continue;
            var auditableDomainField = GetAuditableDomainField<T>(property, EntityId);
            auditableDomainField.FieldValue = value;
                    
            return;
        }

        throw new InvalidOperationException($"Property {propertyName} is not found in type {GetType().Name}");
    }

    protected T? GetValue<T>(string propertyName)
    {
        var properties = GetType().GetProperties();
        foreach (var property in properties)
        {
            if (property.Name != propertyName) continue;
            return GetAuditableDomainField<T>(property, EntityId).FieldValue;
        }

        throw new InvalidOperationException($"Property {propertyName} is not found in type {GetType().Name}");
    }

    private AuditableDomainField<T> GetAuditableDomainField<T>(PropertyInfo property, Ulid entityId)
    {
        if (!_propertyIds.TryGetValue(property.Name, out var fieldId))
        {
            throw new InvalidOperationException($"Property {property.Name} is not found in type {GetType().Name}:{nameof(_propertyIds)}");
        }

        return (AuditableDomainField<T>)_auditableEntityFields[fieldId];
    }
    
    private void LoadEntityHistory(List<IDomainEvent>? events)
    {
        if (events == null || events.Count == 0) return;
        foreach (var domainEvent in events.OrderBy(e => e.EventVersion))
        {
            switch (domainEvent)
            {
                case IDomainEntityEvent domainEntityEvent:
                {
                    if (!_historicalDomainEntityEvents.ContainsKey(domainEntityEvent.EntityId))
                        _historicalDomainEntityEvents.TryAdd(domainEntityEvent.EntityId, []);
                    
                    _historicalDomainEntityEvents[domainEntityEvent.EntityId].Add(domainEntityEvent);
                    
                    // TODO apply event to entity
                    
                    break;
                }
                case IDomainFieldEvent domainFieldEvent:
                    if (!_historicalDomainFieldEvents.ContainsKey(domainFieldEvent.FieldId))
                        _historicalDomainFieldEvents.TryAdd(domainFieldEvent.FieldId, []);
                    
                    _historicalDomainFieldEvents[domainFieldEvent.FieldId].Add(domainFieldEvent);
                    _propertyIds.TryAdd(domainFieldEvent.FieldName, domainFieldEvent.FieldId);
                    break;
                default:
                    throw new InvalidOperationException($"Unknown event type {domainEvent.GetType()}");
            }
        }
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
                if (_historicalDomainFieldEvents.TryGetValue(fieldId, out var domainEvents))
                {
                    var contextType = typeof(AuditableDomainField<>).MakeGenericType(property.PropertyType);
                    dynamic auditableDomainField = Activator.CreateInstance(contextType, domainEvents)!;
                    _auditableEntityFields.TryAdd(auditableDomainField.FieldId, auditableDomainField);
                }
            };
        }
    }

    private void LoadPropertyClean()
    {
        var properties = GetType().GetProperties();
        foreach (var property in properties)
        {
            var attributes = property.GetCustomAttributes();
            if (!attributes.Any(a => a is IAuditableFieldAttribute)) continue;
            
            var contextType = typeof(AuditableDomainField<>).MakeGenericType(property.PropertyType);
            dynamic auditableDomainField = Activator.CreateInstance(contextType, EntityId, property.Name)!;
            _propertyIds.Add(auditableDomainField.Name, auditableDomainField.FieldId);
            _auditableEntityFields.TryAdd(auditableDomainField.FieldId, auditableDomainField);
        }
    }
}