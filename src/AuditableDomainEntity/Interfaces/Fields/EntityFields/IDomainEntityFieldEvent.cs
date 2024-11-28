namespace AuditableDomainEntity.Interfaces.Fields.EntityFields;

public interface IDomainEntityFieldEvent : IDomainEvent
{
    public Ulid FieldId { get; init; }
    public string FieldName {get; init; }
}