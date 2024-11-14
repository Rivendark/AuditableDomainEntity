﻿using AuditableDomainEntity.Interfaces;

namespace AuditableDomainEntity.Events.EntityEvents;

public record AuditableEntityCreated(
    AggregateRootId Id,
    Ulid EventId,
    Ulid EntityId,
    Ulid? FieldId,
    Ulid? ParentId,
    int EventVersion,
    List<IDomainFieldEvent>? FieldEvents,
    List<IDomainEntityEvent>? EntityFieldEvents,
    DateTimeOffset CreatedAtUtc
    ): IDomainEntityEvent;