using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Events.EntityEvents;

namespace AuditableDomainEntity.UnitTests.ValueFields;

public class BoolValueFieldTests
{
    [Fact]
    public void ValueField_Should_HandleBoolTypes()
    {
        var boolTestClass = new BoolTestClass
        {
            NonNullableBool = true
        };

        boolTestClass.FinalizeChanges();

        Assert.True(boolTestClass.NonNullableBool);
        Assert.Null(boolTestClass.NullableBool);

        var history = boolTestClass.GetEntityChanges();

        Assert.Single(history);

        var firstEvent = history[0];

        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);

        var auditableEntityCreated = firstEvent as AuditableEntityCreated;

        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);

        boolTestClass.Commit();

        var boolHistoryClass =
            AuditableRootEntity.LoadFromHistory<BoolTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(boolHistoryClass);
        Assert.True(boolHistoryClass.NonNullableBool);
        Assert.Null(boolHistoryClass.NullableBool);

        boolHistoryClass.NullableBool = false;

        boolHistoryClass.FinalizeChanges();

        Assert.True(boolHistoryClass.NonNullableBool);
        Assert.False(boolHistoryClass.NullableBool);

        var history2 = boolHistoryClass.GetEntityChanges();

        Assert.Single(history2);

        var firstEvent2 = history2[0];

        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);

        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated.ValueFieldEvents);

        boolHistoryClass.Commit();

        history.AddRange(history2);
        var boolHistoryClassTwo =
            AuditableRootEntity.LoadFromHistory<BoolTestClass>(auditableEntityCreated.Id, history);

        Assert.NotNull(boolHistoryClassTwo);
        Assert.True(boolHistoryClassTwo.NonNullableBool);
        Assert.False(boolHistoryClassTwo.NullableBool);

        boolHistoryClassTwo.NullableBool = null;

        Assert.Null(boolHistoryClassTwo.NullableBool);

        boolHistoryClassTwo.FinalizeChanges();
        var history3 = boolHistoryClassTwo.GetEntityChanges();

        Assert.Single(history3);

        var firstEvent3 = history3[0];

        Assert.NotNull(firstEvent3);

        Assert.IsType<AuditableEntityUpdated>(firstEvent3);

        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;

        Assert.NotNull(auditableEntityUpdated2);
        Assert.NotNull(auditableEntityUpdated2.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated2.ValueFieldEvents);
    }
    
    public class BoolTestClass : AuditableRootEntity
    {
        [AuditableValueField<bool>(true)]
        public bool? NullableBool
        {
            get => GetValue<bool?>(nameof(NullableBool));
            set => SetValue<bool?>(value, nameof(NullableBool));
        }

        [AuditableValueField<bool>(false)]
        public bool NonNullableBool
        {
            get => GetValue<bool>(nameof(NonNullableBool));
            set => SetValue(value, nameof(NonNullableBool));
        }
    
        public BoolTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
        public BoolTestClass() { }
    }
}