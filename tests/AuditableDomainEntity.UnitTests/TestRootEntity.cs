using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Collections.Lists;

namespace AuditableDomainEntity.UnitTests;

public class TestRootEntity : AuditableRootEntity
{
    [AuditableEntityListField<TestChildEntity>(false)]
    public AuditableEntityList<TestChildEntity> TestChildren
    {
        get => GetEntityList<TestChildEntity>(nameof(TestChildren));
        set => SetEntityList<TestChildEntity>(value, nameof(TestChildren));
    }
    
    [AuditableValueListField<string>(true)]
    public AuditableValueList<string>? Strings
    {
        get => GetValueList<string>(nameof(Strings));
        set => SetValueList<string>(value, nameof(Strings));
    }
    
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
    
    public TestRootEntity(AggregateRootId aggregateRootId)
        : base(aggregateRootId)
    {
    }

    public TestRootEntity()
    {
        
    }
}