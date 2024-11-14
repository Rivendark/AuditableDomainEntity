using AuditableDomainEntity.Interfaces;

namespace AuditableDomainEntity;

public class TestRootEntity : AuditableAggregateRootEntity
{
    [AuditableField<int?>(nameof(Number), true)]
    public int? Number
    {
        get => GetValue<int?>(nameof(Number));
        set => SetValue<int?>(value, nameof(Number));
    }

    [AuditableField<DateTime?>(nameof(Date),true)]
    public DateTime? Date
    {
        get => GetValue<DateTime?>(nameof(Date));
        set => SetValue<DateTime?>(value, nameof(Date));
    }

    public TestRootEntity(AggregateRootId aggregateRootId, List<IDomainEvent>? history) : base(aggregateRootId, history)
    {
        
    }

    public TestRootEntity(AggregateRootId aggregateRootId) : base(aggregateRootId)
    {
        
    }
}