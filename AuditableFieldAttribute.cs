using AuditableDomainEntity.Interfaces;


namespace AuditableDomainEntity;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public sealed class AuditableFieldAttribute<T> : Attribute
{
    private Type FieldType { get; }
    private T? DefaultValue { get; }
    public bool IsNullable { get; }
    public AuditableDomainField<T> Field { get; private set; }

    public AuditableFieldAttribute(string fieldName)
    {
        FieldType = typeof(T);
        Field = new AuditableDomainField<T>(fieldName);
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

    public void Hydrate(List<IDomainFieldEvent> events)
    {
        Field = new AuditableDomainField<T>(events);
    }
}