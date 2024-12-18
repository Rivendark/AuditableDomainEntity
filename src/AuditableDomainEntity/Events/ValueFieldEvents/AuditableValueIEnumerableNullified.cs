using AuditableDomainEntity.Interfaces.Fields.ValueFields;

namespace AuditableDomainEntity.Events.ValueFieldEvents;

public record AuditableValueIEnumerableNullified<T>(
    Ulid EventId,
    Ulid FieldId,
    Ulid EntityId,
    string FieldName,
    float EventVersion,
    T[]? OldValue,
    DateTimeOffset CreatedAtUtc)
    : IAuditableValueIEnumerableNullified;