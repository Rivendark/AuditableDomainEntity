using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Events.EntityEvents;

namespace AuditableDomainEntity.UnitTests.EnumerableValueFields.Arrays;

public class DecimalArrayValueFieldTests
{
    private static readonly decimal[] ExpectedNonNull = [1.1m, 2.2m, 3.3m];
    private static readonly decimal[] ExpectedNullable = [4.4m, 5.5m, 6.6m];

    [Fact]
    public void ValueField_Should_HandleDecimalArrayTypes()
    {
        var decimalArrayClass = new DecimalArrayTestClass
        {
            NonNullableDecimalArray = ExpectedNonNull
        };

        decimalArrayClass.FinalizeChanges();

        Assert.Equal(ExpectedNonNull, decimalArrayClass.NonNullableDecimalArray);
        Assert.Null(decimalArrayClass.NullableDecimalArray);

        var history = decimalArrayClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);

        decimalArrayClass.Commit();

        var decimalArrayHistoryClass =
            AuditableRootEntity.LoadFromHistory<DecimalArrayTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(decimalArrayHistoryClass);
        Assert.Equal(ExpectedNonNull, decimalArrayHistoryClass.NonNullableDecimalArray);
        Assert.Null(decimalArrayHistoryClass.NullableDecimalArray);
        
        decimalArrayHistoryClass.NullableDecimalArray = ExpectedNullable;
        
        decimalArrayHistoryClass.FinalizeChanges();
        
        Assert.Equal(ExpectedNonNull, decimalArrayHistoryClass.NonNullableDecimalArray);
        Assert.Equal(ExpectedNullable, decimalArrayHistoryClass.NullableDecimalArray);
        
        var history2 = decimalArrayHistoryClass.GetEntityChanges();
        
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
        
        decimalArrayHistoryClass.Commit();
        history.AddRange(history2);
        
        var decimalArrayHistoryClass2 = AuditableRootEntity.LoadFromHistory<DecimalArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(decimalArrayHistoryClass2);
        Assert.Equal(ExpectedNonNull, decimalArrayHistoryClass2.NonNullableDecimalArray);
        Assert.Equal(ExpectedNullable, decimalArrayHistoryClass2.NullableDecimalArray);
        
        decimalArrayHistoryClass2.NullableDecimalArray = null;
        
        decimalArrayHistoryClass2.FinalizeChanges();
        
        Assert.Equal(ExpectedNonNull, decimalArrayHistoryClass2.NonNullableDecimalArray);
        Assert.Null(decimalArrayHistoryClass2.NullableDecimalArray);
        
        var history3 = decimalArrayHistoryClass2.GetEntityChanges();
        
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
        
        decimalArrayHistoryClass2.Commit();
        
        history.AddRange(history3);
        
        var decimalArrayHistoryClass3 = AuditableRootEntity.LoadFromHistory<DecimalArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(decimalArrayHistoryClass3);
        
        Assert.Equal(ExpectedNonNull, decimalArrayHistoryClass3.NonNullableDecimalArray);
        Assert.Null(decimalArrayHistoryClass3.NullableDecimalArray);
    }

    [Fact]
    public void ValueField_Should_HandleEmptyArrays()
    {
        var decimalArrayClass = new DecimalArrayTestClass
        {
            NonNullableDecimalArray = Array.Empty<decimal>()
        };

        decimalArrayClass.FinalizeChanges();

        Assert.Empty(decimalArrayClass.NonNullableDecimalArray);
        Assert.Null(decimalArrayClass.NullableDecimalArray);
        
        var history = decimalArrayClass.GetEntityChanges();
        
        Assert.Single(history);
        
        var firstEvent = history[0];
        
        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);
        
        var auditableEntityCreated = firstEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);
        
        decimalArrayClass.Commit();
        
        var decimalArrayHistoryClass =
            AuditableRootEntity.LoadFromHistory<DecimalArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(decimalArrayHistoryClass);
        Assert.NotNull(decimalArrayHistoryClass.NonNullableDecimalArray);
        Assert.Empty(decimalArrayHistoryClass.NonNullableDecimalArray);
    }
    
    [Fact]
    public void ValueField_Should_HandleNullArrays()
    {
        var decimalArrayClass = new DecimalArrayTestClass
        {
            NonNullableDecimalArray = null
        };

        decimalArrayClass.FinalizeChanges();

        Assert.Null(decimalArrayClass.NonNullableDecimalArray);
        Assert.Null(decimalArrayClass.NullableDecimalArray);
        
        var history = decimalArrayClass.GetEntityChanges();
        
        Assert.Single(history);
        
        var firstEvent = history[0];
        
        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);
        
        var auditableEntityCreated = firstEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.Empty(auditableEntityCreated.ValueFieldEvents);
        
        decimalArrayClass.Commit();
        
        var decimalArrayHistoryClass =
            AuditableRootEntity.LoadFromHistory<DecimalArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(decimalArrayHistoryClass);
        Assert.Null(decimalArrayHistoryClass.NonNullableDecimalArray);
        Assert.Null(decimalArrayHistoryClass.NullableDecimalArray);
    }

    public class DecimalArrayTestClass : AuditableRootEntity
    {
        [AuditableValueField<decimal[]>(true)]
        public decimal[]? NullableDecimalArray
        {
            get => GetValue<decimal[]?>(nameof(NullableDecimalArray));
            set => SetValue<decimal[]?>(value, nameof(NullableDecimalArray));
        }
        
        [AuditableValueField<decimal[]>(false)]
        public decimal[] NonNullableDecimalArray
        {
            get => GetValue<decimal[]>(nameof(NonNullableDecimalArray));
            set => SetValue<decimal[]>(value, nameof(NonNullableDecimalArray));
        }
        
        public DecimalArrayTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
        public DecimalArrayTestClass() { }
    }
}