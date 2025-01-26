using AuditableDomainEntity.Attributes;

namespace AuditableDomainEntity.FunctionalTests.PersonTests;

public class EmailDbo : AuditableEntity
{
    [AuditableValueField<string>(false)]
    public string? EmailAddress
    {
        get => GetValue<string?>(nameof(EmailAddress));
        set => SetValue<string?>(value, nameof(EmailAddress));
    }

    public EmailDbo() { }

    public EmailDbo(AggregateRootId aggregateRootId, Ulid entityId)
        : base(aggregateRootId, entityId) { }
}