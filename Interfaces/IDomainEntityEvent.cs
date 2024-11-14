namespace AuditableDomainEntity.Interfaces;

public interface IDomainEntityEvent : IDomainEvent
{
    public AggregateRootId Id { get; init; }
    public Ulid? FieldId { get; init; }
    public Ulid? ParentId { get; init; }
    public List<IDomainEntityEvent>? EntityFieldEvents { get; init; }
    public List<IDomainFieldEvent>? FieldEvents { get; init; }
}