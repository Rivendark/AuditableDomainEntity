using AuditableDomainEntity.Events.ValueFieldEvents;
using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Interfaces.Collections;

namespace AuditableDomainEntity.Collections;

public class AuditableList<T> : List<T>, IAuditableCollection<T>
{
    private readonly List<T> _addedValues = new();
    private readonly List<T> _removedValues = new();

    public AuditableList(T[]? collection) : base(collection ?? []) { }

    public AuditableList() { }
    
    public IDomainEvent? GetChanges(Ulid fieldId, Ulid entityId, string name, float version)
    {
        if (_addedValues.Count > 0 || _removedValues.Count > 0)
        {
            return new AuditableValueCollectionFieldUpdated<T>(
                Ulid.NewUlid(),
                fieldId,
                entityId,
                name,
                version,
                _addedValues.ToArray(),
                _removedValues.ToArray(),
                ToArray(),
                DateTimeOffset.UtcNow);
        }

        return null;
    }
    
    public new void Add(T item)
    {
        _addedValues.Add(item);
        base.Add(item);
    }
    
    public new void Remove(T item)
    {
        _removedValues.Add(item);
        base.Remove(item);
    }

    public new void AddRange(IEnumerable<T> collection)
    {
        var items = collection.ToList();
        base.AddRange(items);
        _addedValues.AddRange(items);
    }
    
    public new void RemoveRange(int index, int count)
    {
        base.RemoveRange(index, count);
        _removedValues.AddRange(GetRange(index, count));
    }
    
    public void RemoveRange(IEnumerable<T> collection)
    {
        var items = collection.ToList();
        RemoveAll(items.Contains);
        _removedValues.AddRange(items);
    }

    public new void RemoveAt(int index)
    {
        base.RemoveAt(index);
        _removedValues.Add(this[index]);
    }
    
    public void TryAdd(T item)
    {
        if (!Contains(item))
        {
            Add(item);
        }
    }
    
    public void TryRemove(T item)
    {
        if (Contains(item))
        {
            Remove(item);
        }
    }
    
    public void TryUpdate(IEnumerable<T> addedValues, IEnumerable<T> removedValues)
    {
        RemoveRange(removedValues);
        AddRange(addedValues);
    }

    public void ClearChanges()
    {
        _addedValues.Clear();
        _removedValues.Clear();
    }

    public new void Clear()
    {
        _removedValues.AddRange(this);
        base.Clear();
    }
    
    public bool HasChanges() => _addedValues.Count > 0 || _removedValues.Count > 0;
    
    public T[]? GetAddedValues() => _addedValues?.ToArray();
    
    public T[]? GetRemovedValues() => _removedValues?.ToArray();
}