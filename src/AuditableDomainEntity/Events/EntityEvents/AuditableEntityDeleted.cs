using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Interfaces.Fields;
using AuditableDomainEntity.Interfaces.Fields.EntityFields;
using AuditableDomainEntity.Interfaces.Fields.ValueFields;

namespace AuditableDomainEntity.Events.EntityEvents;

public record AuditableEntityDeleted(
    AggregateRootId Id,
    Ulid EventId,
    Ulid EntityId,
    float EventVersion,
    DateTimeOffset CreatedAtUtc,
    Ulid? FieldId,
    Ulid? ParentId,
    List<IDomainEntityFieldEvent>? EntityFieldEvents,
    List<IDomainValueFieldEvent> ValueFieldEvents
    ) : IDomainEntityEvent;