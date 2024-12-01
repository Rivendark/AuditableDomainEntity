using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Events.EntityEvents;

namespace AuditableDomainEntity.Tests.ValueFields;

public class DateTimeValueFieldTests
{
    [Fact]
    public void ValueField_Should_HandleDateTimeTypes()
    {
        var dateTimeUtcNow = DateTime.UtcNow;
        var dateTimeTestClass = new DateTimeTestClass
        {
            NonNullableDateTime = dateTimeUtcNow
        };

        dateTimeTestClass.FinalizeChanges();

        Assert.Equal(dateTimeUtcNow, dateTimeTestClass.NonNullableDateTime);
        Assert.Null(dateTimeTestClass.NullableDateTime);

        var history = dateTimeTestClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);

        dateTimeTestClass.Commit();

        var dateTimeHistoryClass =
            AuditableRootEntity.LoadFromHistory<DateTimeTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(dateTimeHistoryClass);
        Assert.Equal(dateTimeUtcNow, dateTimeHistoryClass.NonNullableDateTime);
        Assert.Null(dateTimeHistoryClass.NullableDateTime);

        dateTimeHistoryClass.NullableDateTime = dateTimeUtcNow.AddHours(1);

        dateTimeHistoryClass.FinalizeChanges();

        Assert.Equal(dateTimeUtcNow, dateTimeHistoryClass.NonNullableDateTime);
        Assert.Equal(dateTimeUtcNow.AddHours(1), dateTimeHistoryClass.NullableDateTime);

        var history2 = dateTimeHistoryClass.GetEntityChanges();

        Assert.Single(history2);

        var firstEvent2 = history2[0];

        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);

        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated.ValueFieldEvents);

        dateTimeHistoryClass.Commit();

        history.AddRange(history2);
        var dateTimeHistoryClassTwo =
            AuditableRootEntity.LoadFromHistory<DateTimeTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(dateTimeHistoryClassTwo);
        Assert.Equal(dateTimeUtcNow, dateTimeHistoryClassTwo.NonNullableDateTime);
        Assert.Equal(dateTimeUtcNow.AddHours(1), dateTimeHistoryClassTwo.NullableDateTime);

        dateTimeHistoryClassTwo.NullableDateTime = null;

        Assert.Null(dateTimeHistoryClassTwo.NullableDateTime);

        dateTimeHistoryClassTwo.FinalizeChanges();
        var history3 = dateTimeHistoryClassTwo.GetEntityChanges();

        Assert.Single(history3);

        var firstEvent3 = history3[0];

        Assert.NotNull(firstEvent3);
        Assert.IsType<AuditableEntityUpdated>(firstEvent3);

        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated2);
        Assert.NotNull(auditableEntityUpdated2.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated2.ValueFieldEvents);
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
}