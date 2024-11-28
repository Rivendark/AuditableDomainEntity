using AuditableDomainEntity.Events.EntityEvents;
using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Interfaces.Fields;
using AuditableDomainEntity.Interfaces.Fields.EntityFields;
using AuditableDomainEntity.Interfaces.Fields.ValueFields;

namespace AuditableDomainEntity;

public abstract partial class AuditableEntityBase
{
    public List<IDomainEntityEvent> GetEntityChanges()
    {
        var events = new List<IDomainEntityEvent>();
        foreach (var entityEvents in _entityChanges.Values)
        {
            events.AddRange(entityEvents);
        }

        foreach (var entity in Children.Values.OfType<AuditableEntity>())
        {
            events.AddRange(entity.GetEntityChanges());
        }

        return events;
    }
    
    #region Finalize

    protected virtual AuditableEntityCreated CreateAuditableEntityCreated(
        AggregateRootId aggregateRootId,
        List<IDomainValueFieldEvent> valueFieldEvents,
        List<IDomainEntityFieldEvent> entityFieldEvents)
    {
        var created = new AuditableEntityCreated(
            AggregateRootId,
            Ulid.NewUlid(),
            EntityId,
            EntityType,
            null,
            null,
            ++Version,
            valueFieldEvents,
            entityFieldEvents,
            DateTimeOffset.UtcNow);
        
        return created;
    }

    protected virtual AuditableEntityUpdated CreateAuditableEntityUpdated(
        AggregateRootId aggregateRootId,
        List<IDomainValueFieldEvent> valueFieldEvents,
        List<IDomainEntityFieldEvent> entityFieldEvents)
    {
        var updated = new AuditableEntityUpdated(
            AggregateRootId,
            Ulid.NewUlid(),
            EntityId,
            null,
            null,
            ++Version,
            valueFieldEvents,
            entityFieldEvents,
            DateTimeOffset.UtcNow);

        return updated;
    }

    protected void FinalizeChangesInternal(AggregateRootId aggregateRootId)
    {
        if (!IsInitialized) throw new InvalidOperationException("Cannot save an entity before the initialization is called.");
        var entityFieldChanges = GetEntityFieldChanges();
        var valueFieldChanges = GetValueFieldChanges();
        if (_entityEvents.Count == 0)
        {
            if (!_entityChanges.ContainsKey(aggregateRootId.Value))
            {
                _entityChanges.TryAdd(EntityId, [
                    CreateAuditableEntityCreated(aggregateRootId, valueFieldChanges, entityFieldChanges)
                ]);
                
                CommitFieldChanges();
                return;
            }
        }

        if (entityFieldChanges.Count == 0 && valueFieldChanges.Count == 0) return;

        if (_isDirty)
        {
            _entityChanges.TryAdd(EntityId, []);
            _entityChanges[EntityId].Add(CreateAuditableEntityUpdated(aggregateRootId, valueFieldChanges, entityFieldChanges));
        }
            
        CommitFieldChanges();
    }
    
    private List<IDomainValueFieldEvent> GetValueFieldChanges()
    {
        var events = new List<IDomainValueFieldEvent>();
        foreach (var fieldEvents in _valueFields.Values
                     .Select(fieldChanges => fieldChanges.GetChanges())
                     .ToList()
                )
        {
            events.AddRange(fieldEvents.OfType<IDomainValueFieldEvent>());
        }

        return events;
    }

    private List<IDomainEntityFieldEvent> GetEntityFieldChanges()
    {
        var events = new List<IDomainEntityFieldEvent>();
        foreach (var fieldEvents in _entityFields.Values
                     .Select(fieldChanges => fieldChanges.GetChanges()))
        {
            events.AddRange(fieldEvents.OfType<IDomainEntityFieldEvent>());
        }

        return events;
    }

    #endregion

    #region Commit

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

    #endregion
}