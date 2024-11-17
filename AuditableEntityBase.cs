using AuditableDomainEntity.Events.EntityEvents;
using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Interfaces.Attributes;
using System.Reflection;

namespace AuditableDomainEntity;

public abstract partial class AuditableEntityBase
{
    public Ulid EntityId { get; protected set; } = Ulid.NewUlid();
    protected bool IsInitialized;
    private bool _isDirty;
    private int _version;
    private readonly Dictionary<string, Ulid> _propertyIds = new();
    private readonly Dictionary<Ulid, AuditableFieldRoot> _entityFields = new();
    private readonly Dictionary<Ulid, AuditableFieldRoot> _valueFields = new();
    private readonly Dictionary<Ulid, AuditableEntity?> _children = new();

    protected AuditableEntityBase(Ulid entityId)
    {
        EntityId = entityId;
        InitializeNewProperties();
        IsInitialized = true;
    }

    protected AuditableEntityBase()
    {
        
    }

    private void AttachChild(AuditableEntity? child, string propertyName)
    {
        if (child == null) throw new NullReferenceException(nameof(child));
        child.Attach(EntityId, propertyName);
        _children.Add(child.EntityId, child);
    }

    protected void SetValue<T>(T? value, string propertyName)
    {
        switch (value)
        {
            case ValueType:
                SetValueType(value, propertyName);
                return;
            case IAuditableChildEntity:
                SetValueEntity(value, propertyName);
                return;
            case null:
                switch (typeof(T))
                {
                    case { IsValueType: true }:
                        SetValueType(value, propertyName);
                        return;
                    case { IsValueType: false }:
                        SetValueEntity<T>(propertyName);
                        return;
                }
                break;
        }
    }
    
    protected T? GetValue<T>(string propertyName)
    {
        var properties = GetType().GetProperties();
        foreach (var property in properties)
        {
            if (property.Name != propertyName) continue;
            switch (typeof(T))
            {
                case { IsValueType: true }:
                    return GetValueField<T>(property).FieldValue;
                case { IsValueType: false, IsClass: true }:
                    return GetEntityField<T>(property).FieldValue;
            }
        }

        throw new InvalidOperationException($"Property {propertyName} is not found in type {GetType().Name}");
    }
    
    private void SetValueType<T>(T? value, string propertyName)
    {
        var properties = GetType().GetProperties();
        foreach (var property in properties)
        {
            if (property.Name != propertyName) continue;
            if (!IsInitialized)
            {
                if (!LoadInitialValueField(property))
                    throw new InvalidOperationException($"Property {propertyName} can not be initialized");
            }
            var auditableDomainField = GetValueField<T>(property);
            auditableDomainField.FieldValue = value;
            _isDirty = true;
                    
            return;
        }

        throw new InvalidOperationException($"Property {propertyName} is not found in type {GetType().Name}");
    }

    private void SetValueEntity<T>(T value, string propertyName)
    {
        if (!IsInitialized) return;
        var properties = GetType().GetProperties();
        foreach (var property in properties)
        {
            if (property.Name != propertyName) continue;
            var field = GetEntityField<T>(property);
            AttachChild(value as AuditableEntity, propertyName);
            field.FieldValue = value;
            _isDirty = true;
            
            return;
        }
    }

    private void SetValueEntity<T>(string propertyName)
    {
        if (!IsInitialized) return;
        var properties = GetType().GetProperties();
        foreach (var property in properties)
        {
            if (property.Name != propertyName) continue;
            var field = GetEntityField<T>(property);
            field.FieldValue = default;
            _isDirty = true;
            
            return;
        }
        
    }
    
    private AuditableValueField<T> GetValueField<T>(PropertyInfo property)
    {
        if (!_propertyIds.TryGetValue(property.Name, out var fieldId))
        {
            throw new InvalidOperationException($"Unable to find PropertyId for {property.Name}. {GetType().Name}:{nameof(_propertyIds)}");
        }

        if (!_valueFields.TryGetValue(fieldId, out var field))
        {
            throw new InvalidOperationException($"PropertyField not found for {property.Name}. {GetType().Name}:{nameof(_valueFields)}");
        }

        return (AuditableValueField<T>)field;
    }

    private AuditableEntityField<T> GetEntityField<T>(PropertyInfo property)
    {
        if (!_propertyIds.TryGetValue(property.Name, out var fieldId))
        {
            throw new InvalidOperationException($"Unable to find PropertyId for {property.Name}. {GetType().Name}:{nameof(_propertyIds)}");
        }
        
        if (!_entityFields.TryGetValue(fieldId, out var field))
        {
            throw new InvalidOperationException($"PropertyField not found for {property.Name}. {GetType().Name}:{nameof(_entityFields)}");
        }
        
        return (AuditableEntityField<T>)field;
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

    protected void InitializeNewProperties()
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

    private bool LoadInitialValueField(PropertyInfo property)
    {
        var attributes = property.GetCustomAttributes().ToList();
        if (!attributes.Any(a => a is IAuditableValueFieldAttribute)) return false;
        LoadValueField(property);
        return true;
    }

    private void LoadValueField(PropertyInfo property)
    {
        if (_propertyIds.ContainsKey(property.Name)) return;
        
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

        foreach (var entity in _children.Values.OfType<AuditableEntity>())
        {
            events.AddRange(entity.GetEntityChanges());
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