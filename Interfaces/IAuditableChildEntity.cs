namespace AuditableDomainEntity.Interfaces;

public interface IAuditableChildEntity
{
    Ulid GetEntityId();
    Ulid? GetParentEntityId();
    void SetParentEntityId(Ulid parentId);
    void SetFieldId(Ulid? fieldId);
    Ulid? GetFieldId();
    void Attach(Ulid parent, string propertyName);
    bool Initialized();
    AggregateRootId GetAggregateRootId();
    void FinalizeChanges(AggregateRootId aggregateRootId);
}