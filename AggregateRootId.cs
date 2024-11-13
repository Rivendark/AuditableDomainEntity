using System.Reflection.Metadata;

namespace AuditableDomainEntity;

public record AggregateRootId(Ulid Value, Ulid? ParentId = null);