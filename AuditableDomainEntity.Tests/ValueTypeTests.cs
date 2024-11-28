using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Interfaces;

namespace AuditableDomainEntity.AuditableDomainEntity.Tests;

public class ValueTypeTests
{
    
}

public class IntTestClass : AuditableRootEntity
{
    [AuditableValueField<int?>(true)]
    public int? NullableInteger
    {
        get => GetValue<int?>(nameof(NullableInteger));
        set => SetValue<int?>(value, nameof(NullableInteger));
    }
    
    [AuditableValueField<int>(false)]
    public int NonNullableInteger
    {
        get => GetValue<int>(nameof(NonNullableInteger));
        set => SetValue<int>(value, nameof(NonNullableInteger));
    }
    
    public IntTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId)
    {
    }
}