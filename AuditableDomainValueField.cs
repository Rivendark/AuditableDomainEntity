using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Types;

namespace AuditableDomainEntity;

public sealed class AuditableDomainValueField<T> : AuditableDomainFieldBase<T>
{
    public AuditableDomainValueField(Ulid fieldId, Ulid entityId, string name)
        : base(fieldId, entityId, name, AuditableDomainFieldType.Value) { }

    public AuditableDomainValueField(Ulid entityId, string name)
        : base(entityId, name, AuditableDomainFieldType.Value) { }
    
    public AuditableDomainValueField(List<IDomainFieldEvent> events)
        : base(AuditableDomainFieldType.Value, events) { }
}