﻿using AuditableDomainEntity.Interfaces.Fields.ValueFields;

namespace AuditableDomainEntity.Events.ValueFieldEvents;

public record AuditableValueFieldInitialized<T>(
    Ulid EventId,
    Ulid FieldId,
    Ulid EntityId,
    string FieldName,
    float EventVersion,
    T? InitialValue,
    DateTimeOffset CreatedAtUtc
) : IAuditableValueFieldInitialized;