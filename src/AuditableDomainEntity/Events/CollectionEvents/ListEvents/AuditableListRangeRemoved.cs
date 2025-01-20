using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Interfaces.Collections;

namespace AuditableDomainEntity.Events.CollectionEvents.ListEvents;

public record AuditableListRangeRemoved<T>(
    Ulid EventId,
    Ulid EntityId,
    Ulid FieldId,
    string FieldName,
    float EventVersion,
    int Index,
    int Count,
    DateTimeOffset CreatedAtUtc
    ) : IAuditableListDomainEvent;