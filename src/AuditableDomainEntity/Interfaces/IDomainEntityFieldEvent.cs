namespace AuditableDomainEntity.Interfaces;

public interface IDomainEntityFieldEvent : IDomainEvent
{
    public Ulid FieldId { get; init; }
    public string FieldName {get; init; }
}