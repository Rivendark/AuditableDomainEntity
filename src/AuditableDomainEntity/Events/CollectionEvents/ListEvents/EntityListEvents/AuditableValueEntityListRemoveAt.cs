using AuditableDomainEntity.Interfaces.Collections;

namespace AuditableDomainEntity.Events.CollectionEvents.ListEvents.EntityListEvents;

public record AuditableValueEntityListRemoveAt<T>(
    Ulid EventId,
    Ulid EntityId,
    Ulid FieldId,
    string FieldName,
    float EventVersion,
    int Index,
    DateTimeOffset CreatedAtUtc
    ) : IAuditableEntityListDomainEvent;