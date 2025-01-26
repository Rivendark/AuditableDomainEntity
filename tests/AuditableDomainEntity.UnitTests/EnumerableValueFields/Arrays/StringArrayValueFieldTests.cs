using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Events.EntityEvents;

namespace AuditableDomainEntity.UnitTests.EnumerableValueFields.Arrays;

public class StringArrayValueFieldTests
{
    private static readonly string[] ExpectedNonNull = ["1", "2", "3"];
    private static readonly string[] ExpectedNullable = ["4", "5", "6"];
    private static readonly string[] ExpectedNullable2 = ["7", "8", "9"];

    [Fact]
    public void ValueField_Should_HandleStringArrayTypes()
    {
        var stringArrayClass = new StringArrayTestClass
        {
            NonNullableStringArray = ExpectedNonNull
        };

        stringArrayClass.FinalizeChanges();

        Assert.Equal(ExpectedNonNull, stringArrayClass.NonNullableStringArray);
        Assert.Null(stringArrayClass.NullableStringArray);
        
        var history = stringArrayClass.GetEntityChanges();
        
        Assert.Single(history);
        
        var firstEvent = history[0];
        
        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);
        
        var auditableEntityCreated = firstEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);
        
        stringArrayClass.Commit();
        
        var stringArrayHistoryClass =
            AuditableRootEntity.LoadFromHistory<StringArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(stringArrayHistoryClass);
        Assert.Equal(ExpectedNonNull, stringArrayHistoryClass.NonNullableStringArray);
        Assert.Null(stringArrayHistoryClass.NullableStringArray);
        
        stringArrayHistoryClass.NullableStringArray = ExpectedNullable;
        
        stringArrayHistoryClass.FinalizeChanges();
        
        Assert.Equal(ExpectedNonNull, stringArrayHistoryClass.NonNullableStringArray);
        Assert.Equal(ExpectedNullable, stringArrayHistoryClass.NullableStringArray);
        
        var history2 = stringArrayHistoryClass.GetEntityChanges();
        history.AddRange(history2);
        Assert.Single(history2);
        
        var firstEvent2 = history2[0];
        
        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);
        
        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);
        
        stringArrayHistoryClass.NullableStringArray = null;
        
        stringArrayHistoryClass.FinalizeChanges();
        
        var history3 = stringArrayHistoryClass.GetEntityChanges();
        
        stringArrayHistoryClass.Commit();
        history.AddRange(history3);
        
        var stringArrayHistoryClass2 = AuditableRootEntity.LoadFromHistory<StringArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(stringArrayHistoryClass2);
        Assert.Equal(ExpectedNonNull, stringArrayHistoryClass2.NonNullableStringArray);
        Assert.Null(stringArrayHistoryClass2.NullableStringArray);
        
        stringArrayHistoryClass2.NullableStringArray = ExpectedNullable2;
        
        stringArrayHistoryClass2.FinalizeChanges();
        
        Assert.Equal(ExpectedNonNull, stringArrayHistoryClass2.NonNullableStringArray);
        Assert.Equal(ExpectedNullable2, stringArrayHistoryClass2.NullableStringArray);
        
        var history4 = stringArrayHistoryClass2.GetEntityChanges();
        
        Assert.Single(history4);
        
        var firstEvent3 = history4[0];
        
        Assert.NotNull(firstEvent3);
        Assert.IsType<AuditableEntityUpdated>(firstEvent3);
        
        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated2);
        Assert.NotNull(auditableEntityUpdated2.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated2.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated2.ValueFieldEvents);
    }

    [Fact]
    public void ValueField_Should_HandleEmptyArrays()
    {
        var stringArrayClass = new StringArrayTestClass
        {
            NonNullableStringArray = Array.Empty<string>()
        };

        stringArrayClass.FinalizeChanges();

        Assert.Empty(stringArrayClass.NonNullableStringArray);
        Assert.Null(stringArrayClass.NullableStringArray);
        
        var history = stringArrayClass.GetEntityChanges();
        
        Assert.Single(history);
        
        var firstEvent = history[0];
        
        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);
        
        var auditableEntityCreated = firstEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);
        
        stringArrayClass.Commit();
        
        var stringArrayHistoryClass =
            AuditableRootEntity.LoadFromHistory<StringArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(stringArrayHistoryClass);
        Assert.Empty(stringArrayHistoryClass.NonNullableStringArray);
        Assert.Null(stringArrayHistoryClass.NullableStringArray);
    }

    [Fact]
    public void ValueField_Should_HandleNullArrays()
    {
        var stringArrayClass = new StringArrayTestClass
        {
            NonNullableStringArray = null
        };

        stringArrayClass.FinalizeChanges();

        Assert.Null(stringArrayClass.NonNullableStringArray);
        Assert.Null(stringArrayClass.NullableStringArray);

        var history = stringArrayClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.Empty(auditableEntityCreated.ValueFieldEvents);

        stringArrayClass.Commit();

        var stringArrayHistoryClass =
            AuditableRootEntity.LoadFromHistory<StringArrayTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(stringArrayHistoryClass);
        Assert.Null(stringArrayHistoryClass.NonNullableStringArray);
        Assert.Null(stringArrayHistoryClass.NullableStringArray);
    }

    public class StringArrayTestClass : AuditableRootEntity
    {
        [AuditableValueField<string[]>(true)]
        public string[]? NullableStringArray
        {
            get => GetValue<string[]?>(nameof(NullableStringArray));
            set => SetValue<string[]?>(value, nameof(NullableStringArray));
        }
        
        [AuditableValueField<string[]>(false)]
        public string[] NonNullableStringArray
        {
            get => GetValue<string[]>(nameof(NonNullableStringArray));
            set => SetValue<string[]>(value, nameof(NonNullableStringArray));
        }
        
        public StringArrayTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
        public StringArrayTestClass() { }
    }
}