using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Types;

namespace AuditableDomainEntity;

public sealed class AuditableDomainEntityField<T> : AuditableDomainFieldBase<T>
{
    public AuditableDomainEntityField(Ulid entityId, string name)
        : base(entityId, name, AuditableDomainFieldType.Entity) { }

    public AuditableDomainEntityField(Ulid fieldId, Ulid entityId, string name)
        : base(fieldId, entityId, name, AuditableDomainFieldType.Entity) { }

    public AuditableDomainEntityField(List<IDomainFieldEvent> domainEvents)
        : base(AuditableDomainFieldType.Entity, domainEvents) { }
}