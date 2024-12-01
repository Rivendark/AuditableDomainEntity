using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Events.EntityEvents;

namespace AuditableDomainEntity.Tests.ValueFields;

public class DecimalValueFieldTests
{
    [Fact]
    public void ValueField_Should_HandleDecimalTypes()
    {
        var decimalTestClass = new DecimalTestClass
        {
            NonNullableDecimal = 12.0m
        };
        
        decimalTestClass.FinalizeChanges();
        
        Assert.Equal(12.0m, decimalTestClass.NonNullableDecimal);
        Assert.Null(decimalTestClass.NullableDecimal);
        
        var history = decimalTestClass.GetEntityChanges();
        
        Assert.Single(history);
        
        var firstEvent = history[0];
        
        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);
        
        var auditableEntityCreated = firstEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);
        
        decimalTestClass.Commit();
        
        var decimalHistoryClass = AuditableRootEntity.LoadFromHistory<DecimalTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(decimalHistoryClass);
        Assert.Equal(12.0m, decimalHistoryClass.NonNullableDecimal);
        Assert.Null(decimalHistoryClass.NullableDecimal);
        
        decimalHistoryClass.NullableDecimal = 24.0m;
        
        decimalHistoryClass.FinalizeChanges();
        
        Assert.Equal(12.0m, decimalHistoryClass.NonNullableDecimal);
        Assert.Equal(24.0m, decimalHistoryClass.NullableDecimal);
        
        var history2 = decimalHistoryClass.GetEntityChanges();
        
        Assert.Single(history2);
        
        var firstEvent2 = history2[0];
        
        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);
        
        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated.ValueFieldEvents);
        
        decimalHistoryClass.Commit();
        
        history.AddRange(history2);
        var decimalHistoryClassTwo = AuditableRootEntity.LoadFromHistory<DecimalTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(decimalHistoryClassTwo);
        Assert.Equal(12.0m, decimalHistoryClassTwo.NonNullableDecimal);
        Assert.Equal(24.0m, decimalHistoryClassTwo.NullableDecimal);
        
        decimalHistoryClassTwo.NullableDecimal = null;
        
        Assert.Null(decimalHistoryClassTwo.NullableDecimal);
        
        decimalHistoryClassTwo.FinalizeChanges();
        var history3 = decimalHistoryClassTwo.GetEntityChanges();
        
        Assert.Single(history3);
        
        var firstEvent3 = history3[0];
        
        Assert.NotNull(firstEvent3);
        Assert.IsType<AuditableEntityUpdated>(firstEvent3);
        
        var auditableEntityUpdated2 = firstEvent3 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated2);
        Assert.NotNull(auditableEntityUpdated2.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated2.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated2.ValueFieldEvents);
        
        decimalHistoryClassTwo.Commit();
        
        history.AddRange(history3);
        var decimalHistoryClassThree = AuditableRootEntity.LoadFromHistory<DecimalTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(decimalHistoryClassThree);
        Assert.Equal(12.0m, decimalHistoryClassThree.NonNullableDecimal);
        Assert.Null(decimalHistoryClassThree.NullableDecimal);
    }
    
    public class DecimalTestClass : AuditableRootEntity
    {
        [AuditableValueField<decimal?>(true)]
        public decimal? NullableDecimal
        {
            get => GetValue<decimal?>(nameof(NullableDecimal));
            set => SetValue<decimal?>(value, nameof(NullableDecimal));
        }

        [AuditableValueField<decimal>(false)]
        public decimal NonNullableDecimal
        {
            get => GetValue<decimal>(nameof(NonNullableDecimal));
            set => SetValue(value, nameof(NonNullableDecimal));
        }
    
        public DecimalTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
        public DecimalTestClass() { }
    }
}