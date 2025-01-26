using AuditableDomainEntity.Interfaces.Collections;

namespace AuditableDomainEntity.Events.CollectionEvents.ListEvents.ValueListEvents;

public record AuditableValueValueListRangeInserted<T>(
    Ulid EventId,
    Ulid EntityId,
    Ulid FieldId,
    string FieldName,
    float EventVersion,
    ICollection<T> Items,
    int Index,
    DateTimeOffset CreatedAtUtc
    ) : IAuditableValueListDomainEvent;