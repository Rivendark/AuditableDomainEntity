﻿using AuditableDomainEntity.Events.EntityEvents;
using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Interfaces.Fields.EntityFields;
using AuditableDomainEntity.Types;
using System.Reflection;

namespace AuditableDomainEntity;

public sealed class AuditableEntityField<T> : AuditableFieldBase where T : IAuditableChildEntity?
{
    private T? _value;
    public T? FieldValue
    {
        get => _value;
        set => ApplyValue(value);
    }
    
    private readonly Dictionary<Ulid, T> _auditableEntities = new();
    public AuditableEntityField(Ulid entityId, PropertyInfo property)
        : base(entityId, property, AuditableDomainFieldType.Entity) { }

    public AuditableEntityField(Ulid fieldId, Ulid entityId, PropertyInfo property)
        : base(fieldId, entityId, property, AuditableDomainFieldType.Entity) { }
    
    public AuditableEntityField(
        List<IDomainEntityFieldEvent> domainEvents,
        Dictionary<Ulid, IAuditableChildEntity> auditableEntities,
        PropertyInfo property)
    {
        var initializedEvent = domainEvents.OrderBy(e => e.EventVersion).FirstOrDefault(x => x is AuditableEntityAdded);
        
        if (initializedEvent is null)
            throw new ArgumentException($"Failed to find auditable domain field initialized event for type {GetType().Name}");
        
        var iEvent = initializedEvent as AuditableEntityAdded;
        
        if (property.Name != iEvent!.FieldName)
            throw new ArgumentException($"Field name mismatch. Expected {property.Name} but got {iEvent.FieldName}");
        
        SetEvents(domainEvents.Select(IDomainEvent (e) => e).ToList());

        // Load Field Properties
        FieldId = iEvent.FieldId;
        FieldType = typeof(T);
        EntityId = iEvent.EntityId;
        Name = iEvent.FieldName;
        Type = AuditableDomainFieldType.Value;
        Version = iEvent.EventVersion;
        
        foreach (var child in auditableEntities
                     .Where(e => e.Value.GetFieldId() == FieldId  && e.Value is T))
        {
            _auditableEntities.Add(child.Key, (T)child.Value);
        }
        
        // Try load field Entity
        _auditableEntities.TryGetValue(iEvent.EntityId, out var existingEntity);
        _value = existingEntity ?? default;
        
        Hydrate();

        Status = AuditableDomainFieldStatus.Initialized;
    }

    private void ApplyValue(T? value)
    {
        if (_value is null && value is null) return;
        if (_value is null && value is not null)
        {
            if (value is IAuditableChildEntity)
            {
                value.SetParentEntityId(EntityId);
                value.SetFieldId(FieldId);
                AddDomainEvent(new AuditableEntityAdded(
                    value.GetAggregateRootId(),
                    Ulid.NewUlid(),
                    value.GetEntityId(),
                    FieldId,
                    Name,
                    EntityId,
                    Version,
                    DateTimeOffset.UtcNow
                    ));
                _value = value;
                return;
            }
            
            throw new InvalidCastException($"Cannot convert {value} to {typeof(T)}");
        }

        if (_value is not null && value is null)
        {
            if (value is IAuditableChildEntity)
            {
                AddDomainEvent(new AuditableEntityRemoved(
                Ulid.NewUlid(),
                value.GetEntityId(),
                FieldId,
                Name,
                EntityId,
                Version,
                DateTimeOffset.UtcNow));
                
                _value = value;
                return;
            }
            
            throw new InvalidCastException($"Cannot convert {value} to {typeof(T)}");
        }
    }

    protected override void Hydrate(IDomainEvent domainEvent)
    {
        
    }
}