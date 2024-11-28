using AuditableDomainEntity.Interfaces.Attributes;

namespace AuditableDomainEntity.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public sealed class AuditableValueFieldAttribute<T> : Attribute, IAuditableValueFieldAttribute
{
    private Type FieldType { get; }
    public T? DefaultValue { get; }
    public bool IsNullable { get; }

    public AuditableValueFieldAttribute()
    {
        FieldType = typeof(T);
    }

    public AuditableValueFieldAttribute(T? defaultValue)
    {
        DefaultValue = defaultValue;
    }

    public AuditableValueFieldAttribute(bool isNullable)
    {
        IsNullable = isNullable;
    }

    public AuditableValueFieldAttribute(T? defaultValue, bool isNullable)
    {
        DefaultValue = defaultValue;
        IsNullable = isNullable;
    }
}