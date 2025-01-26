using AuditableDomainEntity.Interfaces.Collections;

namespace AuditableDomainEntity.Events.CollectionEvents.ListEvents.EntityListEvents;

public record AuditableValueEntityListInitialized<T>(
    Ulid EventId,
    Ulid EntityId,
    Ulid FieldId,
    string FieldName,
    float EventVersion,
    IEnumerable<T> Items,
    DateTimeOffset CreatedAtUtc
    ) : IAuditableEntityListDomainEvent;