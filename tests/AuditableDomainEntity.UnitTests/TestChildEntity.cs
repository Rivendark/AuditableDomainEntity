using AuditableDomainEntity.Attributes;

namespace AuditableDomainEntity.UnitTests;

public class TestChildEntity : AuditableEntity
{
    [AuditableValueField<bool?>]
    public bool? BoolProperty
    {
        get => GetValue<bool?>(nameof(BoolProperty));
        set => SetValue<bool?>(value, nameof(BoolProperty));
    }

    [AuditableValueField<int?>]
    public int? IntProperty
    {
        get => GetValue<int?>(nameof(IntProperty));
        set => SetValue<int?>(value, nameof(IntProperty));
    }

    public TestChildEntity() { }
    
    public TestChildEntity(AggregateRootId aggregateRootId, Ulid entityId) : base(aggregateRootId, entityId) { }
}