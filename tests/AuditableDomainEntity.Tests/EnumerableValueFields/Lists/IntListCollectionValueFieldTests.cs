using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Collections;
using AuditableDomainEntity.Events.EntityEvents;

namespace AuditableDomainEntity.Tests.EnumerableValueFields.Lists;

public class IntListCollectionValueFieldTests
{
    [Fact]
    public void ValueField_Should_HandleIntListTypes()
    {
        var intListClass = new IntListTestClass
        {
            NonNullableIntegerList = [1, 2, 3]
        };

        intListClass.FinalizeChanges();

        Assert.Equal([1, 2, 3], intListClass.NonNullableIntegerList);
        Assert.Null(intListClass.NullableIntegerList);
        
        var history = intListClass.GetEntityChanges();
        
        Assert.Single(history);
        
        var firstEvent = history[0];
        
        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);
        
        var auditableEntityCreated = firstEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);
        
        intListClass.Commit();
        
        var intListHistoryClass = AuditableRootEntity.LoadFromHistory<IntListTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(intListHistoryClass);
        Assert.Equal([1, 2, 3], intListHistoryClass.NonNullableIntegerList);
        Assert.Null(intListHistoryClass.NullableIntegerList);
        
        intListHistoryClass.NullableIntegerList = [4, 5, 6];
        intListHistoryClass.NonNullableIntegerList.AddRange([7, 8, 9]);
        intListHistoryClass.NonNullableIntegerList.Remove(1);
        intListHistoryClass.NullableIntegerList.Add(7);
        
        var nonNullableIntegerList = intListHistoryClass.NonNullableIntegerList;

        Assert.NotNull(nonNullableIntegerList);
        Assert.IsType<AuditableList<int>>(nonNullableIntegerList);
        
        intListHistoryClass.FinalizeChanges();
        
        Assert.Equal([2, 3, 7, 8, 9], intListHistoryClass.NonNullableIntegerList);
        Assert.Equal([4, 5, 6, 7], intListHistoryClass.NullableIntegerList);
        
        var history2 = intListHistoryClass.GetEntityChanges();
        
        Assert.Single(history2);
        
        var firstEvent2 = history2[0];
        
        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);
        
        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);
        Assert.Equal(3, auditableEntityUpdated.ValueFieldEvents.Count);
        
        intListHistoryClass.Commit();
        
        history.AddRange(history2);
        var intListHistoryClass2 = AuditableRootEntity.LoadFromHistory<IntListTestClass>(auditableEntityUpdated.Id, history);
        
        Assert.NotNull(intListHistoryClass2);
        Assert.Equal([2, 3, 7, 8, 9], intListHistoryClass2.NonNullableIntegerList);
        Assert.Equal([4, 5, 6, 7], intListHistoryClass2.NullableIntegerList);
    }

    private class IntListTestClass : AuditableRootEntity
    {
        [AuditableValueListField<int>(true)]
        public AuditableList<int>? NullableIntegerList
        {
            get => GetValueList<int>(nameof(NullableIntegerList));
            set => SetValueList<int>(value, nameof(NullableIntegerList));
        }
        
        [AuditableValueListField<int>(false)]
        public AuditableList<int> NonNullableIntegerList
        {
            get => GetValueList<int>(nameof(NonNullableIntegerList));
            set => SetValueList<int>(value, nameof(NonNullableIntegerList));
        }
        
        public IntListTestClass(AggregateRootId aggregateRootId) : base(aggregateRootId) { }
        public IntListTestClass() { }
    }
}