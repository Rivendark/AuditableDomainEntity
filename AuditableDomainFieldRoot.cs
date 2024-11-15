using AuditableDomainEntity.Interfaces;

namespace AuditableDomainEntity;

public abstract class AuditableDomainFieldRoot
{
    protected readonly List<IDomainFieldEvent> Changes = [];
    protected readonly List<IDomainFieldEvent> Events = [];
    public bool HasChanges() => Changes.Count > 0;
    public List<IDomainFieldEvent> GetChanges() => Changes;

    public void CommitChanges()
    {
        Events.AddRange(Changes);
        Changes.Clear();
    }
};