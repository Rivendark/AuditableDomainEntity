using AuditableDomainEntity.Interfaces;

namespace AuditableDomainEntity.Events.EntityEvents;

public record AuditableEntityDeleted(
    AggregateRootId Id,
    Ulid EventId,
    Ulid EntityId,
    int EventVersion,
    DateTimeOffset CreatedAtUtc,
    Ulid? FieldId,
    Ulid? ParentId,
    List<IDomainEntityFieldEvent>? EntityFieldEvents,
    List<IDomainValueFieldEvent> ValueFieldEvents
    ) : IDomainEntityEvent;