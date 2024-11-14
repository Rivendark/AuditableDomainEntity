using AuditableDomainEntity.Interfaces;


namespace AuditableDomainEntity;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public sealed class AuditableFieldAttribute<T> : Attribute, IAuditableFieldAttribute
{
    private Type FieldType { get; }
    public T? DefaultValue { get; }
    public bool IsNullable { get; }

    public AuditableFieldAttribute(string fieldName)
    {
        FieldType = typeof(T);
    }

    public AuditableFieldAttribute(string fieldName, T? defaultValue) : this(fieldName)
    {
        DefaultValue = defaultValue;
    }

    public AuditableFieldAttribute(string fieldName, bool isNullable) : this(fieldName)
    {
        IsNullable = isNullable;
    }

    public AuditableFieldAttribute(string fieldName, T? defaultValue, bool isNullable) : this(fieldName)
    {
        DefaultValue = defaultValue;
        IsNullable = isNullable;
    }
}