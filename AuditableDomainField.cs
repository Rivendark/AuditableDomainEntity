using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Types;

namespace AuditableDomainEntity;

public sealed class AuditableDomainField<T> : AuditableDomainFieldBase<T>
{
    private bool Equals(AuditableDomainField<T> other)
    {
        return Equals(this, other);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is AuditableDomainField<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public AuditableDomainField(Ulid fieldId, Ulid entityId, string name)
        : base(fieldId, entityId, name, AuditableDomainFieldType.Value) { }

    public AuditableDomainField(Ulid fieldId, Ulid entityId, string name, T? value)
        : base(fieldId, entityId, name, AuditableDomainFieldType.Value, value) { }

    public AuditableDomainField(Ulid entityId, string name)
        : base(entityId, name, AuditableDomainFieldType.Value) { }
    
    public AuditableDomainField(List<IDomainFieldEvent> events)
        : base(AuditableDomainFieldType.Value, events) { }
    
    public static bool operator ==(AuditableDomainField<T>? left, AuditableDomainField<T>? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(AuditableDomainField<T>? left, AuditableDomainField<T>? right)
    {
        return !Equals(left, right);
    }
}