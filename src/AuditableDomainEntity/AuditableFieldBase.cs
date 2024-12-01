using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Interfaces.Fields.EntityFields;
using AuditableDomainEntity.Interfaces.Fields.ValueFields;
using AuditableDomainEntity.Types;
using System.Reflection;

namespace AuditableDomainEntity;

public abstract class AuditableFieldBase
{
    public Ulid FieldId { get; protected set; }
    public Ulid EntityId { get; protected set; }
    public string Name { get; init; }
    public Type? FieldType { get; init; }
    protected AuditableDomainFieldType Type { get; init; }
    protected AuditableDomainFieldStatus Status { get; set; } = AuditableDomainFieldStatus.Created;
    protected float Version = 0;
    protected PropertyInfo Property { get; init; }
    protected readonly List<IDomainEvent> Changes = [];
    private readonly List<IDomainEvent> _events = [];

    protected AuditableFieldBase(
        Ulid entityId,
        PropertyInfo property,
        AuditableDomainFieldType type)
    {
        Type = type;
        FieldId = Ulid.NewUlid();
        EntityId = entityId;
        Property = property;
        Name = property.Name;
        FieldType = property.PropertyType;
        _events = [];
    }
    
    protected AuditableFieldBase(
        Ulid fieldId,
        Ulid entityId,
        PropertyInfo property,
        AuditableDomainFieldType type)
    {
        Type = type;
        FieldId = fieldId;
        EntityId = entityId;
        Property = property;
        Name = property.Name;
        FieldType = property.PropertyType;
        _events = [];
    }

    protected AuditableFieldBase() { }
    
    public static dynamic GenerateNewField(Type fieldType, Ulid entityId, PropertyInfo property)
    {
        if (!fieldType.IsGenericType) throw new ArgumentException("Field type must be a generic type", nameof(fieldType));
        var contextType = fieldType.MakeGenericType(property.PropertyType);
        return Activator.CreateInstance(contextType, entityId, property)!;
    }

    public static dynamic GenerateExistingEntityField(
        Type fieldType,
        List<IDomainEntityFieldEvent> domainEvents,
        Dictionary<Ulid, IAuditableChildEntity?> auditableEntities,
        PropertyInfo property)
    {
        if (!fieldType.IsGenericType) throw new ArgumentException("Field type must be a generic type", nameof(fieldType));
        var contextType = fieldType.MakeGenericType(property.PropertyType);
        return Activator.CreateInstance(contextType, domainEvents, auditableEntities, property)!;
    }
    
    public static dynamic GenerateExistingValueField(
        Type fieldType,
        List<IDomainValueFieldEvent> domainEvents,
        PropertyInfo property)
    {
        if (!fieldType.IsGenericType) throw new ArgumentException("Field type must be a generic type", nameof(fieldType));
        var contextType = fieldType.MakeGenericType(property.PropertyType);
        return Activator.CreateInstance(contextType, domainEvents, property)!;
    }

    protected void SetEvents(List<IDomainEvent> domainEvents)
    {
        _events.AddRange(domainEvents);
    }
    
    public bool HasChanges() => Changes.Count > 0;
    
    public bool HasEvents() => _events.Count > 0;

    public void CommitChanges()
    {
        _events.AddRange(Changes);
        Changes.Clear();
    }
    
    protected void AddDomainEvent(IDomainEvent domainEvent) => Changes.Add(domainEvent);
    
    public List<IDomainEvent> GetChanges() => Changes;

    protected void Hydrate()
    {
        foreach (var domainEvent in _events.OrderBy(x => x.EventVersion))
        {
            Hydrate(domainEvent);
        }
    }

    protected abstract void Hydrate(IDomainEvent domainEvent);
}
