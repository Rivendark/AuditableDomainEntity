namespace AuditableDomainEntity.Interfaces;

public interface IDomainEntityEvent : IDomainEvent
{
    public List<IDomainFieldEvent> DomainFieldEvents { get; init; }
}