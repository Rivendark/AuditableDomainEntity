namespace AuditableDomainEntity.Interfaces.Attributes;

public interface IAuditableFieldAttribute
{
    public bool IsNullable { get; }
};