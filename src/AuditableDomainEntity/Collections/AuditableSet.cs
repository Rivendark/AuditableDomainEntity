using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Interfaces.Collections;

namespace AuditableDomainEntity.Collections;

public sealed class AuditableSet<T> : HashSet<T>, IAuditableCollection<T>
{

    public AuditableSet(T[] collection) : base(collection)
    {
        
    }
    
    public AuditableSet() { }

    public void TryAdd(T item)
    {
        throw new NotImplementedException();
    }

    public void TryRemove(T item)
    {
        throw new NotImplementedException();
    }

    public bool HasChanges()
    {
        throw new NotImplementedException();
    }

    public T[]? GetAddedValues()
    {
        throw new NotImplementedException();
    }

    public T[]? GetRemovedValues()
    {
        throw new NotImplementedException();
    }

    public List<IDomainEvent> GetChanges(float nextVersion, Ulid fieldId, Ulid entityId, string name)
    {
        throw new NotImplementedException();
    }
}