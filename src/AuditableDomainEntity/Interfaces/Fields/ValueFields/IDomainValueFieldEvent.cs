namespace AuditableDomainEntity.Interfaces.Fields.ValueFields;

public interface IDomainValueFieldEvent : IDomainEvent
{
    public Ulid FieldId { get; init; }
    public string FieldName {get; init; }
}