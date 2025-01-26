using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Events.EntityEvents;

namespace AuditableDomainEntity.UnitTests.EnumerableValueFields.Arrays;

public class IntArrayValueFieldTests
{
    private static readonly int[] ExpectedNonNull = [1, 2, 3];
    private static readonly int[] ExpectedNullable = [4, 5, 6];

    [Fact]
    public void ValueField_Should_HandleIntArrayTypes()
    {
        var intArrayClass = new IntArrayTestClass
        {
            NonNullableIntegerArray = [1, 2, 3]
        };
        
        intArrayClass.FinalizeChanges();
        
        Assert.Equal(ExpectedNonNull, intArrayClass.NonNullableIntegerArray);
        Assert.Null(intArrayClass.NullableIntegerArray);

        var history = intArrayClass.GetEntityChanges();
        
        Assert.Single(history);

        var firstEvent = history[0];
        
        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);
        
        var auditableEntityCreated = firstEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);
        
        intArrayClass.Commit();
        
        var intArrayHistoryClass = AuditableRootEntity.LoadFromHistory<IntArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(intArrayHistoryClass);
        Assert.Equal(ExpectedNonNull, intArrayHistoryClass.NonNullableIntegerArray);
        Assert.Null(intArrayHistoryClass.NullableIntegerArray);

        intArrayHistoryClass.NullableIntegerArray = [4, 5, 6];
        
        intArrayHistoryClass.FinalizeChanges();
        
        Assert.Equal(ExpectedNonNull, intArrayHistoryClass.NonNullableIntegerArray);
        Assert.Equal(ExpectedNullable, intArrayHistoryClass.NullableIntegerArray);
        
        var history2 = intArrayHistoryClass.GetEntityChanges();
        
        Assert.Single(history2);
        
        var firstEvent2 = history2[0];
        
        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);
        
        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated.ValueFieldEvents);
        
        intArrayHistoryClass.Commit();
        history.AddRange(history2);
        var intArrayHistoryClass2 = AuditableRootEntity.LoadFromHistory<IntArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(intArrayHistoryClass2);
        Assert.Equal(ExpectedNonNull, intArrayHistoryClass2.NonNullableIntegerArray);
        Assert.Equal(ExpectedNullable, intArrayHistoryClass2.NullableIntegerArray);
        
        intArrayHistoryClass2.NullableIntegerArray = null;
        
        intArrayHistoryClass2.FinalizeChanges();
        
        Assert.Equal(ExpectedNonNull, intArrayHistoryClass2.NonNullableIntegerArray);
        Assert.Null(intArrayHistoryClass2.NullableIntegerArray);
        
        var history3 = intArrayHistoryClass2.GetEntityChanges();
        
        Assert.Single(history3);
        
        var firstEvent3 = history3[0];
        
        Assert.NotNull(firstEvent3);
        Assert.IsType<AuditableEntityUpdated>(firstEvent3);
        
        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated2);
        Assert.NotNull(auditableEntityUpdated2.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated2.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated2.ValueFieldEvents);
        
        intArrayHistoryClass2.Commit();
        
        history.AddRange(history3);
        
        var intArrayHistoryClass3 = AuditableRootEntity.LoadFromHistory<IntArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(intArrayHistoryClass3);
        Assert.Equal(ExpectedNonNull, intArrayHistoryClass3.NonNullableIntegerArray);
        Assert.Null(intArrayHistoryClass3.NullableIntegerArray);
    }

    [Fact]
    public void ValueField_Should_HandleEmptyArrays()
    {
        var intArrayClass = new IntArrayTestClass
        {
            NonNullableIntegerArray = Array.Empty<int>()
        };

        intArrayClass.FinalizeChanges();

        Assert.Empty(intArrayClass.NonNullableIntegerArray);
        Assert.Null(intArrayClass.NullableIntegerArray);

        var history = intArrayClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);

        intArrayClass.Commit();

        var intArrayHistoryClass =
            AuditableRootEntity.LoadFromHistory<IntArrayTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(intArrayHistoryClass);
        Assert.Empty(intArrayHistoryClass.NonNullableIntegerArray);
        Assert.Null(intArrayHistoryClass.NullableIntegerArray);

        intArrayHistoryClass.NullableIntegerArray = Array.Empty<int>();

        intArrayHistoryClass.FinalizeChanges();

        Assert.Empty(intArrayHistoryClass.NonNullableIntegerArray);
        Assert.Empty(intArrayHistoryClass.NullableIntegerArray);

        var history2 = intArrayHistoryClass.GetEntityChanges();

        Assert.Single(history2);

        var firstEvent2 = history2[0];

        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);

        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated.ValueFieldEvents);

        intArrayHistoryClass.Commit();
        history.AddRange(history2);
        var intArrayHistoryClass2 =
            AuditableRootEntity.LoadFromHistory<IntArrayTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(intArrayHistoryClass2);
        Assert.Empty(intArrayHistoryClass2.NonNullableIntegerArray);
        Assert.Empty(intArrayHistoryClass2.NullableIntegerArray);
    }

    [Fact]
    public void ValueField_Should_HandleNullArrays()
    {
        var intArrayClass = new IntArrayTestClass
        {
            NonNullableIntegerArray = null
        };

        intArrayClass.FinalizeChanges();

        Assert.Null(intArrayClass.NonNullableIntegerArray);
        Assert.Null(intArrayClass.NullableIntegerArray);

        var history = intArrayClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.Empty(auditableEntityCreated.ValueFieldEvents);

        intArrayClass.Commit();

        var intArrayHistoryClass =
            AuditableRootEntity.LoadFromHistory<IntArrayTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(intArrayHistoryClass);
        Assert.Null(intArrayHistoryClass.NonNullableIntegerArray);
        Assert.Null(intArrayHistoryClass.NullableIntegerArray);
    }

    public class IntArrayTestClass : AuditableRootEntity
    {
        [AuditableValueField<int[]>(true)]
        public int[]? NullableIntegerArray
        {
            get => GetValue<int[]?>(nameof(NullableIntegerArray));
            set => SetValue<int[]?>(value, nameof(NullableIntegerArray));
        }
        
        [AuditableValueField<int[]>(false)]
        public int[] NonNullableIntegerArray
        {
            get => GetValue<int[]>(nameof(NonNullableIntegerArray));
            set => SetValue<int[]>(value, nameof(NonNullableIntegerArray));
        }
        
        public IntArrayTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
        public IntArrayTestClass() { }
    }
}