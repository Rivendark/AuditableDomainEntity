using AuditableDomainEntity.Interfaces.Collections;

namespace AuditableDomainEntity.Events.CollectionEvents.ListEvents.ValueListEvents;

public record AuditableValueListCleared<T>(
    Ulid EventId,
    Ulid EntityId,
    Ulid FieldId,
    string FieldName,
    float EventVersion,
    DateTimeOffset CreatedAtUtc
    ) : IAuditableValueListDomainEvent;