using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Events.EntityEvents;

namespace AuditableDomainEntity.UnitTests.EnumerableValueFields.Arrays;

public class BoolArrayValueFieldTests
{
    private static readonly bool[] ExpectedNonNull = [true, false, true];
    private static readonly bool[] ExpectedNullable = [false, true, false];

    [Fact]
    public void ValueField_Should_HandleBoolArrayTypes()
    {
        var boolArrayClass = new BoolArrayTestClass
        {
            NonNullableBoolArray = [true, false, true]
        };
        
        boolArrayClass.FinalizeChanges();
        
        Assert.Equal(ExpectedNonNull, boolArrayClass.NonNullableBoolArray);
        Assert.Null(boolArrayClass.NullableBoolArray);

        var history = boolArrayClass.GetEntityChanges();
        
        Assert.Single(history);

        var firstEvent = history[0];
        
        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);
        
        var auditableEntityCreated = firstEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);
        
        boolArrayClass.Commit();
        
        var boolArrayHistoryClass = AuditableRootEntity.LoadFromHistory<BoolArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(boolArrayHistoryClass);
        Assert.Equal(ExpectedNonNull, boolArrayHistoryClass.NonNullableBoolArray);
        Assert.Null(boolArrayHistoryClass.NullableBoolArray);

        boolArrayHistoryClass.NullableBoolArray = [false, true, false];
        
        boolArrayHistoryClass.FinalizeChanges();
        
        Assert.Equal(ExpectedNonNull, boolArrayHistoryClass.NonNullableBoolArray);
        Assert.Equal(ExpectedNullable, boolArrayHistoryClass.NullableBoolArray);
        
        var history2 = boolArrayHistoryClass.GetEntityChanges();
        
        Assert.Single(history2);
        
        var firstEvent2 = history2[0];
        
        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);
        
        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated.ValueFieldEvents);
        
        boolArrayHistoryClass.Commit();
        history.AddRange(history2);
        var boolArrayHistoryClass2 = AuditableRootEntity.LoadFromHistory<BoolArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(boolArrayHistoryClass2);
        Assert.Equal(ExpectedNonNull, boolArrayHistoryClass2.NonNullableBoolArray);
        Assert.Equal(ExpectedNullable, boolArrayHistoryClass2.NullableBoolArray);
        Assert.Equal(boolArrayHistoryClass2.NonNullableBoolArray, boolArrayHistoryClass.NonNullableBoolArray);
        
        boolArrayHistoryClass2.NullableBoolArray = null;
        
        boolArrayHistoryClass2.FinalizeChanges();
        
        Assert.Equal(ExpectedNonNull, boolArrayHistoryClass2.NonNullableBoolArray);
        Assert.Null(boolArrayHistoryClass2.NullableBoolArray);
        
        var history3 = boolArrayHistoryClass2.GetEntityChanges();
        
        Assert.Single(history3);
        
        var firstEvent3 = history3[0];
        
        Assert.NotNull(firstEvent3);
        Assert.IsType<AuditableEntityUpdated>(firstEvent3);
        
        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated2);
        Assert.NotNull(auditableEntityUpdated2.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated2.ValueFieldEvents);
        
        Assert.Single(auditableEntityUpdated2.ValueFieldEvents);
        
        boolArrayHistoryClass2.Commit();
        
        history.AddRange(history3);
        var boolArrayHistoryClass3 = AuditableRootEntity.LoadFromHistory<BoolArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(boolArrayHistoryClass3);
        Assert.Equal(ExpectedNonNull, boolArrayHistoryClass3.NonNullableBoolArray);
        Assert.Null(boolArrayHistoryClass3.NullableBoolArray);
    }

    [Fact]
    public void ValueField_Should_HandleEmptyArrays()
    {
        var boolArrayClass = new BoolArrayTestClass
        {
            NonNullableBoolArray = []
        };

        boolArrayClass.FinalizeChanges();

        Assert.Empty(boolArrayClass.NonNullableBoolArray);
        Assert.Null(boolArrayClass.NullableBoolArray);

        var history = boolArrayClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);

        boolArrayClass.Commit();

        var boolArrayHistoryClass =
            AuditableRootEntity.LoadFromHistory<BoolArrayTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(boolArrayHistoryClass);
        Assert.Empty(boolArrayHistoryClass.NonNullableBoolArray);
        Assert.Null(boolArrayHistoryClass.NullableBoolArray);

        boolArrayHistoryClass.NullableBoolArray = [false, true, false];

        boolArrayHistoryClass.FinalizeChanges();

        Assert.Empty(boolArrayHistoryClass.NonNullableBoolArray);
        Assert.Equal(ExpectedNullable, boolArrayHistoryClass.NullableBoolArray);

        var history2 = boolArrayHistoryClass.GetEntityChanges();

        Assert.Single(history2);

        var firstEvent2 = history2[0];

        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);

        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated.ValueFieldEvents);

        boolArrayHistoryClass.Commit();
        history.AddRange(history2);
        var boolArrayHistoryClass2 =
            AuditableRootEntity.LoadFromHistory<BoolArrayTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(boolArrayHistoryClass2);
        Assert.Empty(boolArrayHistoryClass2.NonNullableBoolArray);
        Assert.Equal(ExpectedNullable, boolArrayHistoryClass2.NullableBoolArray);
        Assert.Equal(boolArrayHistoryClass2.NonNullableBoolArray, boolArrayHistoryClass.NonNullableBoolArray);
    }
    
    [Fact]
    public void ValueField_Should_HandleNullArrays()
    {
        var boolArrayClass = new BoolArrayTestClass
        {
            NonNullableBoolArray = null
        };

        boolArrayClass.FinalizeChanges();

        Assert.Null(boolArrayClass.NonNullableBoolArray);
        Assert.Null(boolArrayClass.NullableBoolArray);

        var history = boolArrayClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.Empty(auditableEntityCreated.ValueFieldEvents);

        boolArrayClass.Commit();

        var boolArrayHistoryClass =
            AuditableRootEntity.LoadFromHistory<BoolArrayTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(boolArrayHistoryClass);
        Assert.Null(boolArrayHistoryClass.NonNullableBoolArray);
        Assert.Null(boolArrayHistoryClass.NullableBoolArray);
    }

    public class BoolArrayTestClass : AuditableRootEntity
    {
        [AuditableValueField<bool[]>(true)]
        public bool[]? NullableBoolArray
        {
            get => GetValue<bool[]?>(nameof(NullableBoolArray));
            set => SetValue<bool[]?>(value, nameof(NullableBoolArray));
        }
        
        [AuditableValueField<bool[]>(false)]
        public bool[] NonNullableBoolArray
        {
            get => GetValue<bool[]>(nameof(NonNullableBoolArray));
            set => SetValue<bool[]>(value, nameof(NonNullableBoolArray));
        }
        
        public BoolArrayTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
        public BoolArrayTestClass() { }
    }
}