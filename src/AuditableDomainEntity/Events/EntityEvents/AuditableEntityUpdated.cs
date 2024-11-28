using AuditableDomainEntity.Interfaces;

namespace AuditableDomainEntity.Events.EntityEvents;

public record AuditableEntityUpdated(
    AggregateRootId Id,
    Ulid EventId,
    Ulid EntityId,
    Ulid? FieldId,
    Ulid? ParentId,
    int EventVersion,
    List<IDomainValueFieldEvent>? ValueFieldEvents,
    List<IDomainEntityFieldEvent>? EntityFieldEvents,
    DateTimeOffset CreatedAtUtc)
    : IDomainEntityEvent;