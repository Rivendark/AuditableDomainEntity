using AuditableDomainEntity.Attributes;

namespace AuditableDomainEntity.Tests;

public class ValueTypeTests
{
    
}

public class IntTestClass : AuditableRootEntity
{
    [AuditableValueField<int?>(true)]
    public int? NullableInteger
    {
        get => GetValue<int?>(nameof(NullableInteger));
        set => SetValue<int?>(value, nameof(NullableInteger));
    }
    
    [AuditableValueField<int>(false)]
    public int NonNullableInteger
    {
        get => GetValue<int>(nameof(NonNullableInteger));
        set => SetValue<int>(value, nameof(NonNullableInteger));
    }
    
    public IntTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
    public IntTestClass() { }
}

public class LongTestClass : AuditableRootEntity
{
    [AuditableValueField<long?>(true)]
    public long? NullableLong
    {
        get => GetValue<long?>(nameof(NullableLong));
        set => SetValue<long?>(value, nameof(NullableLong));
    }

    [AuditableValueField<long>(false)]
    public long? NonNullableLong
    {
        get => GetValue<long>(nameof(NonNullableLong));
        set => SetValue(value, nameof(NonNullableLong));
    }
    
    public LongTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
    public LongTestClass() { }
}

public class DoubleTestClass : AuditableRootEntity
{
    [AuditableValueField<double?>(true)]
    public double? NullableDouble
    {
        get => GetValue<double?>(nameof(NullableDouble));
        set => SetValue<double?>(value, nameof(NullableDouble));
    }

    [AuditableValueField<double>(false)]
    public double? NonNullableDouble
    {
        get => GetValue<double>(nameof(NonNullableDouble));
        set => SetValue(value, nameof(NonNullableDouble));
    }
    
    public DoubleTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
    public DoubleTestClass() { }
}

public class FloatTestClass : AuditableRootEntity
{
    [AuditableValueField<float?>(true)]
    public float? NullableFloat
    {
        get => GetValue<float?>(nameof(NullableFloat));
        set => SetValue<float?>(value, nameof(NullableFloat));
    }

    [AuditableValueField<float>(false)]
    public float? NonNullableFloat
    {
        get => GetValue<float>(nameof(NonNullableFloat));
        set => SetValue(value, nameof(NonNullableFloat));
    }
    
    public FloatTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
    public FloatTestClass() { }
}

public class DecimalTestClass : AuditableRootEntity
{
    [AuditableValueField<decimal?>(true)]
    public decimal? NullableDecimal
    {
        get => GetValue<decimal?>(nameof(NullableDecimal));
        set => SetValue<decimal?>(value, nameof(NullableDecimal));
    }

    [AuditableValueField<decimal>(false)]
    public decimal? NonNullableDecimal
    {
        get => GetValue<decimal>(nameof(NonNullableDecimal));
        set => SetValue(value, nameof(NonNullableDecimal));
    }
    
    public DecimalTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
    public DecimalTestClass() { }
}

public class StringTestClass : AuditableRootEntity
{
    [AuditableValueField<string>(true)]
    public string NullableString
    {
        get => GetValue<string?>(nameof(NullableString));
        set => SetValue<string?>(value, nameof(NullableString));
    }

    [AuditableValueField<string>(false)]
    public string NonNullableString
    {
        get => GetValue<string>(nameof(NonNullableString));
        set => SetValue<string>(value, nameof(NonNullableString));
    }
    
    public StringTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
    public StringTestClass() { }
}

public enum TestBasicEnum
{
    Value1,
    Value2,
}

public enum TestStringEnum
{
    Value1 = 'A',
    Value2 = 'B',
}

public class EnumTestClass : AuditableRootEntity
{
    [AuditableValueField<TestBasicEnum>(true)]
    public TestBasicEnum? NullableEnum
    {
        get => GetValue<TestBasicEnum?>(nameof(NullableEnum));
        set => SetValue<TestBasicEnum?>(value, nameof(NullableEnum));
    }

    [AuditableValueField<TestBasicEnum>(false)]
    public TestBasicEnum? NonNullableEnum
    {
        get => GetValue<TestBasicEnum>(nameof(NonNullableEnum));
        set => SetValue(value, nameof(NonNullableEnum));
    }

    [AuditableValueField<TestStringEnum>(true)]
    public TestStringEnum? NullableNonNullableEnum
    {
        get => GetValue<TestStringEnum?>(nameof(NullableNonNullableEnum));
        set => SetValue(value, nameof(NullableNonNullableEnum));
    }

    [AuditableValueField<TestStringEnum>(false)]
    public TestStringEnum? NonNullableNonNullableEnum
    {
        get => GetValue<TestStringEnum>(nameof(NonNullableNonNullableEnum));
        set => SetValue(value, nameof(NonNullableNonNullableEnum));
    }
    
    public EnumTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
    public EnumTestClass() { }
}

public class DateTimeTestClass : AuditableRootEntity
{
    [AuditableValueField<DateTime>(true)]
    public DateTime? NullableDateTime
    {
        get => GetValue<DateTime?>(nameof(NullableDateTime));
        set => SetValue<DateTime?>(value, nameof(NullableDateTime));
    }

    [AuditableValueField<DateTime>(false)]
    public DateTime NonNullableDateTime
    {
        get => GetValue<DateTime>(nameof(NonNullableDateTime));
        set => SetValue(value, nameof(NonNullableDateTime));
    }
    
    public DateTimeTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
    public DateTimeTestClass() { }
}

public class DateTimeOffsetTestClass : AuditableRootEntity
{
    [AuditableValueField<DateTimeOffset>(true)]
    public DateTimeOffset? NullableDateTimeOffset
    {
        get => GetValue<DateTimeOffset?>(nameof(NullableDateTimeOffset));
        set => SetValue<DateTimeOffset?>(value, nameof(NullableDateTimeOffset));
    }

    [AuditableValueField<DateTimeOffset>(false)]
    public DateTimeOffset NonNullableDateTimeOffset
    {
        get => GetValue<DateTimeOffset>(nameof(NonNullableDateTimeOffset));
        set => SetValue(value, nameof(NonNullableDateTimeOffset));
    }
    
    public DateTimeOffsetTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
    public DateTimeOffsetTestClass() { }
}