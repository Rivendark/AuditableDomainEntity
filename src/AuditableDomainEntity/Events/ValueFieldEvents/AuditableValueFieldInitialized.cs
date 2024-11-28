using AuditableDomainEntity.Interfaces;

namespace AuditableDomainEntity.Events.ValueFieldEvents;

public record AuditableValueFieldInitialized<T>(
    Ulid EventId,
    Ulid FieldId,
    Ulid EntityId,
    string FieldName,
    int EventVersion,
    T InitialValue,
    DateTimeOffset CreatedAtUtc
) : IDomainValueFieldEvent;