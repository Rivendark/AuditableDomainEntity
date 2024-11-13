namespace AuditableDomainEntity;

public class TestRootEntity : AuditableDomainRootEntity
{
    [AuditableField<int?>(nameof(Number), true)]
    public int? Number
    {
        get => GetValue<int?>(nameof(Number));
        set => SetValue<int?>(value, nameof(Number));
    }

    [AuditableField<DateTime?>(nameof(Date),true)]
    public DateTime? Date
    {
        get => GetValue<DateTime?>(nameof(Date));
        set => SetValue<DateTime?>(value, nameof(Date));
    }
}