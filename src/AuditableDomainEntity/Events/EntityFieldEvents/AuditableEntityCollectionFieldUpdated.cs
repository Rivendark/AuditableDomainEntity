using AuditableDomainEntity.Interfaces.Fields.EntityFields;

namespace AuditableDomainEntity.Events.EntityFieldEvents;

public record AuditableEntityCollectionFieldUpdated<T>(
    Ulid EventId,
    Ulid FieldId,
    Ulid EntityId,
    string FieldName,
    float EventVersion,
    T[]? AddedValues,
    T[]? RemovedValues,
    T[]? CurrentValues,
    DateTimeOffset CreatedAtUtc)
    : IAuditableEntityCollectionFieldUpdated;