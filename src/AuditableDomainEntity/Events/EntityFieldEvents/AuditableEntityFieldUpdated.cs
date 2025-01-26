using AuditableDomainEntity.Interfaces.Fields.EntityFields;

namespace AuditableDomainEntity.Events.EntityFieldEvents;

public record AuditableEntityFieldUpdated<T>(
    Ulid EventId,
    Ulid FieldId,
    Ulid EntityId,
    string FieldName,
    float EventVersion,
    T? OldValue,
    T? NewValue,
    DateTimeOffset CreatedAtUtc
    ) : IAuditableEntityFieldUpdated;