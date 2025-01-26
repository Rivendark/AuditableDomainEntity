using AuditableDomainEntity.Events.CollectionEvents.ListEvents.EntityListEvents;
using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Interfaces.Collections;

namespace AuditableDomainEntity.Collections.Lists;

public sealed class AuditableEntityList<T> : AuditableList<T>, IAuditableEntityCollection where T : IAuditableChildEntity
{
    private Dictionary<Ulid, IAuditableChildEntity> _childEntities = [];
    
    public static implicit operator AuditableEntityList<T>(List<T> l) => new (l);
    
    public AuditableEntityList()
    {
        AddHydrateMethods();
        AddDomainEvent(new AuditableEntityListInitialized<T>(
            Ulid.NewUlid(),
            EntityId,
            FieldId,
            FieldName,
            ++EventVersion,
            [],
            DateTimeOffset.UtcNow));
    }
    
    public AuditableEntityList(int capacity) : base(capacity)
    {
        AddHydrateMethods();
        AddDomainEvent(new AuditableEntityListInitialized<T>(
            Ulid.NewUlid(),
            EntityId,
            FieldId,
            FieldName,
            ++EventVersion,
            [],
            DateTimeOffset.UtcNow));
    }
    
    public AuditableEntityList(IEnumerable<T> items) : base(items)
    {
        AddHydrateMethods();
        switch (items)
        {
            case ICollection<T> c:
                AddDomainEvent(new AuditableEntityListInitialized<T>(
                    Ulid.NewUlid(),
                    EntityId,
                    FieldId,
                    FieldName,
                    ++EventVersion,
                    c.ToArray(),
                    DateTimeOffset.UtcNow));
                break;
            default:
                AddDomainEvent(new AuditableEntityListInitialized<T>(
                    Ulid.NewUlid(),
                    EntityId,
                    FieldId,
                    FieldName,
                    ++EventVersion,
                    [],
                    DateTimeOffset.UtcNow));
                break;
        }
    }
    
    internal AuditableEntityList(
        Ulid fieldId,
        IList<IAuditableEntityListDomainEvent> events,
        Dictionary<Ulid,IAuditableChildEntity> childEntities) : base(fieldId, events)
    {
        _childEntities = childEntities;
        AddHydrateMethods();
        var orderedEvents = events.OrderBy(e => e.EventVersion).ToArray();
        if (orderedEvents.Length == 0)
        {
            throw new ArgumentException("No events found");
        }

        var initializedEvent = orderedEvents[0] as AuditableEntityListInitialized<T>;
        
        if (initializedEvent == null)
        {
            throw new ArgumentException("The first event must be an initialized event");
        }
        
        EntityId = initializedEvent.EntityId;
        FieldName = initializedEvent.FieldName;
        EventVersion = initializedEvent.EventVersion;
        Items = initializedEvent.Items.ToArray();
        Size = Items.Length;
        
        foreach (var domainEvent in orderedEvents.Skip(1))
        {
            Hydrate(domainEvent);
        }
        IsInitialized = true;
    }
    
    // This method is used to set the entity values for the list
    // This also ensures that any events triggered before being attached to the entity have the correct values
    // for EntityId, FieldId and FieldName
    public override void SetParentFieldValues(Ulid entityId, Ulid fieldId, string fieldName)
    {
        var changes = GetChanges();
        EntityId = entityId;
        FieldId = fieldId;
        FieldName = fieldName;
        
        if (changes.Count == 0)
            return;

        if (!changes
                .Select(e => e as IAuditableEntityListDomainEvent)
                .Where(e => e is not null)
                .Any(e => e!.FieldId != fieldId || e.EntityId != entityId)) return;
        
        var changeEvents = changes.ToList();
        changes.Clear();
        var isInitialized = IsInitialized;
        IsInitialized = false;
        
        Clear();
        
        IsInitialized = true;
        EventVersion = 0;
        
        foreach (var changeEvent in changeEvents)
        {
            Hydrate(changeEvent);
            if (changeEvent is AuditableEntityListInitialized<T> initializedChangeEvent)
            {
                AddDomainEvent(new AuditableEntityListInitialized<T>(
                    Ulid.NewUlid(),
                    EntityId,
                    FieldId,
                    FieldName,
                    ++EventVersion,
                    initializedChangeEvent.Items,
                    DateTimeOffset.UtcNow));
            }
        }
        IsInitialized = isInitialized;
    }

    protected override void AddHydrateMethods()
    {
        Hydrators.TryAdd(typeof(AuditableEntityListInitialized<T>), e => Initialized((AuditableEntityListInitialized<T>)e));
        Hydrators.TryAdd(typeof(AuditableEntityListItemAdded<T>), e => ItemAdded((AuditableEntityListItemAdded<T>)e));
        Hydrators.TryAdd(typeof(AuditableEntityListRemoveAt<T>), e => ItemRemovedAt((AuditableEntityListRemoveAt<T>)e));
        Hydrators.TryAdd(typeof(AuditableEntityListCleared<T>), e => Cleared((AuditableEntityListCleared<T>)e));
        Hydrators.TryAdd(typeof(AuditableEntityListItemInserted<T>), e => ItemInserted((AuditableEntityListItemInserted<T>)e));
        Hydrators.TryAdd(typeof(AuditableEntityListRangeInserted<T>), e => RangeInserted((AuditableEntityListRangeInserted<T>)e));
        Hydrators.TryAdd(typeof(AuditableEntityListRangeRemoved<T>), e => RangeRemoved((AuditableEntityListRangeRemoved<T>)e));
    }

    protected override void AddItemAddedEvent(T item)
    {
        AddDomainEvent(new AuditableEntityListItemAdded<T>(
            Ulid.NewUlid(),
            EntityId,
            FieldId,
            FieldName,
            ++EventVersion,
            item.GetEntityId(),
            DateTimeOffset.UtcNow));
    }

    protected override void AddItemRemovedAtEvent(int index)
    {
        AddDomainEvent(new AuditableEntityListRemoveAt<T>(
            Ulid.NewUlid(),
            EntityId,
            FieldId,
            FieldName,
            ++EventVersion,
            index,
            DateTimeOffset.UtcNow));
    }

    protected override void AddItemInsertedEvent(int index, T item)
    {
        AddDomainEvent(new AuditableEntityListItemInserted<T>(
            Ulid.NewUlid(),
            EntityId,
            FieldId,
            FieldName,
            ++EventVersion,
            item.GetEntityId(),
            index,
            DateTimeOffset.UtcNow));
    }

    protected override void AddRangeInsertedEvent(int index, ICollection<T> items)
    {
        AddDomainEvent(new AuditableEntityListRangeInserted<T>(
            Ulid.NewUlid(),
            EntityId,
            FieldId,
            FieldName,
            ++EventVersion,
            items.Select(i => i.GetEntityId()).ToList(),
            index,
            DateTimeOffset.UtcNow));
    }

    protected override void AddRangeRemovedEvent(int index, int count)
    {
        AddDomainEvent(new AuditableEntityListRangeRemoved<T>(
            Ulid.NewUlid(),
            EntityId,
            FieldId,
            FieldName,
            ++EventVersion,
            index,
            count,
            DateTimeOffset.UtcNow));
    }

    protected override void AddClearedEvent()
    {
        AddDomainEvent(new AuditableEntityListCleared<T>(
            Ulid.NewUlid(),
            EntityId,
            FieldId,
            FieldName,
            ++EventVersion,
            DateTimeOffset.UtcNow));
    }

    private void Hydrate(IDomainEvent domainEvent)
    {
        if (Hydrators.ContainsKey(domainEvent.GetType()))
        {
            Hydrators[domainEvent.GetType()](domainEvent);
        }
    }
    
    private void Initialized(AuditableEntityListInitialized<T> domainEvent)
    {
        Items = domainEvent.Items.ToArray();
        Size = Items.Length;
        // TODO pull entities from parent entity
        EventVersion = domainEvent.EventVersion;
    }
    
    private void ItemAdded(AuditableEntityListItemAdded<T> domainEvent)
    {
        _childEntities.TryGetValue(domainEvent.ItemId, out var targetItem);
        
        if (targetItem is null) throw new InvalidOperationException("Item not found for ItemAddedEvent.");
        
        Add((T)targetItem);
        EventVersion = domainEvent.EventVersion;
    }
    
    private void ItemRemovedAt(AuditableEntityListRemoveAt<T> domainEvent)
    {
        RemoveAt(domainEvent.Index);
        EventVersion = domainEvent.EventVersion;
    }
    
    private void Cleared(AuditableEntityListCleared<T> domainEvent)
    {
        Clear();
        EventVersion = domainEvent.EventVersion;
    }
    
    private void ItemInserted(AuditableEntityListItemInserted<T> domainEvent)
    {
        _childEntities.TryGetValue(domainEvent.ItemId, out var targetItem);
        
        if (targetItem is null) throw new InvalidOperationException("Item not found for ItemAddedEvent.");
        
        Insert(domainEvent.Index, (T)targetItem);
        EventVersion = domainEvent.EventVersion;
    }
    
    private void RangeInserted(AuditableEntityListRangeInserted<T> domainEvent)
    {
        var targetItems = new List<T>();
        foreach (var entityId in domainEvent.Items)
        {
            _childEntities.TryGetValue(entityId, out var targetItem);
            if (targetItem is not null)
            {
                targetItems.Add((T)targetItem);
            }
        }
        
        if (targetItems.Count != domainEvent.Items.Count) throw new InvalidOperationException("Items not found for RangeInsertedEvent.");
        
        InsertRange(domainEvent.Index, targetItems);
        EventVersion = domainEvent.EventVersion;
    }
    
    private void RangeRemoved(AuditableEntityListRangeRemoved<T> domainEvent)
    {
        RemoveRange(domainEvent.Index, domainEvent.Count);
        EventVersion = domainEvent.EventVersion;
    }

    public void AttachEntityList(Dictionary<Ulid, IAuditableChildEntity> childEntities)
    {
        _childEntities = childEntities;
    }
}