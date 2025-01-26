using AuditableDomainEntity.Interfaces.Collections;

namespace AuditableDomainEntity.Events.CollectionEvents.ListEvents.ValueListEvents;

public record AuditableValueValueListItemInserted<T>(
    Ulid EventId,
    Ulid EntityId,
    Ulid FieldId,
    string FieldName,
    float EventVersion,
    T Item,
    int Index,
    DateTimeOffset CreatedAtUtc
    ) : IAuditableValueListDomainEvent;