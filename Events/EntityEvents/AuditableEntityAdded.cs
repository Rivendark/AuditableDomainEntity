﻿using AuditableDomainEntity.Interfaces;

namespace AuditableDomainEntity.Events.EntityEvents;

public record AuditableEntityAdded(
    Ulid EventId,
    Ulid EntityId,
    Ulid FieldId,
    string FieldName,
    Ulid? ParentId,
    int EventVersion,
    DateTimeOffset CreatedAtUtc
    ): IDomainEntityFieldEvent;