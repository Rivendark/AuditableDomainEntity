using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Types;

namespace AuditableDomainEntity;

public sealed class AuditableDomainField<T> : AuditableDomainFieldBase<T>
{
    public AuditableDomainField(string name)
        : base(name, AuditableDomainFieldType.Value) { }
    
    public AuditableDomainField(Ulid fieldId, Ulid entityId, string name, T value)
        : base(fieldId, entityId, name, AuditableDomainFieldType.Value, value) { }
    
    public AuditableDomainField(Ulid entityId, string name)
        : base(entityId, name, AuditableDomainFieldType.Value) { }
    
    public AuditableDomainField(Ulid fieldId, Ulid entityId, string name, AuditableDomainFieldType type, T value)
        : base(fieldId, entityId, name, type, value) { }
    
    public AuditableDomainField(List<IDomainFieldEvent> events)
        : base(AuditableDomainFieldType.Value, events) { }
}