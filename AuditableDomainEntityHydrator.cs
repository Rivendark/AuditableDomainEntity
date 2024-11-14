using AuditableDomainEntity.Events.EntityEvents;
using AuditableDomainEntity.Interfaces;

namespace AuditableDomainEntity;

public abstract partial class AuditableDomainEntity
{
    private readonly Dictionary<Ulid, List<IDomainEntityEvent>> _entityEvents = new();
    private readonly Dictionary<Ulid, List<IDomainEntityEvent>> _entityFieldRegistryEvents = new();
    private readonly Dictionary<Ulid, List<IDomainEntityEvent>> _entityFieldEvents = new();
    private readonly Dictionary<Ulid, List<IDomainFieldEvent>> _fieldEvents = new();
    
    protected AuditableDomainEntity(AggregateRootId aggregateRootId, List<IDomainEntityEvent>? events)
    {
        ValidateAggregateRootId(aggregateRootId);
        
        Id = aggregateRootId;
        if (events == null || events.Count == 0) return;
        LoadEntityHistory(events);
        LoadPropertyHistory();
        _isInitialized = true;
    }
    
    public List<IDomainEntityEvent> Save()
    {
        if (!_isInitialized) throw new InvalidOperationException("Cannot save an entity before the initialization is called.");
        if (_entityEvents.Count == 0)
        {
            // TODO manage entity state and make decision on state
            _entityChanges.TryAdd(EntityId, [
                new AuditableEntityCreated(
                    Id,
                    Ulid.NewUlid(),
                    EntityId,
                    null,
                    null,
                    ++_version,
                    GetFieldChanges(),
                    GetEntityChanges(),
                    DateTimeOffset.UtcNow)
            ]);
        }
        
        return GetEntityChanges();
    }
    
    private void LoadEntityHistory(List<IDomainEntityEvent>? events)
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
                    foreach (var fieldEvent in domainEvent.FieldEvents)
                    {
                        if (!_fieldEvents.ContainsKey(fieldEvent.FieldId))
                            _fieldEvents.TryAdd(fieldEvent.FieldId, []);

                        _fieldEvents[fieldEvent.FieldId].Add(fieldEvent);
                        _propertyIds.TryAdd(fieldEvent.FieldName, fieldEvent.FieldId);
                        break;
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