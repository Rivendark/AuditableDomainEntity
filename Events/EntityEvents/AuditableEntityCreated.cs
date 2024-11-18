﻿using AuditableDomainEntity.Interfaces;

namespace AuditableDomainEntity.Events.EntityEvents;

public record AuditableEntityCreated(
    AggregateRootId Id,
    Ulid EventId,
    Ulid EntityId,
    Type EntityType,
    Ulid? FieldId,
    Ulid? ParentId,
    int EventVersion,
    List<IDomainValueFieldEvent>? ValueFieldEvents,
    List<IDomainEntityFieldEvent>? EntityFieldEvents,
    DateTimeOffset CreatedAtUtc
    ): IDomainEntityEvent;