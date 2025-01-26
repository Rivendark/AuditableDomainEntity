using AuditableDomainEntity.Interfaces.Collections;

namespace AuditableDomainEntity.Events.CollectionEvents.ListEvents.ValueListEvents;

public record AuditableValueValueListRangeRemoved<T>(
    Ulid EventId,
    Ulid EntityId,
    Ulid FieldId,
    string FieldName,
    float EventVersion,
    int Index,
    int Count,
    DateTimeOffset CreatedAtUtc
    ) : IAuditableValueListDomainEvent;