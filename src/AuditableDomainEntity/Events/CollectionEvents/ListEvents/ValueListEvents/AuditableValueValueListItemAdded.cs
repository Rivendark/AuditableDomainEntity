using AuditableDomainEntity.Interfaces.Collections;

namespace AuditableDomainEntity.Events.CollectionEvents.ListEvents.ValueListEvents;

public record AuditableValueValueListItemAdded<T>(
    Ulid EventId,
    Ulid EntityId,
    Ulid FieldId,
    string FieldName,
    float EventVersion,
    T Item,
    DateTimeOffset CreatedAtUtc
    ) : IAuditableValueListDomainEvent;