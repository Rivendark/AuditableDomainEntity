using AuditableDomainEntity.Interfaces.Attributes;

namespace AuditableDomainEntity.Attributes;

public sealed class AuditableEntityFieldAttribute<T> : Attribute, IAuditableEntityFieldAttribute
{
    private Type FieldType { get; }
    public bool IsNullable { get; }

    public AuditableEntityFieldAttribute()
    {
        FieldType = typeof(T);
    }

    public AuditableEntityFieldAttribute(bool isNullable)
    {
        FieldType = typeof(T);
        IsNullable = isNullable;
    }
}