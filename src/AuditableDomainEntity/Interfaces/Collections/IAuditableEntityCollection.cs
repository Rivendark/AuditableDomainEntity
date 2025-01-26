namespace AuditableDomainEntity.Interfaces.Collections;

public interface IAuditableEntityCollection
{
    public void AttachEntityList<T>(List<T> childEntities) where T : IAuditableChildEntity;
}