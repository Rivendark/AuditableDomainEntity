using AuditableDomainEntity.Events.EntityEvents;
using AuditableDomainEntity.Interfaces;
using System.Reflection;

namespace AuditableDomainEntity;

public abstract partial class AuditableEntityBase
{
    public Ulid EntityId { get; init; } = Ulid.NewUlid();
    private readonly bool _isInitialized;
    private bool _isDirty;
    private int _version;
    private readonly Dictionary<string, Ulid> _propertyIds = new();
    private readonly Dictionary<Ulid, AuditableDomainFieldRoot> _auditableEntityFields = new();

    protected AuditableEntityBase(Ulid entityId)
    {
        EntityId = entityId;
        InitializeNewProperties();
        _isInitialized = true;
    }

    protected void SetValue<T>(T value, string propertyName)
    {
        if (!_isInitialized) return;
        var properties = GetType().GetProperties();
        foreach (var property in properties)
        {
            if (property.Name != propertyName) continue;
            var auditableDomainField = GetAuditableDomainField<T>(property);
            auditableDomainField.FieldValue = value;
            _isDirty = true;
                    
            return;
        }

        throw new InvalidOperationException($"Property {propertyName} is not found in type {GetType().Name}");
    }

    protected T? GetValue<T>(string propertyName)
    {
        var properties = GetType().GetProperties();
        foreach (var property in properties)
        {
            if (property.Name != propertyName) continue;
            return GetAuditableDomainField<T>(property).FieldValue;
        }

        throw new InvalidOperationException($"Property {propertyName} is not found in type {GetType().Name}");
    }

    private AuditableDomainValueField<T> GetAuditableDomainField<T>(PropertyInfo property)
    {
        if (!_propertyIds.TryGetValue(property.Name, out var fieldId))
        {
            throw new InvalidOperationException($"Property {property.Name} is not found in type {GetType().Name}:{nameof(_propertyIds)}");
        }

        return (AuditableDomainValueField<T>)_auditableEntityFields[fieldId];
    }

    private void ApplyEntityEvent(IDomainEntityEvent domainEvent)
    {
        switch (domainEvent)
        {
            case AuditableEntityCreated auditableEntityCreated:
                
                break;
            case AuditableEntityDeleted auditableEntityDeleted:
                // TODO
                break;
            case AuditableEntityUpdated auditableEntityUpdated:
                // TODO
                break;
        }
    }

    private void InitializeNewProperties()
    {
        var properties = GetType().GetProperties();
        foreach (var property in properties)
        {
            var attributes = property.GetCustomAttributes();
            if (!attributes.Any(a => a is IAuditableFieldAttribute)) continue;
            
            var contextType = typeof(AuditableDomainValueField<>).MakeGenericType(property.PropertyType);
            dynamic auditableDomainField = Activator.CreateInstance(contextType, EntityId, property.Name)!;
            _propertyIds.Add(auditableDomainField.Name, auditableDomainField.FieldId);
            _auditableEntityFields.TryAdd(auditableDomainField.FieldId, auditableDomainField);
        }
    }

    protected void ValidateAggregateRootId(AggregateRootId aggregateRootId)
    {
        if ((GetType().DeclaringType ?? GetType()) != aggregateRootId.EntityType)
        {
            throw new ArgumentException($"Invalid Aggregate Root given. AggregateRootType: {aggregateRootId.EntityType.Name}, EntityType: {GetType().DeclaringType?.Name ?? GetType().Name}");
        }
    }
    
    public List<IDomainEntityEvent> GetEntityChanges()
    {
        var events = new List<IDomainEntityEvent>();
        foreach (var entityEvents in _entityChanges.Values)
        {
            events.AddRange(entityEvents);
        }

        return events;
    }

    private List<IDomainFieldEvent> GetFieldChanges()
    {
        var events = new List<IDomainFieldEvent>();
        foreach (var fieldEvents in _auditableEntityFields.Values
                     .Select(fieldChanges => fieldChanges.GetChanges()))
        {
            events.AddRange(fieldEvents);
        }

        return events;
    }
}