using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Events.EntityEvents;

namespace AuditableDomainEntity.Tests.EnumerableValueFields.Arrays;

public class FloatArrayValueFieldTests
{
    private static readonly float[] ExpectedNonNull = [1.1f, 2.2f, 3.3f];
    private static readonly float[] ExpectedNullable = [4.4f, 5.5f, 6.6f];
    private static readonly float[] ExpectedNullable2 = [7.7f, 8.8f, 9.9f];
    
    [Fact]
    public void ValueField_Should_HandleFloatArrayTypes()
    {
        var floatArrayClass = new FloatArrayTestClass
        {
            NonNullableFloatArray = [1.1f, 2.2f, 3.3f]
        };

        floatArrayClass.FinalizeChanges();

        Assert.Equal(ExpectedNonNull, floatArrayClass.NonNullableFloatArray);
        Assert.Null(floatArrayClass.NullableFloatArray);

        var history = floatArrayClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);

        floatArrayClass.Commit();

        var floatArrayHistoryClass =
            AuditableRootEntity.LoadFromHistory<FloatArrayTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(floatArrayHistoryClass);
        Assert.Equal(ExpectedNonNull, floatArrayHistoryClass.NonNullableFloatArray);
        Assert.Null(floatArrayHistoryClass.NullableFloatArray);
        
        floatArrayHistoryClass.NullableFloatArray = [4.4f, 5.5f, 6.6f];
        
        floatArrayHistoryClass.FinalizeChanges();
        
        Assert.Equal(ExpectedNonNull, floatArrayHistoryClass.NonNullableFloatArray);
        Assert.Equal(ExpectedNullable, floatArrayHistoryClass.NullableFloatArray);
        
        var history2 = floatArrayHistoryClass.GetEntityChanges();
        
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
        
        floatArrayHistoryClass.Commit();
        
        history.AddRange(history2);
        
        var floatArrayHistoryClass2 =
            AuditableRootEntity.LoadFromHistory<FloatArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(floatArrayHistoryClass2);
        Assert.Equal(ExpectedNonNull, floatArrayHistoryClass2.NonNullableFloatArray);
        Assert.Equal(ExpectedNullable, floatArrayHistoryClass2.NullableFloatArray);
        
        floatArrayHistoryClass2.NullableFloatArray = [7.7f, 8.8f, 9.9f];

        floatArrayHistoryClass2.FinalizeChanges();
        
        var history3 = floatArrayHistoryClass2.GetEntityChanges();
        
        Assert.Single(history3);
        
        var firstEvent3 = history3[0];
        
        Assert.NotNull(firstEvent3);
        Assert.IsType<AuditableEntityUpdated>(firstEvent3);
        
        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated2);
        
        var valueFieldEvents2 = auditableEntityUpdated2.ValueFieldEvents;
        
        Assert.NotNull(valueFieldEvents2);
        Assert.NotEmpty(valueFieldEvents2);
        Assert.Single(valueFieldEvents2);
        
        floatArrayHistoryClass2.Commit();
        
        history.AddRange(history3);
        var floatArrayHistoryClass3 =
            AuditableRootEntity.LoadFromHistory<FloatArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(floatArrayHistoryClass3);
        Assert.Equal(ExpectedNonNull, floatArrayHistoryClass3.NonNullableFloatArray);
        Assert.Equal(ExpectedNullable2, floatArrayHistoryClass3.NullableFloatArray);
        
        floatArrayHistoryClass3.NullableFloatArray = null;
        
        Assert.Null(floatArrayHistoryClass3.NullableFloatArray);
        
        floatArrayHistoryClass3.FinalizeChanges();
        var history4 = floatArrayHistoryClass3.GetEntityChanges();
        
        Assert.Single(history4);
        
        var firstEvent4 = history4[0];
        
        Assert.NotNull(firstEvent4);
        Assert.IsType<AuditableEntityUpdated>(firstEvent4);
        
        var auditableEntityUpdated3 = firstEvent4 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated3);
        Assert.NotNull(auditableEntityUpdated3.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated3.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated3.ValueFieldEvents);
    }
    
    [Fact]
    public void ValueField_Should_HandleEmptyArrays()
    {
        var floatArrayClass = new FloatArrayTestClass
        {
            NonNullableFloatArray = []
        };

        floatArrayClass.FinalizeChanges();

        Assert.Empty(floatArrayClass.NonNullableFloatArray);
        Assert.Null(floatArrayClass.NullableFloatArray);

        var history = floatArrayClass.GetEntityChanges();

        Assert.Single(history);
        
        var firstEvent = history[0];
        
        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);
        
        var auditableEntityCreated = firstEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);
        
        floatArrayClass.Commit();
        
        var floatArrayHistoryClass =
            AuditableRootEntity.LoadFromHistory<FloatArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(floatArrayHistoryClass);
        Assert.Empty(floatArrayHistoryClass.NonNullableFloatArray);
        Assert.Null(floatArrayHistoryClass.NullableFloatArray);
    }

    [Fact]
    public void ValueField_Should_HandleNullArrays()
    {
        var floatArrayClass = new FloatArrayTestClass
        {
            NonNullableFloatArray = null
        };
        
        floatArrayClass.FinalizeChanges();
        
        Assert.Null(floatArrayClass.NonNullableFloatArray);
        Assert.Null(floatArrayClass.NullableFloatArray);
        
        var history = floatArrayClass.GetEntityChanges();
        
        Assert.Single(history);
        
        var firstEvent = history[0];
        
        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);
        
        var auditableEntityCreated = firstEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.Empty(auditableEntityCreated.ValueFieldEvents);
        
        floatArrayClass.Commit();
        
        var floatArrayHistoryClass =
            AuditableRootEntity.LoadFromHistory<FloatArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(floatArrayHistoryClass);
        Assert.Null(floatArrayHistoryClass.NonNullableFloatArray);
        Assert.Null(floatArrayHistoryClass.NullableFloatArray);
    }

    public class FloatArrayTestClass : AuditableRootEntity
    {
        [AuditableValueField<float[]>(true)]
        public float[]? NullableFloatArray
        {
            get => GetValue<float[]?>(nameof(NullableFloatArray));
            set => SetValue<float[]?>(value, nameof(NullableFloatArray));
        }
        
        [AuditableValueField<float[]>(false)]
        public float[] NonNullableFloatArray
        {
            get => GetValue<float[]>(nameof(NonNullableFloatArray));
            set => SetValue<float[]>(value, nameof(NonNullableFloatArray));
        }
        
        public FloatArrayTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
        public FloatArrayTestClass() { }
    }
}