using AuditableDomainEntity.Interfaces.Fields.ValueFields;

namespace AuditableDomainEntity.Events.ValueFieldEvents;

public record AuditableValueCollectionFieldFieldNullified<T>(
    Ulid EventId,
    Ulid FieldId,
    Ulid EntityId,
    string FieldName,
    float EventVersion,
    T[]? OldValue, // TODO Remove?
    DateTimeOffset CreatedAtUtc)
    : IAuditableValueCollectionFieldNullified;