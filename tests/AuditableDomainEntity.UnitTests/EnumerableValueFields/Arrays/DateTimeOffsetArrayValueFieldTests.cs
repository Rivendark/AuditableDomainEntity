using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Events.EntityEvents;

namespace AuditableDomainEntity.UnitTests.EnumerableValueFields.Arrays;

public class DateTimeOffsetArrayValueFieldTests
{
    private static readonly DateTimeOffset[] ExpectedNonNull = [DateTimeOffset.Now, DateTimeOffset.Now.AddDays(1), DateTimeOffset.Now.AddDays(2)];
    private static readonly DateTimeOffset[] ExpectedNullable = [DateTimeOffset.Now.AddDays(3), DateTimeOffset.Now.AddDays(4), DateTimeOffset.Now.AddDays(5)];

    [Fact]
    public void ValueField_Should_HandleDateTimeOffsetArrayTypes()
    {
        var dateTimeOffsetArrayClass = new DateTimeOffsetArrayTestClass
        {
            NonNullableDateTimeOffsetArray = ExpectedNonNull
        };

        dateTimeOffsetArrayClass.FinalizeChanges();

        Assert.Equal(ExpectedNonNull, dateTimeOffsetArrayClass.NonNullableDateTimeOffsetArray);
        Assert.Null(dateTimeOffsetArrayClass.NullableDateTimeOffsetArray);
        
        var history = dateTimeOffsetArrayClass.GetEntityChanges();
        
        Assert.Single(history);
        
        var firstEvent = history[0];
        
        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);
        
        var auditableEntityCreated = firstEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);
        
        dateTimeOffsetArrayClass.Commit();
        
        var dateTimeOffsetArrayHistoryClass = AuditableRootEntity.LoadFromHistory<DateTimeOffsetArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(dateTimeOffsetArrayHistoryClass);
        Assert.Equal(ExpectedNonNull, dateTimeOffsetArrayHistoryClass.NonNullableDateTimeOffsetArray);
        Assert.Null(dateTimeOffsetArrayHistoryClass.NullableDateTimeOffsetArray);

        dateTimeOffsetArrayHistoryClass.NullableDateTimeOffsetArray = ExpectedNullable;
        
        dateTimeOffsetArrayHistoryClass.FinalizeChanges();
        
        Assert.Equal(ExpectedNonNull, dateTimeOffsetArrayHistoryClass.NonNullableDateTimeOffsetArray);
        Assert.Equal(ExpectedNullable, dateTimeOffsetArrayHistoryClass.NullableDateTimeOffsetArray);
        
        var history2 = dateTimeOffsetArrayHistoryClass.GetEntityChanges();
        
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
        
        dateTimeOffsetArrayHistoryClass.Commit();
        history.AddRange(history2);
        
        Assert.Equal(2, history.Count);
        
        var firstEvent3 = history[0];
        
        Assert.NotNull(firstEvent3);
        Assert.IsType<AuditableEntityCreated>(firstEvent3);
        
        var auditableEntityCreated2 = firstEvent3 as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated2);
        
        var firstEvent4 = history[1];
        
        Assert.NotNull(firstEvent4);
        Assert.IsType<AuditableEntityUpdated>(firstEvent4);
        
        var auditableEntityUpdated2 = firstEvent4 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated2);
        
        var valueFieldEvents2 = auditableEntityUpdated2.ValueFieldEvents;
        
        Assert.NotNull(valueFieldEvents2);
        Assert.NotEmpty(valueFieldEvents2);
        Assert.Single(valueFieldEvents2);
        
        var dateTimeOffsetArrayHistoryClass2 = AuditableRootEntity.LoadFromHistory<DateTimeOffsetArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(dateTimeOffsetArrayHistoryClass2);
        Assert.Equal(ExpectedNonNull, dateTimeOffsetArrayHistoryClass2.NonNullableDateTimeOffsetArray);
        Assert.Equal(ExpectedNullable, dateTimeOffsetArrayHistoryClass2.NullableDateTimeOffsetArray);
        
        dateTimeOffsetArrayHistoryClass2.NullableDateTimeOffsetArray = null;
        
        dateTimeOffsetArrayHistoryClass2.FinalizeChanges();
        
        Assert.Equal(ExpectedNonNull, dateTimeOffsetArrayHistoryClass2.NonNullableDateTimeOffsetArray);
        Assert.Null(dateTimeOffsetArrayHistoryClass2.NullableDateTimeOffsetArray);
        
        var history3 = dateTimeOffsetArrayHistoryClass2.GetEntityChanges();
        
        Assert.Single(history3);
        
        var firstEvent5 = history3[0];
        
        Assert.NotNull(firstEvent5);
        Assert.IsType<AuditableEntityUpdated>(firstEvent5);
        
        var auditableEntityUpdated3 = firstEvent5 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated3);
        
        var valueFieldEvents3 = auditableEntityUpdated3.ValueFieldEvents;
        
        Assert.NotNull(valueFieldEvents3);
        Assert.NotEmpty(valueFieldEvents3);
        Assert.Single(valueFieldEvents3);
    }
    
    [Fact]
    public void ValueField_Should_HandleEmptyArrays()
    {
        var dateTimeOffsetArrayClass = new DateTimeOffsetArrayTestClass
        {
            NonNullableDateTimeOffsetArray = Array.Empty<DateTimeOffset>()
        };

        dateTimeOffsetArrayClass.FinalizeChanges();

        Assert.Empty(dateTimeOffsetArrayClass.NonNullableDateTimeOffsetArray);
        Assert.Null(dateTimeOffsetArrayClass.NullableDateTimeOffsetArray);
        
        var history = dateTimeOffsetArrayClass.GetEntityChanges();
        
        Assert.Single(history);
        
        var firstEvent = history[0];
        
        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);
        
        var auditableEntityCreated = firstEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);
        
        dateTimeOffsetArrayClass.Commit();
        
        var dateTimeOffsetArrayHistoryClass =
            AuditableRootEntity.LoadFromHistory<DateTimeOffsetArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(dateTimeOffsetArrayHistoryClass);
        Assert.NotNull(dateTimeOffsetArrayHistoryClass.NonNullableDateTimeOffsetArray);
        Assert.Empty(dateTimeOffsetArrayHistoryClass.NonNullableDateTimeOffsetArray);
    }

    [Fact]
    public void ValueField_Should_HandleNullArrays()
    {
        var dateTimeOffsetArrayClass = new DateTimeOffsetArrayTestClass
        {
            NonNullableDateTimeOffsetArray = null
        };

        dateTimeOffsetArrayClass.FinalizeChanges();

        Assert.Null(dateTimeOffsetArrayClass.NonNullableDateTimeOffsetArray);
        Assert.Null(dateTimeOffsetArrayClass.NullableDateTimeOffsetArray);

        var history = dateTimeOffsetArrayClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.Empty(auditableEntityCreated.ValueFieldEvents);

        dateTimeOffsetArrayClass.Commit();

        var dateTimeOffsetArrayHistoryClass =
            AuditableRootEntity.LoadFromHistory<DateTimeOffsetArrayTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(dateTimeOffsetArrayHistoryClass);
        Assert.Null(dateTimeOffsetArrayHistoryClass.NonNullableDateTimeOffsetArray);
        Assert.Null(dateTimeOffsetArrayHistoryClass.NullableDateTimeOffsetArray);
    }

    public class DateTimeOffsetArrayTestClass : AuditableRootEntity
    {
        [AuditableValueField<DateTimeOffset[]>(true)]
        public DateTimeOffset[]? NullableDateTimeOffsetArray
        {
            get => GetValue<DateTimeOffset[]?>(nameof(NullableDateTimeOffsetArray));
            set => SetValue<DateTimeOffset[]?>(value, nameof(NullableDateTimeOffsetArray));
        }
        
        [AuditableValueField<DateTimeOffset[]>(false)]
        public DateTimeOffset[] NonNullableDateTimeOffsetArray
        {
            get => GetValue<DateTimeOffset[]>(nameof(NonNullableDateTimeOffsetArray));
            set => SetValue<DateTimeOffset[]>(value, nameof(NonNullableDateTimeOffsetArray));
        }
        
        public DateTimeOffsetArrayTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
        public DateTimeOffsetArrayTestClass() { }
    }
}