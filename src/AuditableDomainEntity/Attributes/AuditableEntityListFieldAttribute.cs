using AuditableDomainEntity.Interfaces.Attributes;

namespace AuditableDomainEntity.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class AuditableEntityListFieldAttribute<T> : Attribute, IAuditableEntityListFieldAttribute
{
    public Type FieldType { get; }
    public T? DefaultValue { get; }
    public bool IsNullable { get; }
    
    public AuditableEntityListFieldAttribute()
    {
        FieldType = typeof(T);
    }
    
    public AuditableEntityListFieldAttribute(T? defaultValue)
    {
        DefaultValue = defaultValue;
        FieldType = typeof(T);
    }
    
    public AuditableEntityListFieldAttribute(bool isNullable)
    {
        IsNullable = isNullable;
        FieldType = typeof(T);
    }
    
    public AuditableEntityListFieldAttribute(T? defaultValue, bool isNullable)
    {
        DefaultValue = defaultValue;
        IsNullable = isNullable;
        FieldType = typeof(T);
    }
}