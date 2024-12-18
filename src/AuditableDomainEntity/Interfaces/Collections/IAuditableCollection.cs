using System.Collections;

namespace AuditableDomainEntity.Interfaces.Collections;

public interface IAuditableCollection<T>
{
    public bool HasChanges();
    
    public void TryAdd(T item);

    public void TryRemove(T item);


    public T[]? GetAddedValues();

    public T[]? GetRemovedValues();
}