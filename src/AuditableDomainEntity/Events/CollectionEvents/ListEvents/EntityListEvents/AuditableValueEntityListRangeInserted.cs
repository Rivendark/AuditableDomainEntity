using AuditableDomainEntity.Interfaces.Collections;

namespace AuditableDomainEntity.Events.CollectionEvents.ListEvents.EntityListEvents;

public record AuditableValueEntityListRangeInserted<T>(
    Ulid EventId,
    Ulid EntityId,
    Ulid FieldId,
    string FieldName,
    float EventVersion,
    ICollection<T> Items,
    int Index,
    DateTimeOffset CreatedAtUtc
    ) : IAuditableEntityListDomainEvent;