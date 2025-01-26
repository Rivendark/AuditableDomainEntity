namespace AuditableDomainEntity.Interfaces.Collections;

public interface IAuditableEntityCollection
{
    public void AttachEntityList(Dictionary<Ulid, IAuditableChildEntity> childEntities);
}