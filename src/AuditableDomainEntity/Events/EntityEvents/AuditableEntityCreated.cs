using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Interfaces.Fields;
using AuditableDomainEntity.Interfaces.Fields.EntityFields;
using AuditableDomainEntity.Interfaces.Fields.ValueFields;

namespace AuditableDomainEntity.Events.EntityEvents;

public record AuditableEntityCreated(
    AggregateRootId Id,
    Ulid EventId,
    Ulid EntityId,
    Type EntityType,
    Ulid? FieldId,
    Ulid? ParentId,
    int EventVersion,
    List<IDomainValueFieldEvent>? ValueFieldEvents,
    List<IDomainEntityFieldEvent>? EntityFieldEvents,
    DateTimeOffset CreatedAtUtc
    ): IDomainEntityEvent;