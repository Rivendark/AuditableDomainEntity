using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Interfaces;

namespace AuditableDomainEntity.Tests;

public class TestRootEntity : AuditableAggregateRootEntity
{
    [AuditableEntityField<TestChildEntity>(true)]
    public TestChildEntity Child
    {
        get => GetValue<TestChildEntity?>(nameof(Child));
        set => SetValue<TestChildEntity?>(value, nameof(Child));
    }
    
    public TestRootEntity(
        AggregateRootId aggregateRootId,
        List<IDomainEntityEvent>? events)
        : base(aggregateRootId, events)
    {
    }

    public TestRootEntity(AggregateRootId aggregateRootId)
        : base(aggregateRootId)
    {
    }
}