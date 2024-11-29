namespace AuditableDomainEntity.Interfaces;

public interface IDomainEvent
{
    public Ulid EventId { get; init; }
    public Ulid EntityId { get; init; }
    public float EventVersion { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; }
}