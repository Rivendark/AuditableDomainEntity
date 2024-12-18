using AuditableDomainEntity.Interfaces.Fields.ValueFields;

namespace AuditableDomainEntity.Events.ValueFieldEvents;

public record AuditableValueCollectionFieldUpdated<T>(
    Ulid EventId,
    Ulid FieldId,
    Ulid EntityId,
    string FieldName,
    float EventVersion,
    T[]? AddedValues,
    T[]? RemovedValues,
    T[]? CurrentValues,
    DateTimeOffset CreatedAtUtc)
    : IAuditableValueCollectionFieldUpdated;