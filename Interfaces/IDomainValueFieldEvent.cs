namespace AuditableDomainEntity.Interfaces;

public interface IDomainValueFieldEvent : IDomainEvent
{
    public Ulid FieldId { get; init; }
    public string FieldName {get; init; }
}