using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Interfaces.Fields;
using AuditableDomainEntity.Interfaces.Fields.ValueFields;

namespace AuditableDomainEntity.Events.ValueFieldEvents;

public record AuditableValueFieldUpdated<T>(
    Ulid EventId,
    Ulid FieldId,
    Ulid EntityId,
    string FieldName,
    float EventVersion,
    T? OldValue,
    T? NewValue,
    DateTimeOffset CreatedAtUtc
    ) : IAuditableValueFieldUpdated;