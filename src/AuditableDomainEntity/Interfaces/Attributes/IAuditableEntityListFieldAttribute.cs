namespace AuditableDomainEntity.Interfaces.Attributes;

public interface IAuditableEntityListFieldAttribute : IAuditableFieldAttribute
{
    public Type FieldType { get; }
}