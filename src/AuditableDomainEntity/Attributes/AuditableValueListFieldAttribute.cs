using AuditableDomainEntity.Interfaces.Attributes;

namespace AuditableDomainEntity.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class AuditableValueListFieldAttribute<T> : Attribute, IAuditableValueListFieldAttribute
{
    public Type FieldType { get; }
    public T? DefaultValue { get; }
    public bool IsNullable { get; }
    
    public AuditableValueListFieldAttribute()
    {
        FieldType = typeof(T);
    }
    
    public AuditableValueListFieldAttribute(T? defaultValue)
    {
        DefaultValue = defaultValue;
        FieldType = typeof(T);
    }
    
    public AuditableValueListFieldAttribute(bool isNullable)
    {
        IsNullable = isNullable;
        FieldType = typeof(T);
    }
    
    public AuditableValueListFieldAttribute(T? defaultValue, bool isNullable)
    {
        DefaultValue = defaultValue;
        IsNullable = isNullable;
        FieldType = typeof(T);
    }
}