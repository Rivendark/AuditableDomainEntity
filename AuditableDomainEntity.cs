using AuditableDomainEntity.Interfaces;
using System.Reflection;

namespace AuditableDomainEntity;

public abstract class AuditableDomainEntity
{
    public AggregateRootId Id { get; set; }
    private Ulid EntityId => Id.Value;
    private readonly Dictionary<Ulid, List<IDomainEvent>> _domainEventChanges = new ();
    private readonly Dictionary<Ulid, List<IDomainEvent>> _domainEvents = new();
    private readonly Dictionary<string, Ulid> _propertyIds = new();
    private readonly Dictionary<Ulid, AuditableDomainFieldRoot> _domainFields = new();

    public AuditableDomainEntity()
    {
        // TODO get all AuditableDomainField objects via reflection and add to list
    }

    protected void SetValue<T>(T value, string propertyName)
    {
        var properties = GetType().GetProperties();
        foreach (var property in properties)
        {
            if (property.Name != propertyName) continue;
            var auditableDomainField = GetAuditableDomainField<T>(property, EntityId);
            InitializeField<T>(auditableDomainField);
            auditableDomainField.FieldValue = value;
            RecordDomainEvent<T>(auditableDomainField);
                    
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
            return GetAuditableDomainField<T>(property, EntityId).FieldValue;
        }

        throw new InvalidOperationException($"Property {propertyName} is not found in type {GetType().Name}");
    }

    private void InitializeField<T>(AuditableDomainField<T> auditableDomainField)
    {
        if (!auditableDomainField.IsInitialized())
        {
            if (!_propertyIds.ContainsKey(auditableDomainField.Name))
                _propertyIds.Add(auditableDomainField.Name, auditableDomainField.FieldId);
            
            var fieldId = _propertyIds[auditableDomainField.Name];
            
            _domainFields.TryAdd(fieldId, auditableDomainField);
            
            if (_domainEventChanges.TryGetValue(fieldId, out var domainEvents))
            {
                auditableDomainField.Initialize(fieldId, EntityId, domainEvents);
                return;
            }
            
            auditableDomainField.Initialize(fieldId, EntityId);
        }
    }

    private void RecordDomainEvent<T>(AuditableDomainField<T> auditableDomainField)
    {
        if (!auditableDomainField.HasChanges()) return;
        if (_domainEventChanges.ContainsKey(auditableDomainField.FieldId))
        {
            _domainEventChanges[auditableDomainField.FieldId] = auditableDomainField.GetChanges();
            return;
        }
                        
        _domainEventChanges.TryAdd(auditableDomainField.FieldId, auditableDomainField.GetChanges());
    }

    public List<IDomainEvent> GetDomainEvents()
    {
        var events = new List<IDomainEvent>();
        foreach (var domainEventsValue in _domainEventChanges.Values)
        {
            events.AddRange(domainEventsValue);
        }

        return events;
    }

    private AuditableDomainField<T> GetAuditableDomainField<T>(PropertyInfo property, Ulid entityId)
    {
        if (!_propertyIds.TryGetValue(property.Name, out var fieldId))
        {
            fieldId = Ulid.NewUlid();
            _propertyIds.Add(property.Name, fieldId);
        }
            
        if (!_domainFields.TryGetValue(fieldId, out var field))
        {
            AuditableDomainField<T> auditableDomainField;
            var attribute = property.GetCustomAttribute<AuditableFieldAttribute<T>>();
            if (attribute != null)
            {
                if (attribute.IsNullable)
                {
                    auditableDomainField = new AuditableDomainField<T>(
                        fieldId,
                        entityId,
                        property.Name);
                }
                else
                {
                    auditableDomainField = new AuditableDomainField<T>(
                        fieldId,
                        entityId,
                        property.Name,
                        attribute.DefaultValue);
                }
            }
            else
            {
                auditableDomainField = new AuditableDomainField<T>(
                    fieldId,
                    entityId,
                    property.Name);
            }
            
            return auditableDomainField;
        }
        
        return (AuditableDomainField<T>)field;
    }
}