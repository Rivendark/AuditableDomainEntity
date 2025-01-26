using AuditableDomainEntity.Interfaces.Collections;

namespace AuditableDomainEntity.Events.CollectionEvents.ListEvents.ValueListEvents;

public record AuditableValueValueListRemoveAt<T>(
    Ulid EventId,
    Ulid EntityId,
    Ulid FieldId,
    string FieldName,
    float EventVersion,
    int Index,
    DateTimeOffset CreatedAtUtc
    ) : IAuditableValueListDomainEvent;