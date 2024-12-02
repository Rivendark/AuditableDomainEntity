using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Events.EntityEvents;

namespace AuditableDomainEntity.Tests.EnumerableValueFields.Arrays;

public class GuidArrayValueFieldTests
{
    private static readonly Guid[] ExpectedNonNull = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()];
    private static readonly Guid[] ExpectedNullable = [Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()];
    
    [Fact]
    public void ValueField_Should_HandleGuidArrayTypes()
    {
        var guidArrayClass = new GuidArrayTestClass
        {
            NonNullableGuidArray = ExpectedNonNull
        };

        guidArrayClass.FinalizeChanges();

        Assert.Equal(ExpectedNonNull, guidArrayClass.NonNullableGuidArray);
        Assert.Null(guidArrayClass.NullableGuidArray);

        var history = guidArrayClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);

        guidArrayClass.Commit();

        var guidArrayHistoryClass =
            AuditableRootEntity.LoadFromHistory<GuidArrayTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(guidArrayHistoryClass);
        Assert.Equal(ExpectedNonNull, guidArrayHistoryClass.NonNullableGuidArray);
        Assert.Null(guidArrayHistoryClass.NullableGuidArray);

        guidArrayHistoryClass.NullableGuidArray = ExpectedNullable;
        
        guidArrayHistoryClass.FinalizeChanges();
        
        Assert.Equal(ExpectedNonNull, guidArrayHistoryClass.NonNullableGuidArray);
        Assert.Equal(ExpectedNullable, guidArrayHistoryClass.NullableGuidArray);
        
        var history2 = guidArrayHistoryClass.GetEntityChanges();
        
        Assert.Single(history2);
        
        var firstEvent2 = history2[0];
        
        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);
        
        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated.ValueFieldEvents);
        
        guidArrayHistoryClass.Commit();
        
        history.AddRange(history2);
        
        var guidArrayHistoryClass2 = AuditableRootEntity.LoadFromHistory<GuidArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(guidArrayHistoryClass2);
        Assert.Equal(ExpectedNonNull, guidArrayHistoryClass2.NonNullableGuidArray);
        Assert.Equal(ExpectedNullable, guidArrayHistoryClass2.NullableGuidArray);
        
        guidArrayHistoryClass2.NullableGuidArray = null;
        
        guidArrayHistoryClass2.FinalizeChanges();
        
        Assert.Equal(ExpectedNonNull, guidArrayHistoryClass2.NonNullableGuidArray);
        Assert.Null(guidArrayHistoryClass2.NullableGuidArray);
        
        var history3 = guidArrayHistoryClass2.GetEntityChanges();
        
        Assert.Single(history3);
        
        var firstEvent3 = history3[0];
        
        Assert.NotNull(firstEvent3);
        Assert.IsType<AuditableEntityUpdated>(firstEvent3);
        
        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated2);
        Assert.NotNull(auditableEntityUpdated2.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated2.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated2.ValueFieldEvents);
        
        guidArrayHistoryClass2.Commit();
        
        history.AddRange(history3);
        var guidArrayHistoryClass3 =
            AuditableRootEntity.LoadFromHistory<GuidArrayTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(guidArrayHistoryClass3);
        Assert.Equal(ExpectedNonNull, guidArrayHistoryClass3.NonNullableGuidArray);
        Assert.Null(guidArrayHistoryClass3.NullableGuidArray);
    }

    [Fact]
    public void ValueField_Should_HandleEmptyArrays()
    {
        var guidArrayClass = new GuidArrayTestClass
        {
            NonNullableGuidArray = []
        };

        guidArrayClass.FinalizeChanges();

        Assert.Empty(guidArrayClass.NonNullableGuidArray);
        Assert.Null(guidArrayClass.NullableGuidArray);

        var history = guidArrayClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);

        guidArrayClass.Commit();

        var guidArrayHistoryClass =
            AuditableRootEntity.LoadFromHistory<GuidArrayTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(guidArrayHistoryClass);
        Assert.Empty(guidArrayHistoryClass.NonNullableGuidArray);
        Assert.Null(guidArrayHistoryClass.NullableGuidArray);
    }
    
    [Fact]
    public void ValueField_Should_HandleNullArrays()
    {
        var guidArrayClass = new GuidArrayTestClass
        {
            NonNullableGuidArray = null
        };

        guidArrayClass.FinalizeChanges();

        Assert.Null(guidArrayClass.NonNullableGuidArray);
        Assert.Null(guidArrayClass.NullableGuidArray);

        var history = guidArrayClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.Empty(auditableEntityCreated.ValueFieldEvents);

        guidArrayClass.Commit();

        var guidArrayHistoryClass =
            AuditableRootEntity.LoadFromHistory<GuidArrayTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(guidArrayHistoryClass);
        Assert.Null(guidArrayHistoryClass.NonNullableGuidArray);
        Assert.Null(guidArrayHistoryClass.NullableGuidArray);
    }

    public class GuidArrayTestClass : AuditableRootEntity
    {
        [AuditableValueField<Guid[]>(true)]
        public Guid[] NonNullableGuidArray
        {
            get => GetValue<Guid[]>(nameof(NonNullableGuidArray));
            set => SetValue<Guid[]>(value, nameof(NonNullableGuidArray));
        }

        [AuditableValueField<Guid[]>(false)]
        public Guid[]? NullableGuidArray
        {
            get => GetValue<Guid[]?>(nameof(NullableGuidArray));
            set => SetValue<Guid[]?>(value, nameof(NullableGuidArray));
        }
        
        public GuidArrayTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
        public GuidArrayTestClass() { }
    }
}