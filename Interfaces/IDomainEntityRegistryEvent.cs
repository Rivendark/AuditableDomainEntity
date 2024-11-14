namespace AuditableDomainEntity.Interfaces;

public interface IDomainEntityRegistryEvent : IDomainEvent
{
    public AggregateRootId Id { get; init; }
    public Ulid? FieldId { get; init; }
    public string PropertyName { get; init; }
    public Ulid? ParentId { get; init; }
}