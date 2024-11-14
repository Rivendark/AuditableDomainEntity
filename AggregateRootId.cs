using System.Reflection.Metadata;

namespace AuditableDomainEntity;

public record AggregateRootId(Ulid Value, Type EntityType) : IComparable<AggregateRootId>
{
    public int CompareTo(AggregateRootId? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        return other is null ? 1 : Value.CompareTo(other.Value);
    }
}