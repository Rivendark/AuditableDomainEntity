using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Interfaces.Collections;

namespace AuditableDomainEntity.Collections.Lists;

public abstract partial class AuditableList<T> : IAuditableCollection<T>
{
    private readonly List<IDomainEvent> _changes = [];
    private readonly List<IDomainEvent> _events = [];
    protected bool IsInitialized = true;
    protected Ulid FieldId = Ulid.Empty;
    protected Ulid EntityId = Ulid.Empty;
    protected string FieldName = "unknown";
    protected float EventVersion;
    protected readonly Dictionary<Type, Action<IDomainEvent>> Hydrators = new();

    protected AuditableList(Ulid fieldId, IEnumerable<IDomainEvent> events)
    {
        Items = [];
        _events.AddRange(events);
        IsInitialized = false;
        FieldId = fieldId;
    }
    
    public bool HasChanges()
    {
        return _changes.Count > 0;
    }
    
    public List<IDomainEvent> GetChanges()
    {
        return _changes;
    }
    
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        if (!IsInitialized)
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
    
    public abstract void SetParentFieldValues(Ulid entityId, Ulid fieldId, string fieldName);
    
    protected abstract void AddHydrateMethods();
    
    protected abstract void AddItemAddedEvent(T item);
    
    protected abstract void AddItemRemovedAtEvent(int index);
    
    protected abstract void AddItemInsertedEvent(int index, T item);
    
    protected abstract void AddRangeInsertedEvent(int index, ICollection<T> items);
    
    protected abstract void AddRangeRemovedEvent(int index, int count);
    
    protected abstract void AddClearedEvent();
}