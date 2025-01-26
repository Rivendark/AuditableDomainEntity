using AuditableDomainEntity.Interfaces.Collections;

namespace AuditableDomainEntity.Events.CollectionEvents.ListEvents.EntityListEvents;

public record AuditableEntityListItemAdded<T>(
    Ulid EventId,
    Ulid EntityId,
    Ulid FieldId,
    string FieldName,
    float EventVersion,
    Ulid ItemId,
    DateTimeOffset CreatedAtUtc
    ) : IAuditableEntityListDomainEvent;