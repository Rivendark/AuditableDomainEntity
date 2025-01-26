using AuditableDomainEntity.Interfaces.Collections;

namespace AuditableDomainEntity.Events.CollectionEvents.ListEvents.EntityListEvents;

public record AuditableValueEntityListItemInserted<T>(
    Ulid EventId,
    Ulid EntityId,
    Ulid FieldId,
    string FieldName,
    float EventVersion,
    T Item,
    int Index,
    DateTimeOffset CreatedAtUtc
    ) : IAuditableEntityListDomainEvent;