namespace AuditableDomainEntity.Interfaces.Collections;

public interface IAuditableCollection<T>
{
    public bool HasChanges();
    
    public void SetParentFieldValues(Ulid entityId, Ulid fieldId, string fieldName);

    public List<IDomainEvent> GetChanges();
}