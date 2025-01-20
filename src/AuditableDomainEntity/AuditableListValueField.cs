using AuditableDomainEntity.Collections.Lists;
using AuditableDomainEntity.Events.ValueFieldEvents;
using AuditableDomainEntity.Interfaces;
using AuditableDomainEntity.Interfaces.Collections;
using AuditableDomainEntity.Interfaces.Fields.ValueFields;
using AuditableDomainEntity.Types;
using System.Reflection;

namespace AuditableDomainEntity;

public sealed class AuditableListValueField<T> : AuditableFieldBase
{
    private AuditableList<T>? _value;
    private AuditableList<T>? _holder;
    
    public AuditableList<T>? FieldValue
    {
        get => _value;
        set => ApplyValue(value);
    }

    public AuditableListValueField(
        Ulid fieldId,
        Ulid entityId,
        PropertyInfo property)
        : base(fieldId, entityId, property, AuditableDomainFieldType.ValueCollection)
    {
        FieldType = typeof(T);
    }
    
    public AuditableListValueField(
        Ulid entityId,
        PropertyInfo property)
        : base(entityId, property, AuditableDomainFieldType.ValueCollection)
    {
        FieldType = typeof(T);
    }

    public AuditableListValueField(List<IDomainValueFieldEvent> domainEvents, PropertyInfo property)
    {
        var initializedEvent = domainEvents.FirstOrDefault(x => x is AuditableValueFieldInitialized<T[]>);
        
        if (initializedEvent is null)
            throw new ArgumentException($"Failed to find auditable domain field initialized event for type {GetType().Name}<{FieldType?.Name}>");
        
        SetEvents(domainEvents.Select(IDomainEvent (e) => e).ToList());
        
        var iEvent = initializedEvent as AuditableValueFieldInitialized<T[]>;
        
        if (property.Name != iEvent!.FieldName)
            throw new ArgumentException($"Field name mismatch. Expected {property.Name} but got {iEvent.FieldName}");
        
        FieldType = typeof(T);
        Name = iEvent.FieldName;
        Type = AuditableDomainFieldType.Value;

        var listEvents = domainEvents.OfType<IAuditableListDomainEvent>();
        
        _holder = new AuditableList<T>(FieldId, listEvents);
        
        Hydrate();

        Status = AuditableDomainFieldStatus.Initialized;
    }
    
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
    
    protected override void Hydrate(IDomainEvent domainEvent)
    {
        switch (domainEvent)
        {
            case IAuditableValueFieldInitialized:
                var auditableFieldInitialized = domainEvent as AuditableValueFieldInitialized<T[]>;
                FieldId = auditableFieldInitialized!.FieldId;
                EntityId = auditableFieldInitialized.EntityId;
                _value = _holder;
                Version = auditableFieldInitialized.EventVersion;
                break;
            
            case IAuditableValueIEnumerableNullified:
                var auditableFieldNullified = domainEvent as AuditableValueIEnumerableNullified<T>;
                _value = null;
                Version = auditableFieldNullified!.EventVersion;
                break;
        }
    }
    
    private void ApplyValue(AuditableList<T>? value)
    {
        if (!HasEvents())
        {
            if (value is not null)
            {
                _value = InstantiateCollection(value);
                AddDomainEvent(new AuditableValueFieldInitialized<T[]>(
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
                AddDomainEvent(new AuditableValueIEnumerableNullified<T>(
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
                AddDomainEvent(new AuditableValueFieldInitialized<T[]>(
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
    
    private AuditableList<T> InstantiateCollection(AuditableList<T>? list)
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
        
        list.SetEntityValues(EntityId, FieldId, Name);

        _holder = list;

        return list;
    }
}
