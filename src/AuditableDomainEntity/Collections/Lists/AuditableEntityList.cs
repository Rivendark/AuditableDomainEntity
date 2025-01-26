using AuditableDomainEntity.Events.CollectionEvents.ListEvents.EntityListEvents;
using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Interfaces.Collections;

namespace AuditableDomainEntity.Collections.Lists;

public sealed class AuditableEntityList<T> : AuditableList<T> where T : IAuditableChildEntity
{
    public static implicit operator AuditableEntityList<T>(List<T> l) => new (l);
    
    public AuditableEntityList()
    {
        AddHydrateMethods();
    }
    
    public AuditableEntityList(int capacity) : base(capacity)
    {
        AddHydrateMethods();
    }
    
    public AuditableEntityList(IEnumerable<T> items) : base(items)
    {
        AddHydrateMethods();
    }
    
    internal AuditableEntityList(Ulid fieldId, IList<IAuditableEntityListDomainEvent> events) : base(fieldId, events)
    {
        AddHydrateMethods();
        var orderedEvents = events.OrderBy(e => e.EventVersion).ToArray();
        if (orderedEvents.Length == 0)
        {
            throw new ArgumentException("No events found");
        }

        var initializedEvent = orderedEvents[0] as AuditableValueEntityListInitialized<T>;
        
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
                .Select(e => e as IAuditableValueListDomainEvent)
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
            if (changeEvent is AuditableValueEntityListInitialized<T> initializedChangeEvent)
            {
                AddDomainEvent(new AuditableValueEntityListInitialized<T>(
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
        Hydrators.TryAdd(typeof(AuditableValueEntityListInitialized<T>), e => Initialized((AuditableValueEntityListInitialized<T>)e));
        Hydrators.TryAdd(typeof(AuditableValueEntityListItemAdded<T>), e => ItemAdded((AuditableValueEntityListItemAdded<T>)e));
        Hydrators.TryAdd(typeof(AuditableValueEntityListRemoveAt<T>), e => ItemRemovedAt((AuditableValueEntityListRemoveAt<T>)e));
        Hydrators.TryAdd(typeof(AuditableValueEntityListCleared<T>), e => Cleared((AuditableValueEntityListCleared<T>)e));
        Hydrators.TryAdd(typeof(AuditableValueEntityListItemInserted<T>), e => ItemInserted((AuditableValueEntityListItemInserted<T>)e));
        Hydrators.TryAdd(typeof(AuditableValueEntityListRangeInserted<T>), e => RangeInserted((AuditableValueEntityListRangeInserted<T>)e));
        Hydrators.TryAdd(typeof(AuditableValueEntityListRangeRemoved<T>), e => RangeRemoved((AuditableValueEntityListRangeRemoved<T>)e));
    }

    private void Hydrate(IDomainEvent domainEvent)
    {
        if (Hydrators.ContainsKey(domainEvent.GetType()))
        {
            Hydrators[domainEvent.GetType()](domainEvent);
        }
    }
    
    private void Initialized(AuditableValueEntityListInitialized<T> domainEvent)
    {
        Items = domainEvent.Items.ToArray();
        Size = Items.Length;
        // TODO pull entities from parent entity
        EventVersion = domainEvent.EventVersion;
    }
    
    private void ItemAdded(AuditableValueEntityListItemAdded<T> domainEvent)
    {
        Add(domainEvent.Item);
        EventVersion = domainEvent.EventVersion;
    }
    
    private void ItemRemovedAt(AuditableValueEntityListRemoveAt<T> domainEvent)
    {
        RemoveAt(domainEvent.Index);
        EventVersion = domainEvent.EventVersion;
    }
    
    private void Cleared(AuditableValueEntityListCleared<T> domainEvent)
    {
        Clear();
        EventVersion = domainEvent.EventVersion;
    }
    
    private void ItemInserted(AuditableValueEntityListItemInserted<T> domainEvent)
    {
        Insert(domainEvent.Index, domainEvent.Item);
        EventVersion = domainEvent.EventVersion;
    }
    
    private void RangeInserted(AuditableValueEntityListRangeInserted<T> domainEvent)
    {
        InsertRange(domainEvent.Index, domainEvent.Items);
        EventVersion = domainEvent.EventVersion;
    }
    
    private void RangeRemoved(AuditableValueEntityListRangeRemoved<T> domainEvent)
    {
        RemoveRange(domainEvent.Index, domainEvent.Count);
        EventVersion = domainEvent.EventVersion;
    }
}