using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Interfaces;

namespace AuditableDomainEntity.Tests;

public class TestValueTypeRootEntity : AuditableAggregateRootEntity
{
    [AuditableValueField<int?>(true)]
    public int? Number
    {
        get => GetValue<int?>(nameof(Number));
        set => SetValue<int?>(value, nameof(Number));
    }

    [AuditableValueField<DateTime?>(true)]
    public DateTime? Date
    {
        get => GetValue<DateTime?>(nameof(Date));
        set => SetValue<DateTime?>(value, nameof(Date));
    }

    public TestValueTypeRootEntity(
        AggregateRootId aggregateRootId,
        List<IDomainEntityEvent>? history)
        : base(aggregateRootId, history) { }

    public TestValueTypeRootEntity(AggregateRootId aggregateRootId)
        : base(aggregateRootId) { }
}