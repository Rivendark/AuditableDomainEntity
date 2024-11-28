using AuditableDomainEntity.Interfaces;
using System.Reflection;

namespace AuditableDomainEntity;

public abstract class AuditableFieldRoot
{
    public Ulid FieldId { get; set; }
    public Ulid EntityId { get; set; }
    public string Name { get; init; }
    public Type? FieldType { get; init; }
    private readonly List<IDomainEvent> _changes = [];
    private readonly List<IDomainEvent> _events = [];
    
    public static dynamic GenerateNewField(Type fieldType, Ulid entityId, PropertyInfo property)
    {
        if (!fieldType.IsGenericType) throw new ArgumentException("Field type must be a generic type", nameof(fieldType));
        var contextType = fieldType.MakeGenericType(property.PropertyType);
        return Activator.CreateInstance(contextType, entityId, property.Name)!;
    }

    public static dynamic GenerateExistingEntityField(
        Type fieldType,
        List<IDomainEntityFieldEvent> domainEvents,
        Dictionary<Ulid, IAuditableChildEntity?> auditableEntities,
        Type propertyType)
    {
        if (!fieldType.IsGenericType) throw new ArgumentException("Field type must be a generic type", nameof(fieldType));
        var contextType = fieldType.MakeGenericType(propertyType);
        return Activator.CreateInstance(contextType, domainEvents, auditableEntities)!;
    }
    
    public static dynamic GenerateExistingValueField(
        Type fieldType,
        List<IDomainValueFieldEvent> domainEvents,
        PropertyInfo property)
    {
        if (!fieldType.IsGenericType) throw new ArgumentException("Field type must be a generic type", nameof(fieldType));
        var contextType = fieldType.MakeGenericType(property.PropertyType);
        return Activator.CreateInstance(contextType, domainEvents)!;
    }

    protected AuditableFieldRoot(Ulid fieldId, Ulid entityId, string name)
    {
        FieldId = fieldId;
        EntityId = entityId;
        Name = name;
        _events = new List<IDomainEvent>();
    }

    protected AuditableFieldRoot()
    {
    }

    protected void SetEvents(List<IDomainEvent> domainEvents)
    {
        _events.AddRange(domainEvents);
    }
    
    public bool HasChanges() => _changes.Count > 0;

    public void CommitChanges()
    {
        _events.AddRange(_changes);
        _changes.Clear();
    }
    
    protected void AddDomainEvent(IDomainEvent domainEvent) => _changes.Add(domainEvent);
    
    public virtual List<IDomainEvent> GetChanges() => _changes;
    
    protected List<IDomainEvent> GetEvents() => _events;
};