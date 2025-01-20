namespace AuditableDomainEntity.Interfaces.Collections;

public interface IAuditableCollection<T>
{
    public bool HasChanges();
    
    public void SetEntityValues(Ulid entityId, Ulid fieldId, string fieldName);

    public List<IDomainEvent> GetChanges();
}