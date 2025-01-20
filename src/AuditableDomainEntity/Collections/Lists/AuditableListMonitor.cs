using AuditableDomainEntity.Events.CollectionEvents.ListEvents;
using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Interfaces.Collections;

namespace AuditableDomainEntity.Collections.Lists;

public partial class AuditableList<T> : IAuditableCollection<T>
{
    private readonly List<IDomainEvent> _changes = [];
    private bool _isInitialized = true;
    private Ulid _fieldId = Ulid.Empty;
    private Ulid _entityId = Ulid.Empty;
    private string _fieldName = "unknown";
    private float _eventVersion;
    private readonly Dictionary<Type, Action<IDomainEvent>> _hydrators = new();
    
    internal AuditableList(Ulid fieldId, IEnumerable<IAuditableListDomainEvent> events)
    {
        var orderedEvents = events.OrderBy(e => e.EventVersion).ToArray();
        if (orderedEvents.Length == 0)
        {
            throw new ArgumentException("No events found");
        }
        
        AddHydrateMethods();
        _isInitialized = false;
        _fieldId = fieldId;

        var initializedEvent = orderedEvents[0] as AuditableListInitialized<T>;
        
        if (initializedEvent == null)
        {
            throw new ArgumentException("The first event must be an initialized event");
        }
        
        _entityId = initializedEvent.EntityId;
        _fieldName = initializedEvent.FieldName;
        _eventVersion = initializedEvent.EventVersion;
        _items = initializedEvent.Items.ToArray();
        _size = _items.Length;
        
        foreach (var domainEvent in orderedEvents.Skip(1))
        {
            Hydrate(domainEvent);
        }
        _isInitialized = true;
    }
    
    public bool HasChanges()
    {
        return _changes.Count > 0;
    }
    
    public List<IDomainEvent> GetChanges()
    {
        return _changes;
    }
    
    private void AddHydrateMethods()
    {
        _hydrators.Add(typeof(AuditableListInitialized<T>), e => Initialized((AuditableListInitialized<T>)e));
        _hydrators.Add(typeof(AuditableListItemAdded<T>), e => ItemAdded((AuditableListItemAdded<T>)e));
        _hydrators.Add(typeof(AuditableListRemoveAt<T>), e => ItemRemovedAt((AuditableListRemoveAt<T>)e));
        _hydrators.Add(typeof(AuditableListCleared<T>), e => Cleared((AuditableListCleared<T>)e));
        _hydrators.Add(typeof(AuditableListItemInserted<T>), e => ItemInserted((AuditableListItemInserted<T>)e));
        _hydrators.Add(typeof(AuditableListRangeInserted<T>), e => RangeInserted((AuditableListRangeInserted<T>)e));
        _hydrators.Add(typeof(AuditableListRangeRemoved<T>), e => RangeRemoved((AuditableListRangeRemoved<T>)e));
    }

    private void Hydrate(IDomainEvent domainEvent)
    {
        if (_hydrators.ContainsKey(domainEvent.GetType()))
        {
            _hydrators[domainEvent.GetType()](domainEvent);
        }
    }
    
    private void Initialized(AuditableListInitialized<T> domainEvent)
    {
        _items = domainEvent.Items.ToArray();
        _size = _items.Length;
        _eventVersion = domainEvent.EventVersion;
    }
    
    private void ItemAdded(AuditableListItemAdded<T> domainEvent)
    {
        Add(domainEvent.Item);
        _eventVersion = domainEvent.EventVersion;
    }
    
    private void ItemRemovedAt(AuditableListRemoveAt<T> domainEvent)
    {
        RemoveAt(domainEvent.Index);
        _eventVersion = domainEvent.EventVersion;
    }
    
    private void Cleared(AuditableListCleared<T> domainEvent)
    {
        Clear();
        _eventVersion = domainEvent.EventVersion;
    }
    
    private void ItemInserted(AuditableListItemInserted<T> domainEvent)
    {
        Insert(domainEvent.Index, domainEvent.Item);
        _eventVersion = domainEvent.EventVersion;
    }
    
    private void RangeInserted(AuditableListRangeInserted<T> domainEvent)
    {
        InsertRange(domainEvent.Index, domainEvent.Items);
        _eventVersion = domainEvent.EventVersion;
    }
    
    private void RangeRemoved(AuditableListRangeRemoved<T> domainEvent)
    {
        RemoveRange(domainEvent.Index, domainEvent.Count);
        _eventVersion = domainEvent.EventVersion;
    }
    
    private void AddDomainEvent(IDomainEvent domainEvent)
    {
        if (!_isInitialized)
        {
            return;
        }
        
        if (_isReadOnly)
        {
            throw new InvalidOperationException("This collection is read-only and detached from AuditableEntity. " +
                                                "Use the AuditableEntities Property to make changes.");
        }
        _changes.Add(domainEvent);
    }

    public void SetEntityValues(Ulid entityId, Ulid fieldId, string fieldName)
    {
        _entityId = entityId;
        _fieldId = fieldId;
        _fieldName = fieldName;
        
        if (_changes.Count == 0)
            return;

        if (!_changes
                .Select(e => e as IAuditableListDomainEvent)
                .Where(e => e is not null)
                .Any(e => e!.FieldId != fieldId || e.EntityId != entityId)) return;
        
        var changes = _changes.ToList();
        _changes.Clear();
        var isInitialized = _isInitialized;
        _isInitialized = false;
        
        Clear();
        
        _isInitialized = true;
        _eventVersion = 0;
        
        foreach (var changeEvent in changes)
        {
            Hydrate(changeEvent);
            if (changeEvent is AuditableListInitialized<T> initializedChangeEvent)
            {
                AddDomainEvent(new AuditableListInitialized<T>(
                    Ulid.NewUlid(),
                    _entityId,
                    _fieldId,
                    _fieldName,
                    ++_eventVersion,
                    initializedChangeEvent.Items,
                    DateTimeOffset.UtcNow));
            }
        }
        _isInitialized = isInitialized;
    }
}