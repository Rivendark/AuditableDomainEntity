using AuditableDomainEntity.Events.EntityEvents;
using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Interfaces.Attributes;
using System.Reflection;

namespace AuditableDomainEntity;

public abstract partial class AuditableEntityBase
{
    public Ulid EntityId { get; init; } = Ulid.NewUlid();
    private readonly bool _isInitialized;
    private bool _isDirty;
    private int _version;
    private readonly Dictionary<string, Ulid> _propertyIds = new();
    private readonly Dictionary<Ulid, AuditableFieldRoot> _entityFields = new();
    private readonly Dictionary<Ulid, AuditableFieldRoot> _valueFields = new();
    private readonly Dictionary<Ulid, AuditableEntity> _children = new();

    protected AuditableEntityBase(Ulid entityId)
    {
        EntityId = entityId;
        InitializeNewProperties();
        _isInitialized = true;
    }

    protected void SetValue<T>(T? value, string propertyName)
    {
        var propertyType = typeof(T);
        
        switch (value)
        {
            case ValueType:
                SetValueType(value, propertyName);
                return;
            case AuditableEntity entity :
                SetValueAuditableEntity(entity, propertyName);
                return;
            case null:
                switch (typeof(T))
                {
                    case { IsValueType:true }:
                        SetValueType(value, propertyName);
                        return;
                    case var _ when propertyType.IsSubclassOf(typeof(AuditableEntity)):
                        SetValueAuditableEntity(value as AuditableEntity, propertyName);
                        return;
                }
                break;
        }
    }

    private void SetValueType<T>(T? value, string propertyName)
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

    private void SetValueAuditableEntity<T>(T? value, string propertyName) where T : AuditableEntity
    {
        if (!_isInitialized) return;
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

    private AuditableValueField<T> GetAuditableDomainField<T>(PropertyInfo property)
    {
        if (!_propertyIds.TryGetValue(property.Name, out var fieldId))
        {
            throw new InvalidOperationException($"Property {property.Name} is not found in type {GetType().Name}:{nameof(_propertyIds)}");
        }

        return (AuditableValueField<T>)_valueFields[fieldId];
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
            var attributes = property.GetCustomAttributes().ToList();
            if (attributes.Any(a => a is IAuditableValueFieldAttribute))
                LoadValueField(property);
            if (attributes.Any(a => a is IAuditableEntityFieldAttribute))
                LoadEntityField(property);
        }   
    }

    private void LoadValueField(PropertyInfo property)
    {
        var contextType = typeof(AuditableValueField<>).MakeGenericType(property.PropertyType);
        dynamic auditableDomainField = Activator.CreateInstance(contextType, EntityId, property.Name)!;
        _propertyIds.Add(auditableDomainField.Name, auditableDomainField.FieldId);
        _valueFields.TryAdd(auditableDomainField.FieldId, auditableDomainField);
    }

    private void LoadEntityField(PropertyInfo property)
    {
        var contextType = typeof(AuditableEntityField<>).MakeGenericType(property.PropertyType);
        dynamic auditableDomainField = Activator.CreateInstance(contextType, EntityId, property.Name)!;
        _propertyIds.Add(auditableDomainField.Name, auditableDomainField.FieldId);
        _entityFields.TryAdd(auditableDomainField.FieldId, auditableDomainField);
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

    private List<IDomainValueFieldEvent> GetValueFieldChanges()
    {
        var events = new List<IDomainValueFieldEvent>();
        foreach (var fieldEvents in _valueFields.Values
                     .Select(fieldChanges => fieldChanges.GetChanges())
                     .ToList()
                 )
        {
            events.AddRange(fieldEvents.OfType<IDomainValueFieldEvent>());
        }

        return events;
    }

    private List<IDomainEntityFieldEvent> GetEntityFieldChanges()
    {
        var events = new List<IDomainEntityFieldEvent>();
        foreach (var fieldEvents in _entityFields.Values
                     .Select(fieldChanges => fieldChanges.GetChanges()))
        {
            events.AddRange(fieldEvents.OfType<IDomainEntityFieldEvent>());
        }

        return events;
    }
}