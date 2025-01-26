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
        set => SetValue<int?>(value, nameof(Age));
    }
    
    [AuditableValueListField<string>(true)]
    public AuditableValueList<string>? Tags
    {
        get => GetValueList<string>(nameof(Tags));
        set => SetValueList<string>(value, nameof(Tags));
    }
    
    [AuditableEntityListField<AddressDbo>(true)]
    public AuditableEntityList<AddressDbo> Addresses
    {
        get => GetEntityList<AddressDbo>(nameof(Addresses));
        set => SetEntityList<AddressDbo>(value, nameof(Addresses));
    }
    
    [AuditableEntityListField<EmailDbo>(true)]
    public AuditableEntityList<EmailDbo> Emails
    {
        get => GetEntityList<EmailDbo>(nameof(Emails));
        set => SetEntityList<EmailDbo>(value, nameof(Emails));
    }
    
    public PersonDbo(AggregateRootId aggregateRootId)
        : base(aggregateRootId)
    {
    }
}