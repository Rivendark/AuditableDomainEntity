using AuditableDomainEntity.Interfaces.Collections;

namespace AuditableDomainEntity.Events.CollectionEvents.ListEvents.EntityListEvents;

public record AuditableEntityListRangeRemoved<T>(
    Ulid EventId,
    Ulid EntityId,
    Ulid FieldId,
    string FieldName,
    float EventVersion,
    int Index,
    int Count,
    DateTimeOffset CreatedAtUtc
    ) : IAuditableEntityListDomainEvent;