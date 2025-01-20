using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Interfaces.Collections;

namespace AuditableDomainEntity.Events.CollectionEvents.ListEvents;

public record AuditableListInitialized<T>(
    Ulid EventId,
    Ulid EntityId,
    Ulid FieldId,
    string FieldName,
    float EventVersion,
    IEnumerable<T> Items,
    DateTimeOffset CreatedAtUtc
    ) : IAuditableListDomainEvent;