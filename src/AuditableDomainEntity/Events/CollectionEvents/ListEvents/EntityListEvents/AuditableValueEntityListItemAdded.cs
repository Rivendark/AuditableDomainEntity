using AuditableDomainEntity.Interfaces.Collections;

namespace AuditableDomainEntity.Events.CollectionEvents.ListEvents.EntityListEvents;

public record AuditableValueEntityListItemAdded<T>(
    Ulid EventId,
    Ulid EntityId,
    Ulid FieldId,
    string FieldName,
    float EventVersion,
    T Item,
    DateTimeOffset CreatedAtUtc
    ) : IAuditableEntityListDomainEvent;