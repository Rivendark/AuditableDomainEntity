using AuditableDomainEntity.Attributes;

namespace AuditableDomainEntity.FunctionalTests.PersonTests;

public class AddressDbo : AuditableEntity
{
    [AuditableValueField<string>(false)]
    public string AddressLine1
    {
        get => GetValue<string>(nameof(AddressLine1)) ?? string.Empty;
        set => SetValue(value, nameof(AddressLine1));
    }
    
    [AuditableValueField<string>(true)]
    public string? AddressLine2
    {
        get => GetValue<string?>(nameof(AddressLine2));
        set => SetValue<string?>(value, nameof(AddressLine2));
    }
    
    [AuditableValueField<string>(false)]
    public string City
    {
        get => GetValue<string>(nameof(City)) ?? string.Empty;
        set => SetValue(value, nameof(City));
    }
    
    [AuditableValueField<string>(false)]
    public string State
    {
        get => GetValue<string>(nameof(State)) ?? string.Empty;
        set => SetValue(value, nameof(State));
    }
    
    [AuditableValueField<string>(false)]
    public string ZipCode
    {
        get => GetValue<string>(nameof(ZipCode)) ?? string.Empty;
        set => SetValue(value, nameof(ZipCode));
    }

    public AddressDbo() { }

    public AddressDbo(AggregateRootId aggregateRootId, Ulid entityId)
        : base(aggregateRootId, entityId) { }
}