using AuditableDomainEntity.Attributes;

namespace AuditableDomainEntity.Tests;

public class TestChildEntity : AuditableEntity
{
    [AuditableValueField<int?>]
    public bool? BoolProperty
    {
        get => GetValue<bool?>(nameof(BoolProperty));
        set => SetValue<bool?>(value, nameof(BoolProperty));
    }
    
    public TestChildEntity(Ulid entityId, Ulid parentEntityId, string propertyName)
        : base(entityId, parentEntityId, propertyName) { }

    public TestChildEntity() { }
}