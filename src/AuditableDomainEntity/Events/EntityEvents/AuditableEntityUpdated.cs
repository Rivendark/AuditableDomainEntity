using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Interfaces.Fields;
using AuditableDomainEntity.Interfaces.Fields.EntityFields;
using AuditableDomainEntity.Interfaces.Fields.ValueFields;

namespace AuditableDomainEntity.Events.EntityEvents;

public record AuditableEntityUpdated(
    AggregateRootId Id,
    Ulid EventId,
    Ulid EntityId,
    Ulid? FieldId,
    Ulid? ParentId,
    float EventVersion,
    List<IDomainValueFieldEvent>? ValueFieldEvents,
    List<IDomainEntityFieldEvent>? EntityFieldEvents,
    DateTimeOffset CreatedAtUtc)
    : IDomainEntityEvent;