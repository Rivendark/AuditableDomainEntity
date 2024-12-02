using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Events.EntityEvents;

namespace AuditableDomainEntity.Tests.EnumerableValueFields.Arrays;

public class DoubleArrayValueFieldTests
{
    private static readonly double[] ExpectedNonNull = [1.1, 2.2, 3.3];
    private static readonly double[] ExpectedNullable = [4.4, 5.5, 6.6];

    [Fact]
    public void ValueField_Should_HandleDoubleArrayTypes()
    {
        var doubleArrayClass = new DoubleArrayTestClass
        {
            NonNullableDoubleArray = [1.1, 2.2, 3.3]
        };

        doubleArrayClass.FinalizeChanges();

        Assert.Equal(ExpectedNonNull, doubleArrayClass.NonNullableDoubleArray);
        Assert.Null(doubleArrayClass.NullableDoubleArray);

        var history = doubleArrayClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);

        doubleArrayClass.Commit();

        var doubleArrayHistoryClass =
            AuditableRootEntity.LoadFromHistory<DoubleArrayTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(doubleArrayHistoryClass);
        Assert.Equal(ExpectedNonNull, doubleArrayHistoryClass.NonNullableDoubleArray);
        Assert.Null(doubleArrayHistoryClass.NullableDoubleArray);
        
        doubleArrayHistoryClass.NullableDoubleArray = [4.4, 5.5, 6.6];
        
        doubleArrayHistoryClass.FinalizeChanges();
        
        Assert.Equal(ExpectedNonNull, doubleArrayHistoryClass.NonNullableDoubleArray);
        Assert.Equal(ExpectedNullable, doubleArrayHistoryClass.NullableDoubleArray);
        
        var history2 = doubleArrayHistoryClass.GetEntityChanges();
        
        Assert.Single(history2);
        
        var firstEvent2 = history2[0];
        
        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);
        
        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated);
        
        var valueFieldEvents = auditableEntityUpdated.ValueFieldEvents;
        
        Assert.NotNull(valueFieldEvents);
        Assert.NotEmpty(valueFieldEvents);
        Assert.Single(valueFieldEvents);
        
        doubleArrayHistoryClass.Commit();
        
        history.AddRange(history2);
        var doubleArrayHistoryClass2 = AuditableRootEntity.LoadFromHistory<DoubleArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(doubleArrayHistoryClass2);
        Assert.Equal(ExpectedNonNull, doubleArrayHistoryClass2.NonNullableDoubleArray);
        Assert.Equal(ExpectedNullable, doubleArrayHistoryClass2.NullableDoubleArray);
        
        doubleArrayHistoryClass2.NullableDoubleArray = null;
        
        doubleArrayHistoryClass2.FinalizeChanges();
        
        Assert.Equal(ExpectedNonNull, doubleArrayHistoryClass2.NonNullableDoubleArray);
        Assert.Null(doubleArrayHistoryClass2.NullableDoubleArray);
        
        var history3 = doubleArrayHistoryClass2.GetEntityChanges();
        
        Assert.Single(history3);
        
        var firstEvent3 = history3[0];
        
        Assert.NotNull(firstEvent3);
        Assert.IsType<AuditableEntityUpdated>(firstEvent3);
        
        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated2);
        Assert.NotNull(auditableEntityUpdated2.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated2.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated2.ValueFieldEvents);
        
        doubleArrayHistoryClass2.Commit();
        
        history.AddRange(history3);
        
        var doubleArrayHistoryClass3 = AuditableRootEntity.LoadFromHistory<DoubleArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(doubleArrayHistoryClass3);
        Assert.Equal(ExpectedNonNull, doubleArrayHistoryClass3.NonNullableDoubleArray);
        Assert.Null(doubleArrayHistoryClass3.NullableDoubleArray);
    }

    [Fact]
    public void ValueField_Should_HandleEmptyArrays()
    {
        var doubleArrayClass = new DoubleArrayTestClass
        {
            NonNullableDoubleArray = []
        };

        doubleArrayClass.FinalizeChanges();

        Assert.Empty(doubleArrayClass.NonNullableDoubleArray);
        Assert.Null(doubleArrayClass.NullableDoubleArray);
        
        var history = doubleArrayClass.GetEntityChanges();
        
        Assert.Single(history);
        
        var fristEvent = history[0];
        
        Assert.NotNull(fristEvent);
        Assert.IsType<AuditableEntityCreated>(fristEvent);
        
        var auditableEntityCreated = fristEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);
        
        doubleArrayClass.Commit();
        
        var doubleArrayHistoryClass =
            AuditableRootEntity.LoadFromHistory<DoubleArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(doubleArrayHistoryClass);
        Assert.NotNull(doubleArrayHistoryClass.NonNullableDoubleArray);
        Assert.Empty(doubleArrayHistoryClass.NonNullableDoubleArray);
    }

    [Fact]
    public void ValueField_Should_HandleNullArrays()
    {
        var doubleArrayClass = new DoubleArrayTestClass
        {
            NonNullableDoubleArray = null
        };

        doubleArrayClass.FinalizeChanges();

        Assert.Null(doubleArrayClass.NonNullableDoubleArray);
        Assert.Null(doubleArrayClass.NullableDoubleArray);

        var history = doubleArrayClass.GetEntityChanges();

        Assert.Single(history);

        var fristEvent = history[0];

        Assert.NotNull(fristEvent);
        Assert.IsType<AuditableEntityCreated>(fristEvent);

        var auditableEntityCreated = fristEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.Empty(auditableEntityCreated.ValueFieldEvents);

        doubleArrayClass.Commit();

        var doubleArrayHistoryClass =
            AuditableRootEntity.LoadFromHistory<DoubleArrayTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(doubleArrayHistoryClass);
        Assert.Null(doubleArrayHistoryClass.NonNullableDoubleArray);
        Assert.Null(doubleArrayHistoryClass.NullableDoubleArray);
    }

    public class DoubleArrayTestClass : AuditableRootEntity
    {
        [AuditableValueField<double[]>(true)]
        public double[]? NullableDoubleArray
        {
            get => GetValue<double[]?>(nameof(NullableDoubleArray));
            set => SetValue<double[]?>(value, nameof(NullableDoubleArray));
        }
        
        [AuditableValueField<double[]>(false)]
        public double[] NonNullableDoubleArray
        {
            get => GetValue<double[]>(nameof(NonNullableDoubleArray));
            set => SetValue<double[]>(value, nameof(NonNullableDoubleArray));
        }
        
        public DoubleArrayTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
        public DoubleArrayTestClass() { }
    }
}