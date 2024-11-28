using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Interfaces.Fields;
using AuditableDomainEntity.Interfaces.Fields.EntityFields;

namespace AuditableDomainEntity.Events.EntityEvents;

public record AuditableEntityAdded(
    AggregateRootId Id,
    Ulid EventId,
    Ulid EntityId,
    Ulid FieldId,
    string FieldName,
    Ulid ParentEntityId,
    int EventVersion,
    DateTimeOffset CreatedAtUtc
    ): IDomainEntityFieldEvent;