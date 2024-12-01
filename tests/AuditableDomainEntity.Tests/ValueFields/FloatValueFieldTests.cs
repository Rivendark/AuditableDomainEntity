using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Events.EntityEvents;

namespace AuditableDomainEntity.Tests.ValueFields;

public class FloatValueFieldTests
{
    [Fact]
    public void ValueField_Should_HandleFloatTypes()
    {
        var floatTestClass = new FloatTestClass
        {
            NonNullableFloat = 12.0f
        };

        floatTestClass.FinalizeChanges();

        Assert.Equal(12.0f, floatTestClass.NonNullableFloat);
        Assert.Null(floatTestClass.NullableFloat);

        var history = floatTestClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);

        floatTestClass.Commit();

        var floatHistoryClass = AuditableRootEntity.LoadFromHistory<FloatTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(floatHistoryClass);
        Assert.Equal(12.0f, floatHistoryClass.NonNullableFloat);
        Assert.Null(floatHistoryClass.NullableFloat);

        floatHistoryClass.NullableFloat = 24.0f;

        floatHistoryClass.FinalizeChanges();

        Assert.Equal(12.0f, floatHistoryClass.NonNullableFloat);
        Assert.Equal(24.0f, floatHistoryClass.NullableFloat);

        var history2 = floatHistoryClass.GetEntityChanges();

        Assert.Single(history2);

        var firstEvent2 = history2[0];

        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);

        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);

        floatHistoryClass.Commit();

        history.AddRange(history2);
        var floatHistoryClassTwo =
            AuditableRootEntity.LoadFromHistory<FloatTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(floatHistoryClassTwo);
        Assert.Equal(12.0f, floatHistoryClassTwo.NonNullableFloat);
        Assert.Equal(24.0f, floatHistoryClassTwo.NullableFloat);

        floatHistoryClassTwo.NullableFloat = null;

        Assert.Null(floatHistoryClassTwo.NullableFloat);

        floatHistoryClassTwo.FinalizeChanges();
        var history3 = floatHistoryClassTwo.GetEntityChanges();

        Assert.Single(history3);

        var firstEvent3 = history3[0];

        Assert.NotNull(firstEvent3);
        Assert.IsType<AuditableEntityUpdated>(firstEvent3);

        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated2);
        Assert.NotNull(auditableEntityUpdated2.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated2.ValueFieldEvents);

        floatHistoryClassTwo.Commit();

        history.AddRange(history3);
        var floatHistoryClassThree =
            AuditableRootEntity.LoadFromHistory<FloatTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(floatHistoryClassThree);
        Assert.Equal(12.0f, floatHistoryClassThree.NonNullableFloat);
        Assert.Null(floatHistoryClassThree.NullableFloat);
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
        public float NonNullableFloat
        {
            get => GetValue<float>(nameof(NonNullableFloat));
            set => SetValue(value, nameof(NonNullableFloat));
        }
    
        public FloatTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
        public FloatTestClass() { }
    }
}