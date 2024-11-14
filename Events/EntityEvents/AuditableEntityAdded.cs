using AuditableDomainEntity.Interfaces;

namespace AuditableDomainEntity.Events.EntityEvents;

public record AuditableEntityAdded(
    AggregateRootId Id,
    Ulid EventId,
    Ulid EntityId,
    Ulid? FieldId,
    string PropertyName,
    Ulid? ParentId,
    int EventVersion,
    DateTimeOffset CreatedAtUtc
    ): IDomainEntityRegistryEvent;