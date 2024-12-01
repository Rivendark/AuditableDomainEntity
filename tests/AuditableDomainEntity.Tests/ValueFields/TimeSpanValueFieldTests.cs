using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Events.EntityEvents;

namespace AuditableDomainEntity.Tests.ValueFields;

public class TimeSpanValueFieldTests
{
     [Fact]
    public void ValueField_Should_HandleTimeSpanTypes()
    {
        var timeSpanTestClass = new TimeSpanTestClass
        {
            NonNullableTimeSpan = TimeSpan.FromHours(1)
        };

        timeSpanTestClass.FinalizeChanges();

        Assert.Equal(TimeSpan.FromHours(1), timeSpanTestClass.NonNullableTimeSpan);
        Assert.Null(timeSpanTestClass.NullableTimeSpan);

        var history = timeSpanTestClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);

        timeSpanTestClass.Commit();

        var timeSpanHistoryClass =
            AuditableRootEntity.LoadFromHistory<TimeSpanTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(timeSpanHistoryClass);
        Assert.Equal(TimeSpan.FromHours(1), timeSpanHistoryClass.NonNullableTimeSpan);
        Assert.Null(timeSpanHistoryClass.NullableTimeSpan);

        timeSpanHistoryClass.NullableTimeSpan = TimeSpan.FromHours(2);

        timeSpanHistoryClass.FinalizeChanges();

        Assert.Equal(TimeSpan.FromHours(1), timeSpanHistoryClass.NonNullableTimeSpan);
        Assert.Equal(TimeSpan.FromHours(2), timeSpanHistoryClass.NullableTimeSpan);

        var history2 = timeSpanHistoryClass.GetEntityChanges();

        Assert.Single(history2);

        var firstEvent2 = history2[0];

        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);

        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated.ValueFieldEvents);

        timeSpanHistoryClass.Commit();

        history.AddRange(history2);
        var timeSpanHistoryClassTwo =
            AuditableRootEntity.LoadFromHistory<TimeSpanTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(timeSpanHistoryClassTwo);
        Assert.Equal(TimeSpan.FromHours(1), timeSpanHistoryClassTwo.NonNullableTimeSpan);
        Assert.Equal(TimeSpan.FromHours(2), timeSpanHistoryClassTwo.NullableTimeSpan);

        timeSpanHistoryClassTwo.NullableTimeSpan = null;

        Assert.Null(timeSpanHistoryClassTwo.NullableTimeSpan);

        timeSpanHistoryClassTwo.FinalizeChanges();
        var history3 = timeSpanHistoryClassTwo.GetEntityChanges();

        Assert.Single(history3);

        var firstEvent3 = history3[0];

        Assert.NotNull(firstEvent3);
        Assert.IsType<AuditableEntityUpdated>(firstEvent3);

        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated2);
        Assert.NotNull(auditableEntityUpdated2.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated2.ValueFieldEvents);
    }
    
    public class TimeSpanTestClass : AuditableRootEntity
    {
        [AuditableValueField<TimeSpan?>(true)]
        public TimeSpan? NullableTimeSpan
        {
            get => GetValue<TimeSpan?>(nameof(NullableTimeSpan));
            set => SetValue<TimeSpan?>(value, nameof(NullableTimeSpan));
        }

        [AuditableValueField<TimeSpan>(false)]
        public TimeSpan NonNullableTimeSpan
        {
            get => GetValue<TimeSpan>(nameof(NonNullableTimeSpan));
            set => SetValue(value, nameof(NonNullableTimeSpan));
        }
    
        public TimeSpanTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
        public TimeSpanTestClass() { }
    }
}