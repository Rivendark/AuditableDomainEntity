using AuditableDomainEntity.Events.EntityEvents;

namespace AuditableDomainEntity.Tests;

public class AuditableFieldTests
{
    [Fact]
    public void Should_Generate_FieldInitializedAndUpdatedEvents()
    {
        var aggregateRootId = new AggregateRootId(Ulid.NewUlid(), typeof(TestRootEntity));
        var testClass = new TestRootEntity(aggregateRootId)
        {
            Number = 3,
            Date = new DateTime(2021, 12, 10)
        };
        testClass.Save();
        var changes = testClass.GetEntityChanges();
        
        Assert.NotNull(changes);
        Assert.NotEmpty(changes);
        Assert.Single(changes);
        
        var domainEvent = changes.Single();
        Assert.NotNull(domainEvent);
        Assert.Equal(aggregateRootId.Value, domainEvent.EntityId);
        
        var fieldEvents = domainEvent.FieldEvents;
        
        Assert.NotNull(fieldEvents);
        Assert.NotEmpty(fieldEvents);
        Assert.Equal(2, fieldEvents.Count);
        
        testClass.Number = 4;
        
        testClass.Save();
        var updatedChanges = testClass.GetEntityChanges();
        
        Assert.NotNull(updatedChanges);
        Assert.NotEmpty(updatedChanges);
        Assert.Equal(2, updatedChanges.Count);

        Assert.IsType<AuditableEntityCreated>(updatedChanges[0]);
        
        var auditableEntityCreated = (AuditableEntityCreated)updatedChanges[0];
        Assert.NotNull(auditableEntityCreated);
        Assert.NotNull(auditableEntityCreated.FieldEvents);
        Assert.Equal(2, auditableEntityCreated.FieldEvents.Count);
        
        Assert.IsType<AuditableEntityUpdated>(updatedChanges[1]);
        var auditableEntityUpdated = (AuditableEntityUpdated)updatedChanges[1];
        Assert.NotNull(auditableEntityUpdated);
        Assert.NotNull(auditableEntityUpdated.FieldEvents);
        Assert.Single(auditableEntityUpdated.FieldEvents);
    }

    [Fact]
    public void Should_LoadDataFromAuditHistory_IntoFields()
    {
        var aggregateRootId = new AggregateRootId(Ulid.NewUlid(), typeof(TestRootEntity));
        var preloadClass = new TestRootEntity(aggregateRootId)
        {
            Number = 3,
            Date = new DateTime(2021, 12, 10),
        };
        
        preloadClass.Save();
        var changes = preloadClass.GetEntityChanges();
        preloadClass.Commit();

        var testClassNew = new TestRootEntity(aggregateRootId, changes);
        
        var instantiatedChanges = testClassNew.GetEntityChanges();
        Assert.NotNull(instantiatedChanges);
        Assert.Empty(instantiatedChanges);
        Assert.Equal(preloadClass.Number, testClassNew.Number);
        Assert.Equal(preloadClass.Date, testClassNew.Date);
        
        testClassNew.Number = 4;
        
        testClassNew.Save();
        var updatedChanges = testClassNew.GetEntityChanges();
        Assert.NotNull(updatedChanges);
        Assert.NotEmpty(updatedChanges);
        Assert.Single(updatedChanges);
        Assert.Equal(4, testClassNew.Number);
    }
}