using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Collections.Lists;

namespace AuditableDomainEntity.FunctionalTests.PersonTests;

public class PersonDbo : AuditableRootEntity
{
    [AuditableValueField<string>(false)]
    public string? FirstName
    {
        get => GetValue<string?>(nameof(FirstName));
        set => SetValue<string?>(value, nameof(FirstName));
    }
    
    [AuditableValueField<string>(false)]
    public string? LastName
    {
        get => GetValue<string?>(nameof(LastName));
        set => SetValue<string?>(value, nameof(LastName));
    }
    
    [AuditableValueField<int>(false)]
    public int? Age
    {
        get => GetValue<int?>(nameof(Age));
        set => SetValue(value, nameof(Age));
    }
    
    [AuditableEntityField<AddressDbo>(true)]
    public AddressDbo? PrimaryAddress
    {
        get => GetEntity<AddressDbo?>(nameof(PrimaryAddress));
        set => SetEntity<AddressDbo?>(value, nameof(PrimaryAddress));
    }
    
    [AuditableValueListField<string>(true)]
    public AuditableValueList<string>? Tags
    {
        get => GetValueList<string>(nameof(Tags));
        set => SetValueList(value, nameof(Tags));
    }
    
    [AuditableEntityListField<AddressDbo>(true)]
    public AuditableEntityList<AddressDbo> Addresses
    {
        get => GetEntityList<AddressDbo>(nameof(Addresses));
        set => SetEntityList(value, nameof(Addresses));
    }
    
    [AuditableEntityListField<EmailDbo>(true)]
    public AuditableEntityList<EmailDbo> Emails
    {
        get => GetEntityList<EmailDbo>(nameof(Emails));
        set => SetEntityList(value, nameof(Emails));
    }
    
    public PersonDbo(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
    
    public PersonDbo() : base(new AggregateRootId(Ulid.NewUlid(), typeof(PersonDbo))) { }
}