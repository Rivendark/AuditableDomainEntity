using AuditableDomainEntity.Attributes;
using AuditableDomainEntity.Collections.Lists;
using AuditableDomainEntity.Events.CollectionEvents.ListEvents;
using AuditableDomainEntity.Events.EntityEvents;
using AuditableDomainEntity.Events.ValueFieldEvents;

namespace AuditableDomainEntity.Tests.EnumerableValueFields.Lists;

public class IntListCollectionValueFieldTests
{
    [Fact]
    public void ValueField_Should_HandleIntListTypes()
    {
        var nullableList = new List<int>([4, 5, 6]);
        var intListClass = new IntListTestClass
        {
            NonNullableIntegerList = [1, 2, 3],
            NullableIntegerList = nullableList
        };

        intListClass.FinalizeChanges();

        Assert.Equal([1, 2, 3], intListClass.NonNullableIntegerList);
        Assert.Equal([4, 5, 6], intListClass.NullableIntegerList);
        
        var history = intListClass.GetEntityChanges();
        
        Assert.Single(history);
        
        var firstEvent = history[0];
        
        Assert.NotNull(firstEvent);
        Assert.IsType<AuditableEntityCreated>(firstEvent);
        
        var auditableEntityCreated = firstEvent as AuditableEntityCreated;
        
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.Equal(7, auditableEntityCreated.ValueFieldEvents.Count);
        Assert.Equal(2, auditableEntityCreated.ValueFieldEvents.Count(e => e is AuditableValueFieldInitialized<int[]>));
        Assert.Equal(2, auditableEntityCreated.ValueFieldEvents.Count(e => e is AuditableListInitialized<int>));
        Assert.Equal(3, auditableEntityCreated.ValueFieldEvents.Count(e => e is AuditableListItemAdded<int>));
        
        intListClass.Commit();
        
        var intListHistoryClass = AuditableRootEntity.LoadFromHistory<IntListTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(intListHistoryClass);
        Assert.Equal([1, 2, 3], intListHistoryClass.NonNullableIntegerList);
        Assert.Equal([4, 5, 6], intListHistoryClass.NullableIntegerList);
        
        intListHistoryClass.NullableIntegerList = null;
        
        intListHistoryClass.FinalizeChanges();
        
        Assert.Equal([1, 2, 3], intListHistoryClass.NonNullableIntegerList);
        Assert.Null(intListHistoryClass.NullableIntegerList);
        
        var history2 = intListHistoryClass.GetEntityChanges();
        
        Assert.Single(history2);
        
        var firstEvent2 = history2[0];
        
        Assert.NotNull(firstEvent2);
        Assert.IsType<AuditableEntityUpdated>(firstEvent2);
        
        var auditableEntityUpdated = firstEvent2 as AuditableEntityUpdated;
        
        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityUpdated.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated.ValueFieldEvents);
        
        intListHistoryClass.Commit();
        
        history.AddRange(history2);
        
        var intListHistoryClass2 = AuditableRootEntity.LoadFromHistory<IntListTestClass>(auditableEntityCreated.Id, history);
        
        Assert.NotNull(intListHistoryClass2);
        Assert.Equal([1, 2, 3], intListHistoryClass2.NonNullableIntegerList);
        Assert.Null(intListHistoryClass2.NullableIntegerList);
        
        intListHistoryClass2.FinalizeChanges();
        var history3 = intListHistoryClass2.GetEntityChanges();
        
        Assert.Empty(history3);
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