using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Events.EntityEvents;

namespace AuditableDomainEntity.Tests.ValueFields;

public class DateTimeOffsetValueFieldTests
{
    [Fact]
    public void ValueField_Should_HandleDateTimeOffsetTypes()
    {
        var dateTimeOffsetUtcNow = DateTimeOffset.UtcNow;
        var dateTimeOffsetTestClass = new DateTimeOffsetTestClass
        {
            NonNullableDateTimeOffset = dateTimeOffsetUtcNow
        };

        dateTimeOffsetTestClass.FinalizeChanges();

        Assert.Equal(dateTimeOffsetUtcNow, dateTimeOffsetTestClass.NonNullableDateTimeOffset);
        Assert.Null(dateTimeOffsetTestClass.NullableDateTimeOffset);

        var history = dateTimeOffsetTestClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);

        dateTimeOffsetTestClass.Commit();

        var dateTimeOffsetHistoryClass =
            AuditableRootEntity.LoadFromHistory<DateTimeOffsetTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(dateTimeOffsetHistoryClass);
        Assert.Equal(dateTimeOffsetUtcNow, dateTimeOffsetHistoryClass.NonNullableDateTimeOffset);
        Assert.Null(dateTimeOffsetHistoryClass.NullableDateTimeOffset);

        dateTimeOffsetHistoryClass.NullableDateTimeOffset = dateTimeOffsetUtcNow.AddHours(1);

        dateTimeOffsetHistoryClass.FinalizeChanges();

        Assert.Equal(dateTimeOffsetUtcNow, dateTimeOffsetHistoryClass.NonNullableDateTimeOffset);
        Assert.Equal(dateTimeOffsetUtcNow.AddHours(1), dateTimeOffsetHistoryClass.NullableDateTimeOffset);

        var history2 = dateTimeOffsetHistoryClass.GetEntityChanges();

        Assert.Single(history2);

        var firstEvent2 = history2[0];

        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);

        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated.ValueFieldEvents);

        dateTimeOffsetHistoryClass.Commit();

        history.AddRange(history2);
        var dateTimeOffsetHistoryClassTwo =
            AuditableRootEntity.LoadFromHistory<DateTimeOffsetTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(dateTimeOffsetHistoryClassTwo);
        Assert.Equal(dateTimeOffsetUtcNow, dateTimeOffsetHistoryClassTwo.NonNullableDateTimeOffset);
        Assert.Equal(dateTimeOffsetUtcNow.AddHours(1), dateTimeOffsetHistoryClassTwo.NullableDateTimeOffset);

        dateTimeOffsetHistoryClassTwo.NullableDateTimeOffset = null;

        Assert.Null(dateTimeOffsetHistoryClassTwo.NullableDateTimeOffset);

        dateTimeOffsetHistoryClassTwo.FinalizeChanges();
        var history3 = dateTimeOffsetHistoryClassTwo.GetEntityChanges();

        Assert.Single(history3);

        var firstEvent3 = history3[0];

        Assert.NotNull(firstEvent3);
        Assert.IsType<AuditableEntityUpdated>(firstEvent3);

        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated2);
        Assert.NotNull(auditableEntityUpdated2.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated2.ValueFieldEvents);
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
}