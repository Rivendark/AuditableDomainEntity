using AuditableDomainEntity.Events.EntityEvents;
using AuditableDomainEntity.Events.ValueFieldEvents;

namespace AuditableDomainEntity.Tests;

public class GeneralAuditableFieldTests
{
    [Fact]
    public void Should_Generate_FieldInitializedAndUpdatedEvents()
    {
        var aggregateRootId = new AggregateRootId(Ulid.NewUlid(), typeof(TestValueTypeRootEntity));
        var testClass = new TestValueTypeRootEntity(aggregateRootId)
        {
            Number = 3,
            Date = new DateTime(2021, 12, 10)
        };
        testClass.FinalizeChanges();
        var changes = testClass.GetEntityChanges();
        
        Assert.IsType<AuditableEntityCreated>(changes[0]);
        
        var auditableEntityCreated = (AuditableEntityCreated)changes[0];
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.NotEmpty(auditableEntityCreated.ValueFieldEvents);
        Assert.NotNull(auditableEntityCreated.EntityFieldEvents);
        Assert.Empty(auditableEntityCreated.EntityFieldEvents);
        Assert.Equal(aggregateRootId.Value, auditableEntityCreated.EntityId);
        
        var fieldEvents = auditableEntityCreated.ValueFieldEvents;
        
        Assert.NotNull(fieldEvents);
        Assert.NotEmpty(fieldEvents);
        Assert.Equal(2, fieldEvents.Count);
        
        testClass.Commit();
        
        Assert.Empty(testClass.GetEntityChanges());
        
        testClass.Number = 4;
        
        testClass.FinalizeChanges();
        var updatedChanges = testClass.GetEntityChanges();
        
        Assert.NotNull(updatedChanges);
        Assert.NotEmpty(updatedChanges);
        Assert.Single(updatedChanges);
        
        Assert.IsType<AuditableEntityUpdated>(updatedChanges[0]);
        var auditableEntityUpdated = (AuditableEntityUpdated)updatedChanges[0];
        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.ValueFieldEvents);
        Assert.Single(auditableEntityUpdated.ValueFieldEvents);
        
        testClass.Commit();
        
        Assert.Empty(testClass.GetEntityChanges());
    }

    [Fact]
    public void Should_LoadDataFromAuditHistory_IntoFields()
    {
        var aggregateRootId = new AggregateRootId(Ulid.NewUlid(), typeof(TestValueTypeRootEntity));
        var preloadClass = new TestValueTypeRootEntity(aggregateRootId)
        {
            Number = 3,
            Date = new DateTime(2021, 12, 10),
        };
        
        preloadClass.FinalizeChanges();
        var changes = preloadClass.GetEntityChanges();
        preloadClass.Commit();

        var testClassNew = AuditableRootEntity.LoadFromHistory<TestValueTypeRootEntity>(aggregateRootId, changes);
        
        Assert.NotNull(testClassNew);
        
        var instantiatedChanges = testClassNew.GetEntityChanges();
        Assert.NotNull(instantiatedChanges);
        Assert.Empty(instantiatedChanges);
        Assert.Equal(preloadClass.Number, testClassNew.Number);
        Assert.Equal(preloadClass.Date, testClassNew.Date);
        
        testClassNew.Number = 4;
        
        testClassNew.FinalizeChanges();
        var updatedChanges = testClassNew.GetEntityChanges();
        Assert.NotNull(updatedChanges);
        Assert.NotEmpty(updatedChanges);
        Assert.Single(updatedChanges);
        Assert.Equal(4, testClassNew.Number);
        Assert.Equal(3, preloadClass.Number);
    }

    [Fact]
    public void Should_SaveHistoryFromEntityField()
    {
        var aggregateRootId = new AggregateRootId(Ulid.NewUlid(), typeof(TestRootEntity));
        var entityTestClass = new TestRootEntity(aggregateRootId)
        {
            Child = new TestChildEntity
            {
                BoolProperty = true
            }
        };

        entityTestClass.Child.IntProperty = 14;
        
        entityTestClass.FinalizeChanges();
        var changes = entityTestClass.GetEntityChanges();
        entityTestClass.Commit();
        Assert.NotNull(changes);
        Assert.NotEmpty(changes);
        Assert.Equal(2, changes.Count);
        
        Assert.IsType<AuditableEntityCreated>(changes[0]);
        var auditableEntityCreated = (AuditableEntityCreated)changes[0];
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.Empty(auditableEntityCreated.ValueFieldEvents);
        Assert.NotNull(auditableEntityCreated.EntityFieldEvents);
        Assert.Single(auditableEntityCreated.EntityFieldEvents);
        
        var auditableChildEntityCreated = (AuditableEntityCreated)changes[1];
        Assert.NotNull(auditableChildEntityCreated);
        Assert.NotNull(auditableChildEntityCreated.ValueFieldEvents);
        Assert.Equal(2, auditableChildEntityCreated.ValueFieldEvents.Count);

        var boolType = typeof(AuditableValueFieldInitialized<bool?>);
        var boolProperty = auditableChildEntityCreated.ValueFieldEvents[0];
        Assert.NotNull(boolProperty);
        Assert.True(boolType.IsInstanceOfType(boolProperty));
        var boolAuditable = boolProperty as AuditableValueFieldInitialized<bool?>;
        Assert.NotNull(boolAuditable);
        Assert.Equal(true, boolAuditable.InitialValue);
        
        var intType = typeof(AuditableValueFieldInitialized<int?>);
        var intProperty = auditableChildEntityCreated.ValueFieldEvents[1];
        Assert.NotNull(intProperty);
        Assert.True(intType.IsInstanceOfType(intProperty));
        var intAuditable = intProperty as AuditableValueFieldInitialized<int?>;
        Assert.NotNull(intAuditable);
        Assert.Equal(14, intAuditable.InitialValue);
    }

    [Fact]
    public void Should_LoadDataFromAuditHistory_IntoFieldsWithChildren()
    {
        var aggregateRootId = new AggregateRootId(Ulid.NewUlid(), typeof(TestRootEntity));
        var entityTestClass = new TestRootEntity(aggregateRootId)
        {
            Child = new TestChildEntity
            {
                BoolProperty = true
            },
            StringProperty = "TestString"
        };

        entityTestClass.Child.IntProperty = 14;
        
        entityTestClass.FinalizeChanges();
        var changes = entityTestClass.GetEntityChanges();
        entityTestClass.Commit();
        
        Assert.NotNull(changes);
        Assert.NotEmpty(changes);
        Assert.Equal(2, changes.Count);
        Assert.IsType<AuditableEntityCreated>(changes[0]);
        var auditableEntityCreated = (AuditableEntityCreated)changes[0];
        Assert.NotNull(auditableEntityCreated);
        Assert.Equal(aggregateRootId, auditableEntityCreated.Id);
        Assert.NotNull(auditableEntityCreated.ValueFieldEvents);
        Assert.Single(auditableEntityCreated.ValueFieldEvents);
        Assert.NotNull(auditableEntityCreated.EntityFieldEvents);
        Assert.Single(auditableEntityCreated.EntityFieldEvents);
        
        var auditableChildEntityCreated = (AuditableEntityCreated)changes[1];
        Assert.NotNull(auditableChildEntityCreated);
        Assert.NotNull(auditableChildEntityCreated.ValueFieldEvents);
        Assert.Equal(2, auditableChildEntityCreated.ValueFieldEvents.Count);
        
        var boolType = typeof(AuditableValueFieldInitialized<bool?>);
        var boolProperty = auditableChildEntityCreated.ValueFieldEvents[0];
        Assert.NotNull(boolProperty);
        Assert.True(boolType.IsInstanceOfType(boolProperty));
        var boolAuditable = boolProperty as AuditableValueFieldInitialized<bool?>;
        Assert.NotNull(boolAuditable);
        Assert.Equal(true, boolAuditable.InitialValue);
        
        var intType = typeof(AuditableValueFieldInitialized<int?>);
        var intProperty = auditableChildEntityCreated.ValueFieldEvents[1];
        Assert.NotNull(intProperty);
        Assert.True(intType.IsInstanceOfType(intProperty));
        var intAuditable = intProperty as AuditableValueFieldInitialized<int?>;
        Assert.NotNull(intAuditable);
        Assert.Equal(14, intAuditable.InitialValue);
        
        var historyTestClass = AuditableRootEntity.LoadFromHistory<TestRootEntity>(aggregateRootId, changes);
        
        Assert.NotNull(historyTestClass);
        
        var instantiatedChanges = historyTestClass.GetEntityChanges();
        Assert.NotNull(instantiatedChanges);
        Assert.Empty(instantiatedChanges);
        
        Assert.NotNull(historyTestClass.Child);
        Assert.Equal(entityTestClass.Child.IntProperty, historyTestClass.Child.IntProperty);
        Assert.Equal(entityTestClass.StringProperty, historyTestClass.StringProperty);
    }
}