namespace AuditableDomainEntity.Interfaces.Attributes;

public interface IAuditableValueListFieldAttribute : IAuditableFieldAttribute
{
    public Type FieldType { get; }
};