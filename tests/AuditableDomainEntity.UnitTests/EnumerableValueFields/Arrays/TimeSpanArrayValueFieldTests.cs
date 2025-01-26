using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Events.EntityEvents;

namespace AuditableDomainEntity.UnitTests.EnumerableValueFields.Arrays;

public class TimeSpanArrayValueFieldTests
{
    private static readonly TimeSpan[] ExpectedNonNull = [TimeSpan.FromHours(1), TimeSpan.FromHours(2), TimeSpan.FromHours(3)];
    private static readonly TimeSpan[] ExpectedNullable = [TimeSpan.FromHours(4), TimeSpan.FromHours(5), TimeSpan.FromHours(6)];
    private static readonly TimeSpan[] ExpectedNullable2 = [TimeSpan.FromHours(7), TimeSpan.FromHours(8), TimeSpan.FromHours(9)];

    [Fact]
    public void ValueField_Should_HandleTimeSpanArrayTypes()
    {
        var timeSpanArrayClass = new TimeSpanArrayTestClass
        {
            NonNullableTimeSpanArray = ExpectedNonNull
        };

        timeSpanArrayClass.FinalizeChanges();

        Assert.Equal(ExpectedNonNull, timeSpanArrayClass.NonNullableTimeSpanArray);
        Assert.Null(timeSpanArrayClass.NullableTimeSpanArray);

        var history = timeSpanArrayClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);

        timeSpanArrayClass.Commit();

        var timeSpanArrayHistoryClass =
            AuditableRootEntity.LoadFromHistory<TimeSpanArrayTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(timeSpanArrayHistoryClass);
        Assert.Equal(ExpectedNonNull, timeSpanArrayHistoryClass.NonNullableTimeSpanArray);
        Assert.Null(timeSpanArrayHistoryClass.NullableTimeSpanArray);

        timeSpanArrayHistoryClass.NullableTimeSpanArray = ExpectedNullable;

        timeSpanArrayHistoryClass.FinalizeChanges();

        Assert.Equal(ExpectedNonNull, timeSpanArrayHistoryClass.NonNullableTimeSpanArray);
        Assert.Equal(ExpectedNullable, timeSpanArrayHistoryClass.NullableTimeSpanArray);

        var history2 = timeSpanArrayHistoryClass.GetEntityChanges();

        Assert.Single(history2);

        var firstEvent2 = history2[0];

        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);

        var auditableEntityChanged = firstEvent2 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityChanged);
        Assert.NotNull(auditableEntityChanged.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityChanged.ValueFieldEvents);
        Assert.Single(auditableEntityChanged.ValueFieldEvents);

        timeSpanArrayHistoryClass.NullableTimeSpanArray = ExpectedNullable2;

        timeSpanArrayHistoryClass.FinalizeChanges();

        Assert.Equal(ExpectedNonNull, timeSpanArrayHistoryClass.NonNullableTimeSpanArray);
        Assert.Equal(ExpectedNullable2, timeSpanArrayHistoryClass.NullableTimeSpanArray);
        
        var history3 = timeSpanArrayHistoryClass.GetEntityChanges();
        history.AddRange(history2);
        history.AddRange(history3);
        
        Assert.Equal(4, history.Count);
        
        timeSpanArrayHistoryClass.Commit();
        
        var timeSpanArrayHistoryClass2 = AuditableRootEntity.LoadFromHistory<TimeSpanArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(timeSpanArrayHistoryClass2);
        Assert.Equal(ExpectedNonNull, timeSpanArrayHistoryClass2.NonNullableTimeSpanArray);
        Assert.Equal(ExpectedNullable2, timeSpanArrayHistoryClass2.NullableTimeSpanArray);
        
        timeSpanArrayHistoryClass2.NullableTimeSpanArray = null;
        
        timeSpanArrayHistoryClass2.FinalizeChanges();
        
        Assert.Equal(ExpectedNonNull, timeSpanArrayHistoryClass2.NonNullableTimeSpanArray);
        Assert.Null(timeSpanArrayHistoryClass2.NullableTimeSpanArray);
        
        var history4 = timeSpanArrayHistoryClass2.GetEntityChanges();
        
        Assert.Single(history4);
        
        var firstEvent4 = history4[0];
        
        Assert.NotNull(firstEvent4);
        Assert.IsType<AuditableEntityUpdated>(firstEvent4);
        
        var auditableEntityUpdated2 = firstEvent4 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated2);
        Assert.NotNull(auditableEntityUpdated2.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated2.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated2.ValueFieldEvents);
    }

    [Fact]
    public void ValueField_Should_HandleEmptyArrays()
    {
        var timeSpanArrayClass = new TimeSpanArrayTestClass
        {
            NonNullableTimeSpanArray = []
        };

        timeSpanArrayClass.FinalizeChanges();

        Assert.Empty(timeSpanArrayClass.NonNullableTimeSpanArray);
        Assert.Null(timeSpanArrayClass.NullableTimeSpanArray);
        
        var history = timeSpanArrayClass.GetEntityChanges();
        
        Assert.Single(history);
        
        var firstEvent = history[0];
        
        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);
        
        var auditableEntityCreated = firstEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);
        
        timeSpanArrayClass.Commit();
        
        var timeSpanArrayHistoryClass =
            AuditableRootEntity.LoadFromHistory<TimeSpanArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(timeSpanArrayHistoryClass);
        Assert.NotNull(timeSpanArrayHistoryClass.NonNullableTimeSpanArray);
        Assert.Empty(timeSpanArrayHistoryClass.NonNullableTimeSpanArray);
    }

    [Fact]
    public void ValueField_Should_HandleNullArrays()
    {
        var timeSpanArrayClass = new TimeSpanArrayTestClass
        {
            NonNullableTimeSpanArray = null
        };

        timeSpanArrayClass.FinalizeChanges();

        Assert.Null(timeSpanArrayClass.NonNullableTimeSpanArray);
        Assert.Null(timeSpanArrayClass.NullableTimeSpanArray);
        
        var history = timeSpanArrayClass.GetEntityChanges();
        
        Assert.Single(history);
        
        var firstEvent = history[0];
        
        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);
        
        var auditableEntityCreated = firstEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.Empty(auditableEntityCreated.ValueFieldEvents);
        
        timeSpanArrayClass.Commit();
        
        var timeSpanArrayHistoryClass =
            AuditableRootEntity.LoadFromHistory<TimeSpanArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(timeSpanArrayHistoryClass);
        Assert.Null(timeSpanArrayHistoryClass.NonNullableTimeSpanArray);
        Assert.Null(timeSpanArrayHistoryClass.NullableTimeSpanArray);
    }

    public class TimeSpanArrayTestClass : AuditableRootEntity
    {
        [AuditableValueField<TimeSpan[]>(true)]
        public TimeSpan[]? NullableTimeSpanArray
        {
            get => GetValue<TimeSpan[]?>(nameof(NullableTimeSpanArray));
            set => SetValue<TimeSpan[]?>(value, nameof(NullableTimeSpanArray));
        }
        
        [AuditableValueField<TimeSpan[]>(false)]
        public TimeSpan[] NonNullableTimeSpanArray
        {
            get => GetValue<TimeSpan[]>(nameof(NonNullableTimeSpanArray));
            set => SetValue<TimeSpan[]>(value, nameof(NonNullableTimeSpanArray));
        }
        
        public TimeSpanArrayTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
        public TimeSpanArrayTestClass() { }
    }
}