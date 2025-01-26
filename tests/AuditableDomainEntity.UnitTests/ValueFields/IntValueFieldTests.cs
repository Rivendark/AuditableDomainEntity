using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Events.EntityEvents;

namespace AuditableDomainEntity.UnitTests.ValueFields;

public class IntValueFieldTests
{
    [Fact]
    public void ValueField_Should_HandleIntTypes()
    {
        var intClass = new IntTestClass
        {
            NonNullableInteger = 12
        };
        
        intClass.FinalizeChanges();
        
        Assert.Equal(12, intClass.NonNullableInteger);
        Assert.Null(intClass.NullableInteger);

        var history = intClass.GetEntityChanges();
        
        Assert.Single(history);

        var firstEvent = history[0];
        
        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);
        
        var auditableEntityCreated = firstEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);
        
        intClass.Commit();
        
        var intHistoryClass = AuditableRootEntity.LoadFromHistory<IntTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(intHistoryClass);
        Assert.Equal(12, intHistoryClass.NonNullableInteger);
        Assert.Null(intHistoryClass.NullableInteger);

        intHistoryClass.NullableInteger = 24;
        
        intHistoryClass.FinalizeChanges();
        
        Assert.Equal(12, intHistoryClass.NonNullableInteger);
        Assert.Equal(24, intHistoryClass.NullableInteger);
        
        var history2 = intHistoryClass.GetEntityChanges();
        
        Assert.Single(history2);
        
        var firstEvent2 = history2[0];
        
        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);
        
        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated.ValueFieldEvents);
        
        intHistoryClass.Commit();

        history.AddRange(history2);
        var intHistoryClassTwo = AuditableRootEntity.LoadFromHistory<IntTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(intHistoryClassTwo);
        Assert.Equal(12, intHistoryClassTwo.NonNullableInteger);
        Assert.NotNull(intHistoryClassTwo.NullableInteger);
        Assert.Equal(24, intHistoryClassTwo.NullableInteger);

        intHistoryClassTwo.NullableInteger = null;
        
        Assert.Null(intHistoryClassTwo.NullableInteger);
        
        intHistoryClassTwo.FinalizeChanges();
        var history3 = intHistoryClassTwo.GetEntityChanges();
        
        Assert.Single(history3);
        
        var firstEvent3 = history3[0];
        
        Assert.NotNull(firstEvent3);
        Assert.IsType<AuditableEntityUpdated>(firstEvent3);
        
        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated2);
        Assert.NotNull(auditableEntityUpdated2.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated2.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated2.ValueFieldEvents);
        
        intHistoryClassTwo.Commit();
        
        history.AddRange(history3);
        var intHistoryClassThree = AuditableRootEntity.LoadFromHistory<IntTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(intHistoryClassThree);
        Assert.Equal(12, intHistoryClassThree.NonNullableInteger);
        Assert.Null(intHistoryClassThree.NullableInteger);
    }
    
    public class IntTestClass : AuditableRootEntity
    {
        [AuditableValueField<int?>(true)]
        public int? NullableInteger
        {
            get => GetValue<int?>(nameof(NullableInteger));
            set => SetValue<int?>(value, nameof(NullableInteger));
        }
    
        [AuditableValueField<int>(false)]
        public int NonNullableInteger
        {
            get => GetValue<int>(nameof(NonNullableInteger));
            set => SetValue<int>(value, nameof(NonNullableInteger));
        }
    
        public IntTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
        public IntTestClass() { }
    }
}