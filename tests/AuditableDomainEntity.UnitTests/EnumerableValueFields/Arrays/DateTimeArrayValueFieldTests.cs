using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Events.EntityEvents;

namespace AuditableDomainEntity.UnitTests.EnumerableValueFields.Arrays;

public class DateTimeArrayValueFieldTests
{
    private static readonly DateTime[] ExpectedNonNull = [DateTime.Now, DateTime.Now.AddDays(1), DateTime.Now.AddDays(2)];
    private static readonly DateTime[] ExpectedNullable = [DateTime.Now.AddDays(3), DateTime.Now.AddDays(4), DateTime.Now.AddDays(5)];

    [Fact]
    public void ValueField_Should_HandleDateTimeArrayTypes()
    {
        var dateTimeArrayClass = new DateTimeArrayTestClass
        {
            NonNullableDateTimeArray = ExpectedNonNull
        };

        dateTimeArrayClass.FinalizeChanges();

        Assert.Equal(ExpectedNonNull, dateTimeArrayClass.NonNullableDateTimeArray);
        Assert.Null(dateTimeArrayClass.NullableDateTimeArray);

        var history = dateTimeArrayClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);

        dateTimeArrayClass.Commit();

        var dateTimeArrayHistoryClass =
            AuditableRootEntity.LoadFromHistory<DateTimeArrayTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(dateTimeArrayHistoryClass);
        Assert.Equal(ExpectedNonNull, dateTimeArrayHistoryClass.NonNullableDateTimeArray);
        Assert.Null(dateTimeArrayHistoryClass.NullableDateTimeArray);
        
        dateTimeArrayHistoryClass.NullableDateTimeArray = ExpectedNullable;
        
        dateTimeArrayHistoryClass.FinalizeChanges();
        
        Assert.Equal(ExpectedNonNull, dateTimeArrayHistoryClass.NonNullableDateTimeArray);
        Assert.Equal(ExpectedNullable, dateTimeArrayHistoryClass.NullableDateTimeArray);
        
        var history2 = dateTimeArrayHistoryClass.GetEntityChanges();
        
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
        
        dateTimeArrayHistoryClass.Commit();
        history.AddRange(history2);
        var dateTimeArrayHistoryClass2 = AuditableRootEntity.LoadFromHistory<DateTimeArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(dateTimeArrayHistoryClass2);
        Assert.Equal(ExpectedNonNull, dateTimeArrayHistoryClass2.NonNullableDateTimeArray);
        Assert.Equal(ExpectedNullable, dateTimeArrayHistoryClass2.NullableDateTimeArray);
        
        dateTimeArrayHistoryClass2.NullableDateTimeArray = null;
        
        dateTimeArrayHistoryClass2.FinalizeChanges();
        
        Assert.Equal(ExpectedNonNull, dateTimeArrayHistoryClass2.NonNullableDateTimeArray);
        Assert.Null(dateTimeArrayHistoryClass2.NullableDateTimeArray);
        
        var history3 = dateTimeArrayHistoryClass2.GetEntityChanges();
        
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
    }

    [Fact]
    public void ValueField_Should_HandleEmptyArrays()
    {
        var dateTimeArrayHistoryClass = new DateTimeArrayTestClass
        {
            NonNullableDateTimeArray = []
        };
        
        dateTimeArrayHistoryClass.FinalizeChanges();
        
        Assert.Empty(dateTimeArrayHistoryClass.NonNullableDateTimeArray);
        
        var history = dateTimeArrayHistoryClass.GetEntityChanges();
        
        Assert.Single(history);
        
        var firstEvent = history[0];
        
        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);
        
        var auditableEntityCreated = firstEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);
        
        dateTimeArrayHistoryClass.Commit();
        
        var dateTimeArrayHistoryClass2 = AuditableRootEntity.LoadFromHistory<DateTimeArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(dateTimeArrayHistoryClass2);
        Assert.NotNull(dateTimeArrayHistoryClass2.NonNullableDateTimeArray);
        Assert.Empty(dateTimeArrayHistoryClass2.NonNullableDateTimeArray);
    }
    
    [Fact]
    public void ValueField_Should_HandleNullArrays()
    {
        var dateTimeArrayHistoryClass = new DateTimeArrayTestClass
        {
            NonNullableDateTimeArray = null
        };
        
        dateTimeArrayHistoryClass.FinalizeChanges();
        
        Assert.Null(dateTimeArrayHistoryClass.NonNullableDateTimeArray);
        
        var history = dateTimeArrayHistoryClass.GetEntityChanges();
        
        Assert.Single(history);
        
        var firstEvent = history[0];
        
        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);
        
        var auditableEntityCreated = firstEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.Empty(auditableEntityCreated.ValueFieldEvents);
        
        dateTimeArrayHistoryClass.Commit();
        
        var dateTimeArrayHistoryClass2 = AuditableRootEntity.LoadFromHistory<DateTimeArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(dateTimeArrayHistoryClass2);
        Assert.Null(dateTimeArrayHistoryClass2.NonNullableDateTimeArray);
    }

    public class DateTimeArrayTestClass : AuditableRootEntity
    {
        [AuditableValueField<DateTime[]>(true)]
        public DateTime[]? NullableDateTimeArray
        {
            get => GetValue<DateTime[]?>(nameof(NullableDateTimeArray));
            set => SetValue<DateTime[]?>(value, nameof(NullableDateTimeArray));
        }
        
        [AuditableValueField<DateTime[]>(false)]
        public DateTime[] NonNullableDateTimeArray
        {
            get => GetValue<DateTime[]>(nameof(NonNullableDateTimeArray));
            set => SetValue<DateTime[]>(value, nameof(NonNullableDateTimeArray));
        }
        
        public DateTimeArrayTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
        public DateTimeArrayTestClass() { }
    }
}