using AuditableDomainEntity.Interfaces.Collections;

namespace AuditableDomainEntity.Events.CollectionEvents.ListEvents.EntityListEvents;

public record AuditableEntityListItemInserted<T>(
    Ulid EventId,
    Ulid EntityId,
    Ulid FieldId,
    string FieldName,
    float EventVersion,
    Ulid ItemId,
    int Index,
    DateTimeOffset CreatedAtUtc
    ) : IAuditableEntityListDomainEvent;