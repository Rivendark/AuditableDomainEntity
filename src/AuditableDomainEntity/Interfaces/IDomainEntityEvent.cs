using AuditableDomainEntity.Interfaces.Fields;
using AuditableDomainEntity.Interfaces.Fields.EntityFields;
using AuditableDomainEntity.Interfaces.Fields.ValueFields;

namespace AuditableDomainEntity.Interfaces;

public interface IDomainEntityEvent : IDomainEvent
{
    public AggregateRootId Id { get; init; }
    public Ulid? FieldId { get; init; }
    public Ulid? ParentId { get; init; }
    public List<IDomainEntityFieldEvent>? EntityFieldEvents { get; init; }
    public List<IDomainValueFieldEvent>? ValueFieldEvents { get; init; }
}