using AuditableDomainEntity.Interfaces;

namespace AuditableDomainEntity.Events.EntityEvents;

public record AuditableEntityUpdated(
    AggregateRootId Id,
    Ulid EventId,
    Ulid EntityId,
    Ulid? FieldId,
    Ulid? ParentId,
    int EventVersion,
    List<IDomainEntityEvent>? EntityFieldEvents,
    List<IDomainFieldEvent> FieldEvents,
    DateTimeOffset CreatedAtUtc)
    : IDomainEntityEvent;