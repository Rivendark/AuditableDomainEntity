using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Events.EntityEvents;

namespace AuditableDomainEntity.Tests.ValueFields;

public class LongValueFieldTests
{
    [Fact]
    public void ValueField_Should_HandleLongTypes()
    {
        var longTestClass = new LongTestClass
        {
            NonNullableLong = 12
        };
        
        longTestClass.FinalizeChanges();
        
        Assert.Equal(12, longTestClass.NonNullableLong);
        Assert.Null(longTestClass.NullableLong);
        
        var history = longTestClass.GetEntityChanges();
        
        Assert.Single(history);
        
        var firstEvent = history[0];
        
        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);
        
        var auditableEntityCreated = firstEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);
        
        longTestClass.Commit();
        
        var longHistoryClass = AuditableRootEntity.LoadFromHistory<LongTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(longHistoryClass);
        Assert.Equal(12, longHistoryClass.NonNullableLong);
        Assert.Null(longHistoryClass.NullableLong);
        
        longHistoryClass.NullableLong = 24;
        
        longHistoryClass.FinalizeChanges();
        
        Assert.Equal(12, longHistoryClass.NonNullableLong);
        Assert.Equal(24, longHistoryClass.NullableLong);
        
        var history2 = longHistoryClass.GetEntityChanges();
        
        Assert.Single(history2);
        
        var firstEvent2 = history2[0];
        
        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);
        
        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated.ValueFieldEvents);
        
        longTestClass.Commit();
        
        history.AddRange(history2);
        var longHistoryClassTwo = AuditableRootEntity.LoadFromHistory<LongTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(longHistoryClassTwo);
        Assert.Equal(12, longHistoryClassTwo.NonNullableLong);
        Assert.Equal(24, longHistoryClassTwo.NullableLong);
        
        longHistoryClassTwo.NullableLong = null;
        
        Assert.Null(longHistoryClassTwo.NullableLong);
        
        longHistoryClassTwo.FinalizeChanges();
        var history3 = longHistoryClassTwo.GetEntityChanges();
        
        Assert.Single(history3);
        
        var firstEvent3 = history3[0];
        
        Assert.NotNull(firstEvent3);
        Assert.IsType<AuditableEntityUpdated>(firstEvent3);
        
        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated2);
        Assert.NotNull(auditableEntityUpdated2.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated2.ValueFieldEvents);
        
        longHistoryClassTwo.Commit();
        
        history.AddRange(history3);
        var longHistoryClassThree = AuditableRootEntity.LoadFromHistory<LongTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(longHistoryClassThree);
        Assert.Equal(12, longHistoryClassThree.NonNullableLong);
        Assert.Null(longHistoryClassThree.NullableLong);
    }
    
    public class LongTestClass : AuditableRootEntity
    {
        [AuditableValueField<long?>(true)]
        public long? NullableLong
        {
            get => GetValue<long?>(nameof(NullableLong));
            set => SetValue<long?>(value, nameof(NullableLong));
        }

        [AuditableValueField<long>(false)]
        public long NonNullableLong
        {
            get => GetValue<long>(nameof(NonNullableLong));
            set => SetValue(value, nameof(NonNullableLong));
        }
    
        public LongTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
        public LongTestClass() { }
    }
}