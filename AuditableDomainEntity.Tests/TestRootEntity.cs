using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Interfaces;

namespace AuditableDomainEntity.Tests;

public class TestRootEntity : AuditableAggregateRootEntity
{
    [AuditableEntityField<TestChildEntity>(true)]
    public TestChildEntity? Child
    {
        get => GetEntity<TestChildEntity?>(nameof(Child));
        set => SetEntity<TestChildEntity?>(value, nameof(Child));
    }

    [AuditableValueField<string>]
    public string? StringProperty
    {
        get => GetValue<string?>(nameof(StringProperty));
        set => SetValue<string?>(value, nameof(StringProperty));
    }
    
    public TestRootEntity(
        AggregateRootId aggregateRootId,
        List<IDomainEntityEvent> events)
        : base(aggregateRootId, events)
    {
    }

    public TestRootEntity(AggregateRootId aggregateRootId)
        : base(aggregateRootId)
    {
    }
}