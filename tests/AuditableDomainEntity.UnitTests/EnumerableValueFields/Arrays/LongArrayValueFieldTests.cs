using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Events.EntityEvents;

namespace AuditableDomainEntity.UnitTests.EnumerableValueFields.Arrays;

public class LongArrayValueFieldTests
{
    private static readonly long[] ExpectedNonNull = [1, 2, 3];
    private static readonly long[] ExpectedNullable = [4, 5, 6];
    private static readonly long[] ExpectedNullable2 = [7, 8, 9];
    
    [Fact]
    public void ValueField_Should_HandleLongArrayTypes()
    {
        var longArrayClass = new LongTestClass
        {
            NonNullableLongArray = ExpectedNonNull
        };

        longArrayClass.FinalizeChanges();

        Assert.Equal(ExpectedNonNull, longArrayClass.NonNullableLongArray);
        Assert.Null(longArrayClass.NullableLongArray);

        var history = longArrayClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);

        longArrayClass.Commit();

        var longArrayHistoryClass =
            AuditableRootEntity.LoadFromHistory<LongTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(longArrayHistoryClass);
        Assert.Equal(ExpectedNonNull, longArrayHistoryClass.NonNullableLongArray);
        Assert.Null(longArrayHistoryClass.NullableLongArray);

        longArrayHistoryClass.NullableLongArray = ExpectedNullable;
        
        longArrayHistoryClass.FinalizeChanges();
        
        Assert.Equal(ExpectedNonNull, longArrayHistoryClass.NonNullableLongArray);
        Assert.Equal(ExpectedNullable, longArrayHistoryClass.NullableLongArray);
        
        var history2 = longArrayHistoryClass.GetEntityChanges();
        
        Assert.Single(history2);
        
        var firstEvent2 = history2[0];
        
        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);
        
        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);
        
        longArrayHistoryClass.Commit();
        history.AddRange(history2);
        var longArrayHistoryClass2 = AuditableRootEntity.LoadFromHistory<LongTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(longArrayHistoryClass2);
        Assert.Equal(ExpectedNonNull, longArrayHistoryClass2.NonNullableLongArray);
        Assert.Equal(ExpectedNullable, longArrayHistoryClass2.NullableLongArray);
        
        longArrayHistoryClass2.NullableLongArray = null;
        
        longArrayHistoryClass2.FinalizeChanges();
        
        Assert.Equal(ExpectedNonNull, longArrayHistoryClass2.NonNullableLongArray);
        Assert.Null(longArrayHistoryClass2.NullableLongArray);
        
        var history3 = longArrayHistoryClass2.GetEntityChanges();
        
        Assert.Single(history3);
        
        var firstEvent3 = history3[0];
        
        Assert.NotNull(firstEvent3);
        Assert.IsType<AuditableEntityUpdated>(firstEvent3);
        
        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated2);
        Assert.NotNull(auditableEntityUpdated2.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated2.ValueFieldEvents);
        
        longArrayHistoryClass2.Commit();
        
        history.AddRange(history3);
        var longArrayHistoryClass3 = AuditableRootEntity.LoadFromHistory<LongTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(longArrayHistoryClass3);
        Assert.Equal(ExpectedNonNull, longArrayHistoryClass3.NonNullableLongArray);
        Assert.Null(longArrayHistoryClass3.NullableLongArray);
        
        longArrayHistoryClass3.NullableLongArray = ExpectedNullable2;
        
        longArrayHistoryClass3.FinalizeChanges();
        
        Assert.Equal(ExpectedNonNull, longArrayHistoryClass3.NonNullableLongArray);
        Assert.Equal(ExpectedNullable2, longArrayHistoryClass3.NullableLongArray);
        
        var history4 = longArrayHistoryClass3.GetEntityChanges();
        
        Assert.Single(history4);
        
        var firstEvent4 = history4[0];
        
        Assert.NotNull(firstEvent4);
        Assert.IsType<AuditableEntityUpdated>(firstEvent4);
        
        var auditableEntityUpdated3 = firstEvent4 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated3);
        Assert.NotNull(auditableEntityUpdated3.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated3.ValueFieldEvents);
    }

    [Fact]
    public void ValueField_Should_HandleEmptyArrays()
    {
        var longArrayClass = new LongTestClass
        {
            NonNullableLongArray = []
        };

        longArrayClass.FinalizeChanges();

        Assert.Empty(longArrayClass.NonNullableLongArray);
        Assert.Null(longArrayClass.NullableLongArray);
        
        var history = longArrayClass.GetEntityChanges();
        
        Assert.Single(history);
        
        var firstEvent = history[0];
        
        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);
        
        var auditableEntityCreated = firstEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        
        longArrayClass.Commit();
        
        var longArrayHistoryClass =
            AuditableRootEntity.LoadFromHistory<LongTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(longArrayHistoryClass);
        Assert.NotNull(longArrayHistoryClass.NonNullableLongArray);
        Assert.Empty(longArrayHistoryClass.NonNullableLongArray);
    }
    
    [Fact]
    public void ValueField_Should_HandleNullArrays()
    {
        var longArrayClass = new LongTestClass
        {
            NonNullableLongArray = null
        };

        longArrayClass.FinalizeChanges();

        Assert.Null(longArrayClass.NonNullableLongArray);
        Assert.Null(longArrayClass.NullableLongArray);
        
        var history = longArrayClass.GetEntityChanges();
        
        Assert.Single(history);
        
        var firstEvent = history[0];
        
        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);
        
        var auditableEntityCreated = firstEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.Empty(auditableEntityCreated.ValueFieldEvents);
        
        longArrayClass.Commit();
        
        var longArrayHistoryClass =
            AuditableRootEntity.LoadFromHistory<LongTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(longArrayHistoryClass);
        Assert.Null(longArrayHistoryClass.NonNullableLongArray);
        Assert.Null(longArrayHistoryClass.NullableLongArray);
    }

    public class LongTestClass : AuditableRootEntity
    {
        [AuditableValueField<long[]>(true)]
        public long[]? NullableLongArray
        {
            get => GetValue<long[]?>(nameof(NullableLongArray));
            set => SetValue<long[]?>(value, nameof(NullableLongArray));
        }

        [AuditableValueField<long[]>(false)]
        public long[] NonNullableLongArray
        {
            get => GetValue<long[]>(nameof(NonNullableLongArray));
            set => SetValue<long[]>(value, nameof(NonNullableLongArray));
        }
    
        public LongTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
        public LongTestClass() { }
    }
}