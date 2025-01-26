using AuditableDomainEntity.Interfaces.Fields.EntityFields;

namespace AuditableDomainEntity.Events.EntityFieldEvents;

public record AuditableEntityCollectionFieldNullified<T>(
    Ulid EventId,
    Ulid FieldId,
    Ulid EntityId,
    string FieldName,
    float EventVersion,
    T[]? OldValue, // TODO Remove?
    DateTimeOffset CreatedAtUtc)
    : IAuditableEntityCollectionFieldNullified;