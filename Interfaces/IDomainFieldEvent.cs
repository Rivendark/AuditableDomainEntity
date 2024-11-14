namespace AuditableDomainEntity.Interfaces;

public interface IDomainFieldEvent : IDomainEvent
{
    public Ulid FieldId { get; init; }
    public string FieldName {get; init; }
}