using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Events.EntityEvents;

namespace AuditableDomainEntity.Tests.ValueFields;

public class DoubleValueFieldTests
{
    [Fact]
    public void ValueField_Should_HandleDoubleTypes()
    {
        var doubleTestClass = new DoubleTestClass
        {
            NonNullableDouble = 12.0
        };

        doubleTestClass.FinalizeChanges();

        Assert.Equal(12.0, doubleTestClass.NonNullableDouble);
        Assert.Null(doubleTestClass.NullableDouble);

        var history = doubleTestClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);

        doubleTestClass.Commit();

        var doubleHistoryClass =
            AuditableRootEntity.LoadFromHistory<DoubleTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(doubleHistoryClass);
        Assert.Equal(12.0, doubleHistoryClass.NonNullableDouble);
        Assert.Null(doubleHistoryClass.NullableDouble);

        doubleHistoryClass.NullableDouble = 24.0;

        doubleHistoryClass.FinalizeChanges();

        Assert.Equal(12.0, doubleHistoryClass.NonNullableDouble);
        Assert.Equal(24.0, doubleHistoryClass.NullableDouble);

        var history2 = doubleHistoryClass.GetEntityChanges();

        Assert.Single(history2);

        var firstEvent2 = history2[0];

        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);

        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated.ValueFieldEvents);

        doubleTestClass.Commit();

        history.AddRange(history2);
        var doubleHistoryClassTwo =
            AuditableRootEntity.LoadFromHistory<DoubleTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(doubleHistoryClassTwo);
        Assert.Equal(12.0, doubleHistoryClassTwo.NonNullableDouble);
        Assert.Equal(24.0, doubleHistoryClassTwo.NullableDouble);

        doubleHistoryClassTwo.NullableDouble = null;

        Assert.Null(doubleHistoryClassTwo.NullableDouble);

        doubleHistoryClassTwo.FinalizeChanges();
        var history3 = doubleHistoryClassTwo.GetEntityChanges();

        Assert.Single(history3);

        var firstEvent3 = history3[0];

        Assert.NotNull(firstEvent3);
        Assert.IsType<AuditableEntityUpdated>(firstEvent3);

        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated2);
        Assert.NotNull(auditableEntityUpdated2.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated2.ValueFieldEvents);

        doubleHistoryClassTwo.Commit();

        history.AddRange(history3);
        var doubleHistoryClassThree =
            AuditableRootEntity.LoadFromHistory<DoubleTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(doubleHistoryClassThree);
        Assert.Equal(12.0, doubleHistoryClassThree.NonNullableDouble);
        Assert.Null(doubleHistoryClassThree.NullableDouble);
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
        public double NonNullableDouble
        {
            get => GetValue<double>(nameof(NonNullableDouble));
            set => SetValue(value, nameof(NonNullableDouble));
        }
    
        public DoubleTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
        public DoubleTestClass() { }
    }
}