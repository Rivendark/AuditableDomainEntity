using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Interfaces;

namespace AuditableDomainEntity;

public class TestRootEntity : AuditableAggregateRootEntity
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

    public TestRootEntity(
        AggregateRootId aggregateRootId,
        List<IDomainEntityEvent>? history)
        : base(aggregateRootId, history) { }

    public TestRootEntity(AggregateRootId aggregateRootId)
        : base(aggregateRootId) { }
}