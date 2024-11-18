using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Interfaces;

namespace AuditableDomainEntity.Tests;

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
    
    // public TestChildEntity(Ulid entityId, Ulid parentEntityId, string propertyName)
    //     : base(entityId, parentEntityId, propertyName) { }

    public TestChildEntity() { }

    public TestChildEntity(Ulid entityId, List<IDomainEntityEvent> events) : base(entityId, events) { }
}