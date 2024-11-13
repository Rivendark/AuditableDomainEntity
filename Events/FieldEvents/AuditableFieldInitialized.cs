using AuditableDomainEntity.Interfaces;

namespace AuditableDomainEntity.Events.FieldEvents;

public record AuditableFieldInitialized<T>(
    Ulid EventId,
    Ulid FieldId,
    Ulid EntityId,
    string FieldName,
    int EventVersion,
    T InitialValue,
    DateTimeOffset CreatedAtUtc
) : IDomainFieldEvent;