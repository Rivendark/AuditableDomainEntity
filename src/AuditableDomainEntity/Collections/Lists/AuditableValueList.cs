﻿using AuditableDomainEntity.Events.CollectionEvents.ListEvents.ValueListEvents;
using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Interfaces.Collections;

namespace AuditableDomainEntity.Collections.Lists;

public sealed class AuditableValueList<T> : AuditableList<T>
{
    public static implicit operator AuditableValueList<T>(List<T> l) => new (l);
    
    public AuditableValueList()
    {
        AddHydrateMethods();
    }
    
    public AuditableValueList(int capacity) : base(capacity)
    {
        AddHydrateMethods();
    }
    
    public AuditableValueList(IEnumerable<T> items) : base(items)
    {
        AddHydrateMethods();
    }
    
    internal AuditableValueList(Ulid fieldId, IList<IAuditableValueListDomainEvent> events) : base(fieldId, events)
    {
        AddHydrateMethods();
        var orderedEvents = events.OrderBy(e => e.EventVersion).ToArray();
        if (orderedEvents.Length == 0)
        {
            throw new ArgumentException("No events found");
        }

        var initializedEvent = orderedEvents[0] as AuditableValueValueListInitialized<T>;
        
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
            if (changeEvent is AuditableValueValueListInitialized<T> initializedChangeEvent)
            {
                AddDomainEvent(new AuditableValueValueListInitialized<T>(
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
        Hydrators.TryAdd(typeof(AuditableValueValueListInitialized<T>), e => Initialized((AuditableValueValueListInitialized<T>)e));
        Hydrators.TryAdd(typeof(AuditableValueValueListItemAdded<T>), e => ItemAdded((AuditableValueValueListItemAdded<T>)e));
        Hydrators.TryAdd(typeof(AuditableValueValueListRemoveAt<T>), e => ItemRemovedAt((AuditableValueValueListRemoveAt<T>)e));
        Hydrators.TryAdd(typeof(AuditableValueValueListCleared<T>), e => Cleared((AuditableValueValueListCleared<T>)e));
        Hydrators.TryAdd(typeof(AuditableValueValueListItemInserted<T>), e => ItemInserted((AuditableValueValueListItemInserted<T>)e));
        Hydrators.TryAdd(typeof(AuditableValueValueListRangeInserted<T>), e => RangeInserted((AuditableValueValueListRangeInserted<T>)e));
        Hydrators.TryAdd(typeof(AuditableValueValueListRangeRemoved<T>), e => RangeRemoved((AuditableValueValueListRangeRemoved<T>)e));
    }

    private void Hydrate(IDomainEvent domainEvent)
    {
        if (Hydrators.ContainsKey(domainEvent.GetType()))
        {
            Hydrators[domainEvent.GetType()](domainEvent);
        }
    }
    
    private void Initialized(AuditableValueValueListInitialized<T> domainEvent)
    {
        Items = domainEvent.Items.ToArray();
        Size = Items.Length;
        EventVersion = domainEvent.EventVersion;
    }
    
    private void ItemAdded(AuditableValueValueListItemAdded<T> domainEvent)
    {
        Add(domainEvent.Item);
        EventVersion = domainEvent.EventVersion;
    }
    
    private void ItemRemovedAt(AuditableValueValueListRemoveAt<T> domainEvent)
    {
        RemoveAt(domainEvent.Index);
        EventVersion = domainEvent.EventVersion;
    }
    
    private void Cleared(AuditableValueValueListCleared<T> domainEvent)
    {
        Clear();
        EventVersion = domainEvent.EventVersion;
    }
    
    private void ItemInserted(AuditableValueValueListItemInserted<T> domainEvent)
    {
        Insert(domainEvent.Index, domainEvent.Item);
        EventVersion = domainEvent.EventVersion;
    }
    
    private void RangeInserted(AuditableValueValueListRangeInserted<T> domainEvent)
    {
        InsertRange(domainEvent.Index, domainEvent.Items);
        EventVersion = domainEvent.EventVersion;
    }
    
    private void RangeRemoved(AuditableValueValueListRangeRemoved<T> domainEvent)
    {
        RemoveRange(domainEvent.Index, domainEvent.Count);
        EventVersion = domainEvent.EventVersion;
    }
}