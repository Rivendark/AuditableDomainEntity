using AuditableDomainEntity.Attributes;

namespace AuditableDomainEntity.Tests;

public class TestValueTypeRootEntity : AuditableRootEntity
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

    public TestValueTypeRootEntity(AggregateRootId aggregateRootId)
        : base(aggregateRootId) { }

    public TestValueTypeRootEntity()
    {
        
    }
}