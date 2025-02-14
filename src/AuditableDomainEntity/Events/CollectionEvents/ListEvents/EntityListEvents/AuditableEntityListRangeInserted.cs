﻿using AuditableDomainEntity.Interfaces.Collections;

namespace AuditableDomainEntity.Events.CollectionEvents.ListEvents.EntityListEvents;

public record AuditableEntityListRangeInserted<T>(
    Ulid EventId,
    Ulid EntityId,
    Ulid FieldId,
    string FieldName,
    float EventVersion,
    ICollection<Ulid> Items,
    int Index,
    DateTimeOffset CreatedAtUtc
    ) : IAuditableEntityListDomainEvent;