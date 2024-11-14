using AuditableDomainEntity.Interfaces;

namespace AuditableDomainEntity.Events.EntityEvents;

public record AuditableEntityCreated(
    Ulid EventId,
    Ulid EntityId,
    int EventVersion,
    List<IDomainFieldEvent> DomainFieldEvents,
    DateTimeOffset CreatedAtUtc
    ): IDomainEntityEvent;