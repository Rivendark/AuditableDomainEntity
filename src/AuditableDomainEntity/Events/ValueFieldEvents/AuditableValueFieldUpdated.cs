using AuditableDomainEntity.Interfaces;

namespace AuditableDomainEntity.Events.ValueFieldEvents;

public record AuditableValueFieldUpdated<T>(
    Ulid EventId,
    Ulid FieldId,
    Ulid EntityId,
    string FieldName,
    int EventVersion,
    T? OldValue,
    T? NewValue,
    DateTimeOffset CreatedAtUtc
    ) : IDomainValueFieldEvent;