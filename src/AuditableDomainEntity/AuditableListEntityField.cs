using AuditableDomainEntity.Collections.Lists;
using AuditableDomainEntity.Events.EntityFieldEvents;
using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Interfaces.Collections;
using AuditableDomainEntity.Interfaces.Fields.EntityFields;
using AuditableDomainEntity.Types;
using System.Reflection;

namespace AuditableDomainEntity;

public class AuditableListEntityField<T> : AuditableFieldBase where T : IAuditableChildEntity
{
    private AuditableEntityList<T>? _value;
    private AuditableEntityList<T>? _holder;
    private Dictionary<Ulid, IAuditableChildEntity> _childEntities = new();
    
    public AuditableEntityList<T>? FieldValue
    {
        get => _value;
        set => ApplyValue(value);
    }
    
    public AuditableListEntityField(
        Ulid fieldId,
        Ulid entityId,
        PropertyInfo property,
        Dictionary<Ulid, IAuditableChildEntity> childEntities)
        : base(fieldId, entityId, property, AuditableDomainFieldType.EntityCollection)
    {
        FieldType = typeof(T);
        _childEntities = childEntities;
    }
    
    public AuditableListEntityField(
        Ulid entityId,
        PropertyInfo property,
        Dictionary<Ulid, IAuditableChildEntity> childEntities)
        : base(entityId, property, AuditableDomainFieldType.EntityCollection)
    {
        FieldType = typeof(T);
        _childEntities = childEntities;
    }
    
    public AuditableListEntityField(
        List<IDomainEntityFieldEvent> domainEvents,
        PropertyInfo property,
        Dictionary<Ulid, IAuditableChildEntity> childEntities)
    {
        _childEntities = childEntities;
        var initializedEvent = domainEvents.FirstOrDefault(x => x is AuditableEntityFieldInitialized<T[]>);
        
        if (initializedEvent is null)
            throw new ArgumentException($"Failed to find auditable domain field initialized event for type {GetType().Name}<{FieldType?.Name}>");
        
        SetEvents(domainEvents.Select(IDomainEvent (e) => e).ToList());
        
        var iEvent = initializedEvent as AuditableEntityFieldInitialized<T[]>;
        
        if (property.Name != iEvent!.FieldName)
            throw new ArgumentException($"Field name mismatch. Expected {property.Name} but got {iEvent.FieldName}");
        
        FieldType = typeof(T);
        Name = iEvent.FieldName;
        Type = AuditableDomainFieldType.Entity;

        var listEvents = domainEvents.OfType<IAuditableEntityListDomainEvent>().ToList();
        
        _holder = new AuditableEntityList<T>(FieldId, listEvents, _childEntities);
        
        Hydrate();

        Status = AuditableDomainFieldStatus.Initialized;
    }
    
    public List<IAuditableChildEntity> Children => _childEntities.Values.ToList();
    
    public override List<IDomainEvent> GetChanges()
    {
        var listEvents = _value?.GetChanges();
        if (listEvents is not null)
            foreach (var domainEvent in listEvents)
            {
                AddDomainEvent(domainEvent);
            }

        return base.GetChanges();
    }

    private void ApplyValue(AuditableEntityList<T>? value)
    {
        if (!HasEvents())
        {
            if (value is not null)
            {
                _value = InstantiateCollection(value);
                AddDomainEvent(new AuditableEntityFieldInitialized<T[]>(
                    Ulid.NewUlid(),
                    FieldId,
                    EntityId,
                    Name,
                    ++Version,
                    _value.ToArray(),
                    DateTimeOffset.UtcNow));
            }
        }
        else
        {
            if (value is null && _value is not null)
            {
                _value.Clear();
                AddDomainEvent(new AuditableEntityCollectionFieldNullified<T>(
                    Ulid.NewUlid(),
                    FieldId,
                    EntityId,
                    Name,
                    ++Version,
                    _value.ToArray(),
                    DateTimeOffset.UtcNow));
                
                _value = null;
                
                return;
            }
            
            if (_value is null && value is not null)
            {
                _value = InstantiateCollection(value);
                AddDomainEvent(new AuditableEntityFieldInitialized<T[]>(
                    Ulid.NewUlid(),
                    FieldId,
                    EntityId,
                    Name,
                    ++Version,
                    _value.ToArray(),
                    DateTimeOffset.UtcNow));
                
                return;
            }

            if (_value is null || value is null) return;
            if (_value.Equals(value)) return;
            
            _value.Clear();
            _value.AddRange(value);
        }
    }

    protected override void Hydrate(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case IAuditableEntityFieldInitialized:
                var auditableFieldInitialized = domainEvent as AuditableEntityFieldInitialized<T[]>;
                FieldId = auditableFieldInitialized!.FieldId;
                EntityId = auditableFieldInitialized.EntityId;
                _value = _holder;
                Version = auditableFieldInitialized.EventVersion;
                break;
            
            case IAuditableEntityCollectionFieldNullified:
                var auditableFieldNullified = domainEvent as AuditableEntityCollectionFieldNullified<T>;
                _value = null;
                Version = auditableFieldNullified!.EventVersion;
                break;
        }
    }
    
    private AuditableEntityList<T> InstantiateCollection(AuditableEntityList<T>? list)
    {
        if (_holder is not null)
        {
            if (_holder.Count > 0)
                _holder.Clear();
            
            if (list is not null)
            {
                _holder.AddRange(list.ToArray());
                list.SetAsReadOnly();
            }
            return _holder;
        }
        
        list ??= [];
        
        list.SetParentFieldValues(EntityId, FieldId, Name);

        _holder = list;

        return list;
    }
}