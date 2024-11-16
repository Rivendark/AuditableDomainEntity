using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Types;

namespace AuditableDomainEntity;

public sealed class AuditableEntityField<T> : AuditableFieldBase<T>
{
    public AuditableEntityField(Ulid entityId, string name)
        : base(entityId, name, AuditableDomainFieldType.Entity) { }

    public AuditableEntityField(Ulid fieldId, Ulid entityId, string name)
        : base(fieldId, entityId, name, AuditableDomainFieldType.Entity) { }

    public AuditableEntityField(List<IDomainValueFieldEvent> domainEvents)
        : base(AuditableDomainFieldType.Entity, domainEvents) { }

    protected override void ApplyValue(T value)
    {
        throw new NotImplementedException();
    }
    
    public new List<IDomainEntityFieldEvent> GetChanges() => base.GetChanges()
        .OfType<IDomainEntityFieldEvent>()
        .ToList()!;
}