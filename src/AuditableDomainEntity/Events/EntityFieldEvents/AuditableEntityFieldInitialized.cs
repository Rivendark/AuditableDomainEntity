using AuditableDomainEntity.Interfaces.Fields.EntityFields;

namespace AuditableDomainEntity.Events.EntityFieldEvents;

public record AuditableEntityFieldInitialized<T>(
    Ulid EventId,
    Ulid FieldId,
    Ulid EntityId,
    string FieldName,
    float EventVersion,
    T? InitialValue,
    DateTimeOffset CreatedAtUtc
) : IAuditableEntityFieldInitialized;