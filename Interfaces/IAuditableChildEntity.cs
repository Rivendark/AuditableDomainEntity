namespace AuditableDomainEntity.Interfaces;

public interface IAuditableChildEntity
{
    void Attach(Ulid parent, string propertyName);
    bool Initialized();
}