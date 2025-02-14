﻿using AuditableDomainEntity.Collections.Lists;
using AuditableDomainEntity.Events.EntityEvents;
using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Interfaces.Attributes;
using System.Reflection;

namespace AuditableDomainEntity;

public abstract partial class AuditableEntityBase
{
    public AggregateRootId AggregateRootId { get; private set; }
    protected Ulid EntityId { get; private set; }
    protected Type EntityType { get; }
    protected Ulid? ParentEntityId { get; set; }
    protected Ulid? FieldId { get; set; }
    protected bool IsInitialized;
    protected readonly Dictionary<Ulid, IAuditableChildEntity?> ChildEntities = new();
    protected float Version;
    private bool _isDirty;
    private readonly Dictionary<string, Ulid> _propertyIds = new();
    private readonly Dictionary<Ulid, AuditableFieldBase> _entityFields = new();
    private readonly Dictionary<Ulid, AuditableFieldBase> _valueFields = new();

    protected AuditableEntityBase(AggregateRootId aggregateRootId, Ulid entityId)
    {
        AggregateRootId = aggregateRootId;
        EntityType = GetType();
        EntityId = entityId;
        InitializeNewProperties();
        IsInitialized = true;
    }

    protected AuditableEntityBase()
    {
        AggregateRootId = new AggregateRootId(Ulid.NewUlid(), GetType());
        EntityId = AggregateRootId.Value;
        EntityType = AggregateRootId.EntityType;
        if (!EntityType.IsSubclassOf(typeof(AuditableRootEntity)))
        {
            EntityId = Ulid.NewUlid();
        }
        InitializeNewProperties();
        IsInitialized = true;
    }

    private void AttachChild(IAuditableChildEntity? child, string propertyName)
    {
        if (child == null) throw new NullReferenceException(nameof(child));
        child.Attach(EntityId, propertyName);
        ChildEntities.TryAdd(child.GetEntityId(), child);
    }

    protected void SetValue<T>(T? value, string propertyName)
    {
        if (typeof(T).IsAssignableTo(typeof(IAuditableChildEntity)))
            throw new ArgumentException("Value is not an IAuditableChildEntity");
        var properties = EntityType.GetProperties();
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
    
    protected void SetValueList<T>(AuditableValueList<T>? value, string propertyName)
    {
        if (typeof(T).IsAssignableTo(typeof(IAuditableChildEntity)))
            throw new ArgumentException("Value is not an IAuditableChildEntity");
        var properties = EntityType.GetProperties();
        foreach (var property in properties)
        {
            if (property.Name != propertyName) continue;
            if (!IsInitialized)
            {
                if (!LoadInitialValueField(property))
                    throw new InvalidOperationException($"Property {propertyName} can not be initialized");
            }
            var auditableDomainField = GetValueListField<T>(property);
            auditableDomainField.FieldValue = value;
            _isDirty = true;
                    
            return;
        }

        throw new InvalidOperationException($"Property {propertyName} is not found in type {GetType().Name}");
    }

    protected void SetEntity<T>(T? value, string propertyName) where T : IAuditableChildEntity?
    {
        if (!typeof(T).IsAssignableTo(typeof(IAuditableChildEntity)))
            throw new ArgumentException("Value is not an IAuditableChildEntity");
        if (!IsInitialized) return;
        var properties = EntityType.GetProperties();
        foreach (var property in properties)
        {
            if (property.Name != propertyName) continue;
            var field = GetEntityField<T>(property);
            AttachChild(value, propertyName);
            field.FieldValue = value;
            _isDirty = true;
            
            return;
        }
    }
    
    protected void SetEntityList<T>(AuditableEntityList<T>? value, string propertyName) where T : IAuditableChildEntity
    {
        if (!typeof(T).IsAssignableTo(typeof(IAuditableChildEntity)))
            throw new ArgumentException("Value is not an IAuditableChildEntity");
        var properties = EntityType.GetProperties();
        foreach (var property in properties)
        {
            if (property.Name != propertyName) continue;
            if (!IsInitialized)
            {
                if (!LoadInitialValueField(property))
                    throw new InvalidOperationException($"Property {propertyName} can not be initialized");
            }
            var auditableDomainField = GetEntityListField<T>(property);
            auditableDomainField.FieldValue = value;
            _isDirty = true;
            
            return;
        }
    }
    
    protected T? GetValue<T>(string propertyName)
    {
        if (typeof(T).IsAssignableTo(typeof(IAuditableChildEntity)))
            throw new ArgumentException("Value is not an IAuditableChildEntity");
        
        var properties = EntityType.GetProperties();
        foreach (var property in properties)
        {
            if (property.Name != propertyName) continue;
            return GetValueField<T>(property).FieldValue;
        }

        throw new InvalidOperationException($"Property {propertyName} is not found in type {GetType().Name}");
    }
    
    protected AuditableValueList<T> GetValueList<T>(string propertyName)
    {
        if (typeof(T).IsAssignableTo(typeof(IAuditableChildEntity)))
            throw new ArgumentException("Value can not be of IAuditableChildEntity");
        
        var properties = EntityType.GetProperties();
        foreach (var property in properties)
        {
            if (property.Name != propertyName) continue;
            return GetValueListField<T>(property).FieldValue!;
        }

        throw new InvalidOperationException($"Property {propertyName} is not found in type {GetType().Name}");
    }

    protected T? GetEntity<T>(string propertyName) where T : IAuditableChildEntity?
    {
        var properties = EntityType.GetProperties();
        foreach (var property in properties)
        {
            if (property.Name != propertyName) continue;
            switch (typeof(T))
            {
                case { IsValueType: false, IsClass: true }:
                    return GetEntityField<T>(property).FieldValue;
            }
        }

        throw new InvalidOperationException($"Property {propertyName} is not found in type {GetType().Name}");
    }
    
    protected AuditableEntityList<T> GetEntityList<T>(string propertyName) where T : IAuditableChildEntity
    {
        var properties = EntityType.GetProperties();
        foreach (var property in properties)
        {
            if (property.Name != propertyName) continue;
            return GetEntityListField<T>(property).FieldValue!;
        }

        throw new InvalidOperationException($"Property {propertyName} is not found in type {GetType().Name}");
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
    
    private AuditableListValueField<T> GetValueListField<T>(PropertyInfo property)
    {
        if (!_propertyIds.TryGetValue(property.Name, out var fieldId))
        {
            throw new InvalidOperationException($"Unable to find PropertyId for {property.Name}. {GetType().Name}:{nameof(_propertyIds)}");
        }

        if (!_valueFields.TryGetValue(fieldId, out var field))
        {
            throw new InvalidOperationException($"PropertyField not found for {property.Name}. {GetType().Name}:{nameof(_valueFields)}");
        }

        return (AuditableListValueField<T>)field;
    }

    private AuditableEntityField<T?> GetEntityField<T>(PropertyInfo property) where T : IAuditableChildEntity?
    {
        if (!_propertyIds.TryGetValue(property.Name, out var fieldId))
        {
            throw new InvalidOperationException($"Unable to find PropertyId for {property.Name}. {GetType().Name}:{nameof(_propertyIds)}");
        }
        
        if (!_entityFields.TryGetValue(fieldId, out var field))
        {
            throw new InvalidOperationException($"PropertyField not found for {property.Name}. {GetType().Name}:{nameof(_entityFields)}");
        }
        
        return (AuditableEntityField<T?>)field;
    }

    private AuditableListEntityField<T> GetEntityListField<T>(PropertyInfo property) where T : IAuditableChildEntity
    {
        if (!_propertyIds.TryGetValue(property.Name, out var fieldId))
        {
            throw new InvalidOperationException($"Unable to find PropertyId for {property.Name}. {GetType().Name}:{nameof(_propertyIds)}");
        }
        
        if (!_entityFields.TryGetValue(fieldId, out var field))
        {
            throw new InvalidOperationException($"PropertyField not found for {property.Name}. {GetType().Name}:{nameof(_entityFields)}");
        }
        
        return (AuditableListEntityField<T>)field;
    }
    
    private void ApplyEntityEvent(IDomainEntityEvent domainEvent)
    {
        if (!_events.ContainsKey(domainEvent.EntityId))
        {
            _events.Add(domainEvent.EntityId, []);
        }
        
        _events[domainEvent.EntityId].Add(domainEvent);
        
        switch (domainEvent)
        {
            case AuditableEntityCreated auditableEntityCreated:
                AggregateRootId = auditableEntityCreated.Id;
                EntityId = auditableEntityCreated.EntityId;
                Version = auditableEntityCreated.EventVersion;
                ParentEntityId = auditableEntityCreated.ParentId;
                FieldId = auditableEntityCreated.FieldId;
                LoadEntityHistory(domainEvent);
                break;
            case AuditableEntityDeleted auditableEntityDeleted:
                break;
            case AuditableEntityUpdated auditableEntityUpdated:
                Version = auditableEntityUpdated.EventVersion;
                LoadEntityHistory(domainEvent);
                break;
        }
    }

    protected void ValidateAggregateRootId(AggregateRootId aggregateRootId)
    {
        if (typeof(AuditableRootEntity).IsSubclassOf(AggregateRootId.EntityType))
        {
            throw new ArgumentException($"Invalid Aggregate Root given. AggregateRootType: {aggregateRootId.EntityType.Name}, EntityType: {GetType().DeclaringType?.Name ?? GetType().Name}");
        }
    }

    protected void InitializeNewProperties()
    {
        var properties = EntityType.GetProperties();
        foreach (var property in properties)
        {
            var attributes = property.GetCustomAttributes().ToList();
            if (attributes.Any(a => a is IAuditableEntityListFieldAttribute))
                LoadEntityListField(property);
            if (attributes.Any(a => a is IAuditableValueListFieldAttribute))
                LoadValueListField(property);
            if (attributes.Any(a => a is IAuditableValueFieldAttribute))
                LoadValueField(property);
            if (attributes.Any(a => a is IAuditableEntityFieldAttribute))
                LoadEntityField(property);
        }   
    }

    private bool LoadInitialValueField(PropertyInfo property)
    {
        var attributes = property.GetCustomAttributes().ToList();
        if (!attributes.Any(a => a is IAuditableValueFieldAttribute
                or IAuditableValueListFieldAttribute)) return false;
        
        if (attributes.Any(a => a is IAuditableValueListFieldAttribute))
            LoadValueListField(property);
        
        if (attributes.Any(a => a is IAuditableValueFieldAttribute))
            LoadValueField(property);
        
        return true;
    }

    private void LoadEntityListField(PropertyInfo property)
    {
        if (_propertyIds.ContainsKey(property.Name)) return;
        
        dynamic auditableDomainField = CreateFieldBase(
            property,
            typeof(AuditableListEntityField<>),
            property.PropertyType.GenericTypeArguments[0],
            ChildEntities);
        _entityFields.TryAdd(auditableDomainField.FieldId, auditableDomainField);
    }

    private void LoadValueListField(PropertyInfo property)
    {
        if (_propertyIds.ContainsKey(property.Name)) return;
        
        dynamic auditableDomainField = CreateFieldBase(
            property,
            typeof(AuditableListValueField<>),
            property.PropertyType.GenericTypeArguments[0]);
        _valueFields.TryAdd(auditableDomainField.FieldId, auditableDomainField);
    }

    private void LoadValueField(PropertyInfo property)
    {
        if (_propertyIds.ContainsKey(property.Name)) return;
        
        dynamic auditableDomainField = CreateFieldBase(property, typeof(AuditableValueField<>));
        _valueFields.TryAdd(auditableDomainField.FieldId, auditableDomainField);
    }

    private void LoadEntityField(PropertyInfo property)
    {
        if (_propertyIds.ContainsKey(property.Name)) return;
        
        dynamic auditableDomainField = CreateFieldBase(property, typeof(AuditableEntityField<>));
        _entityFields.TryAdd(auditableDomainField.FieldId, auditableDomainField);
    }
    
    private AuditableFieldBase CreateFieldBase(PropertyInfo property, Type fieldType)
    {
        var contextType = fieldType.MakeGenericType(property.PropertyType);
        dynamic auditableDomainField = Activator.CreateInstance(contextType, EntityId, property)!;
        _propertyIds.Add(auditableDomainField.Name, auditableDomainField.FieldId);

        return auditableDomainField;
    }
    
    private AuditableFieldBase CreateFieldBase(PropertyInfo property, Type fieldType, Type genericType)
    {
        var contextType = fieldType.MakeGenericType(genericType);
        dynamic auditableDomainField = Activator.CreateInstance(contextType, EntityId, property)!;
        _propertyIds.Add(auditableDomainField.Name, auditableDomainField.FieldId);

        return auditableDomainField;
    }
    
    private AuditableFieldBase CreateFieldBase(
        PropertyInfo property,
        Type fieldType,
        Type genericType,
        Dictionary<Ulid, IAuditableChildEntity?> childEntities)
    {
        var contextType = fieldType.MakeGenericType(genericType);
        dynamic auditableDomainField = Activator.CreateInstance(contextType, EntityId, childEntities, property)!;
        _propertyIds.Add(auditableDomainField.Name, auditableDomainField.FieldId);

        return auditableDomainField;
    }
}